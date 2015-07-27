using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Credentials;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.FE.Model.Bill;
using BinLaw.Account.FE.Model.Common;
using BinLaw.Account.Helper;
using BinLaw.Account.Model;
using BinLaw.Account.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace BinLaw.Account.Service
{
    public class DataService
    {
        private readonly CancellationToken cts = new CancellationToken();
        private readonly Action<string> _dataChangehangeAction;

        public DataService()
        {
            _dataChangehangeAction = param =>
            {
                Messenger.Default.Send(new NotificationMessage(param), Constants.SQLITEDATEBASE_CHANGE_TOKEN_MAIN);
                Messenger.Default.Send(new NotificationMessage(param), Constants.SQLITEDATABASE_CHANGE_TOKEN_LIST);
            };

            if (!DesignMode.DesignModeEnabled)
                InitialDataServiceAsync();
        }

        private async void InitialDataServiceAsync()
        {
            if (!App.MobileService.SyncContext.IsInitialized)
            {
                var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, Constants.SQLITEDATABASE_BILL_TABLENAME);
                var store = new MobileServiceSQLiteStore(dbPath);
                store.DefineTable<BillModel>();
                await App.MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
            }

        }

        public async Task InsertAsync<T>(T obj) where T : new()
        {
            await App.MobileService.GetSyncTable<T>().InsertAsync(obj);
            _dataChangehangeAction("Insert");
        }


        public async Task UpdateAsync<T>(T obj) where T : new()
        {
            await App.MobileService.GetSyncTable<T>().UpdateAsync(obj);
            _dataChangehangeAction("Update");
        }

        public async Task DeleteAsync<T>(T obj) where T : new()
        {
            await App.MobileService.GetSyncTable<T>().DeleteAsync(obj);
            _dataChangehangeAction("Delete");
        }

        public async Task<int> GetCountAsync<T>() where T : new()
        {
            var query = App.MobileService.GetSyncTable<T>().IncludeTotalCount();
            var results = await query.ToEnumerableAsync();
            // ReSharper disable once CSharpWarnings::CS0618
            return (int)((ITotalCountProvider)results).TotalCount;
        }

        public IMobileServiceTableQuery<T> GetQueryAsync<T>() where T : new()
        {
            return App.MobileService.GetSyncTable<T>().CreateQuery();
        }

        public async Task PushAsync()
        {
            try
            {
                if (App.MobileService.CurrentUser != null && DeviceHelper.IsNetworkAvailable && !ViewModelLocator.AppViewModel.IsBusy)
                {
                    ViewModelLocator.AppViewModel.IsBusy = true;
                    await App.MobileService.SyncContext.PushAsync(cts);
                }

            }
            catch (MobileServicePushFailedException ex)
            {

                // ReSharper disable once CSharpWarnings::CS4014
                MessageDialogHelper.ShowAsync("Push failed because of sync errors: " + ex.PushResult.Errors.Count() +
                                              ", message: " + ex.Message, "Error");
            }
            catch (Exception ex)
            {
                // ReSharper disable once CSharpWarnings::CS4014
                MessageDialogHelper.ShowAsync("Push failed: " + ex.Message, "Error");
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public async Task PullAsync<T>()
        {
            try
            {
                if (App.MobileService.CurrentUser != null && DeviceHelper.IsNetworkAvailable && !ViewModelLocator.AppViewModel.IsBusy)
                {
                    ViewModelLocator.AppViewModel.IsBusy = true;
                    var table = App.MobileService.GetSyncTable<T>();
                    try
                    {
                        //await table.PullAsync();
                    }
                    catch (MobileServiceInvalidOperationException ex)
                    {
                        if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            //token expired,clear credential
                            var provider = "MicrosoftAccount";

                            PasswordVault vault = new PasswordVault();
                            PasswordCredential credential = null;
                            try
                            {
                                credential = vault.FindAllByResource(provider).FirstOrDefault();
                            }
                            catch (Exception)
                            {
                                // When there is no matching resource an error occurs, which we ignore.
                            }
                            // Remove the credential with the expired token.
                            vault.Remove(credential);
                            MessageDialogHelper.ShowAsync("您的验证已过期，请尝试重新授权","警告");
                        }
                    }


                    //Sync Month Data
                    await SyncMonthMoney();
                }

            }
            catch (Exception ex)
            {
                // ReSharper disable once CSharpWarnings::CS4014
                MessageDialogHelper.ShowAsync("Pull failed: " + ex.Message, "Error");
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        /// <summary>
        /// 遍历所有数据同步收入或者支出的每月账单汇总
        /// </summary>
        /// <param name="billType"></param>
        /// <returns></returns>
        private async Task SyncMonthMoney(BillTypeEnum billType)
        {
            var billTable = App.MobileService.GetSyncTable<BillModel>();
            var monthData = await billTable.CreateQuery().Where(a=>a.BillType ==(int)billType)
                .Select(a => new KeyValueData<DateTime, double>(a.DateTime.Date, a.Money))
                .ToListAsync();
            foreach (var groupItem in monthData.GroupBy(a => a.Key))
            {
                var money = groupItem.Sum(a => a.Value);
                AppSettingHelper.AddOrUpdateValue(BillHelper.GetSettingConstantEveryMoney((int)billType, groupItem.Key), money);
            }
        }

        private async Task SyncMonthMoney()
        {
            await SyncMonthMoney(BillTypeEnum.Income);
            await SyncMonthMoney(BillTypeEnum.Expanse);
        }

    }
}
