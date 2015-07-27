using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Controls;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Extension;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.FE.Model.Bill;
using BinLaw.Account.FE.Model.Common;
using BinLaw.Account.Helper;
using BinLaw.Account.Model;
using BinLaw.Account.Service;
using BinLaw.Account.View;
using BinLaw.Account.View.Assert;
using BinLaw.Account.View.Bill;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAzure.MobileServices;

namespace BinLaw.Account.ViewModel
{
    /// <summary>
    /// This Class is MainPage ViewModel
    /// It show base account info
    /// </summary>
    /// <author>
    /// BinLaw
    /// </author>
    public class MainViewModel : ViewModelBase
    {
        #region [ Data ]
        private readonly INavigationService _navigationService;
        private readonly DataService _dataService;

        private ObservableCollection<BillModel> _lastestAccount;
        private ObservableCollection<KeyValueData<DateTime, double>> _lastestBalance;
        private BillModel _selectedDetailAccountModel;
        private double _totalExpanseMoney;
        private double _totalIncomeMoney;
        private bool _isFirst;

        #region 总支出 TotalExpanseMoney
        /// <summary>
        /// 总支出
        /// </summary>
        public double TotalExpanseMoney
        {
            get
            {
                return _totalExpanseMoney;
            }
            set
            {
                Set(() => TotalExpanseMoney, ref _totalExpanseMoney, value);
                RaisePropertyChanged(() => TotalMoney);
            }
        }
        #endregion

        #region 总收入 TotalIncomeMoney

        /// <summary>
        /// 总收入
        /// </summary>
        public double TotalIncomeMoney
        {
            get { return _totalIncomeMoney; }
            set
            {
                Set(() => TotalIncomeMoney, ref _totalIncomeMoney, value);
                RaisePropertyChanged(() => TotalMoney);
            }
        }
        #endregion

        #region 总资产 TotalMoney
        /// <summary>
        /// 总资产
        /// </summary>
        public double TotalMoney
        {
            get { return TotalIncomeMoney - TotalExpanseMoney; }
        }
        #endregion

        #region 最近总账单 LastestAccount
        /// <summary>
        /// 最近账单
        /// </summary>
        public ObservableCollection<BillModel> LastestAccount
        {
            get { return _lastestAccount; }
            set
            {
                Set(() => LastestAccount, ref _lastestAccount, value);
                RaisePropertyChanged(() => LastestIncomeAccount);
                RaisePropertyChanged(() => LastestExpanseAccount);
            }
        }
        #endregion

        #region 最近余额 LastestBalance
        /// <summary>
        /// 最近余额
        /// </summary>
        public ObservableCollection<KeyValueData<DateTime, double>> LastestBalance
        {
            get { return _lastestBalance; }
            set { Set(() => LastestBalance, ref _lastestBalance, value); }
        }
        #endregion

        #region 最近收入账单 LastestIncomeAccount
        /// <summary>
        /// 最近收入账单
        /// </summary>
        public ObservableCollection<BillModel> LastestIncomeAccount
        {
            get
            {
                return LastestAccount != null ? LastestAccount.Where(a => a.BillType == (int)BillTypeEnum.Income).ToObserableCollection() : null;
            }
        }
        #endregion

        #region 最近支出账单 LastestExpanseAccount
        /// <summary>
        /// 最近支出账单
        /// </summary>
        public ObservableCollection<BillModel> LastestExpanseAccount
        {
            get
            {
                return LastestAccount != null ? LastestAccount.Where(a => a.BillType == (int)BillTypeEnum.Expanse).ToObserableCollection() : null;
            }
        }
        #endregion

        #region 当前选中的账单记录 SelectedAccountModel
        /// <summary>
        /// 当前选中的账单记录
        /// </summary>
        public BillModel SelectedDetailAccountModel
        {
            get { return _selectedDetailAccountModel; }
            set { Set(() => SelectedDetailAccountModel, ref _selectedDetailAccountModel, value); }
        }
        #endregion

        #region 是否已登陆账号 IsLogin
        public bool IsLogin
        {
            get { return App.MobileService.CurrentUser != null; }
        }
        #endregion
        #endregion

        #region [ Ctor ]
        public MainViewModel(INavigationService navigationService, DataService dataService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            _isFirst = true;

            Messenger.Default.Register<NotificationMessage>(this, Constants.SQLITEDATEBASE_CHANGE_TOKEN_MAIN, async m =>
            {
                await InitialAsync();
            });
        }

        #endregion

        #region [ Command ]

        private RelayCommand _onPageLoadCommand;
        private RelayCommand _navigatedToAddAccountCommand;
        private RelayCommand<SelectionChangedEventArgs> _navigatedToDetailCommand;
        private RelayCommand _navigatedToListAccountCommand;
        private RelayCommand _navigatedToAboutCommand;
        private RelayCommand _navigatedToSettingCommand;
        private RelayCommand<string> _navigatedToAssertCommand;
        private RelayCommand _refreshSyncDataCommand;

        #region 页面加载完毕后执行 OnPageLoadCommand
        /// <summary>
        /// It will execute when page loaded
        /// 当页面载入完毕后执行的事件
        /// </summary>
        public RelayCommand OnPageLoadCommand
        {
            get
            {
                return _onPageLoadCommand ?? (_onPageLoadCommand = new RelayCommand(async () =>
                {
                    if (_isFirst)
                    {
                        try
                        {
                            await LoadAuthenticateAsync();
                            await _dataService.PullAsync<BillModel>();
                            RaisePropertyChanged(() => IsLogin);
                        }
                        catch
                        {
                            //ignore error
                        }

                        _isFirst = false;
                    }
                    await InitialAsync();
                }));
            }
        }
        #endregion

        #region 导航到添加账单 NavigatedToAddAccountCommand
        /// <summary>
        /// Navigate to Add Bill Command From Appbar
        /// 导航到添加账单
        /// </summary>
        public RelayCommand NavigatedToAddAccountCommand
        {
            get
            {
                return _navigatedToAddAccountCommand ??
                       (_navigatedToAddAccountCommand =
                           new RelayCommand(() => _navigationService.Navigate<AddBillPage>(), () => true));
            }
        }
        #endregion

        #region 导航到账单详情 NavigatedToDetailSelectionChangedCommand
        /// <summary>
        /// Navigate to Detail Bill Command From Listview Selected
        /// 导航到账单详情
        /// </summary>
        public RelayCommand<SelectionChangedEventArgs> NavigatedToDetailSelectionChangedCommand
        {
            get
            {
                return _navigatedToDetailCommand ??
                       (_navigatedToDetailCommand =
                           new RelayCommand<SelectionChangedEventArgs>(e =>
                           {
                               if (SelectedDetailAccountModel == null) return;

                               _navigationService.Navigate<DetailBillPage>();
                               Messenger.Default.Send(SelectedDetailAccountModel);
                               SelectedDetailAccountModel = null;
                           }, a => a != null));
            }
        }
        #endregion

        #region 导航到所有账单列表 NavigatedToListAccountCommand
        /// <summary>
        /// Navigate to All Bill List
        /// 导航到所有账单列表
        /// </summary>
        public RelayCommand NavigatedToListAccountCommand
        {
            get
            {
                return _navigatedToListAccountCommand ?? (_navigatedToListAccountCommand = new RelayCommand(() =>
                    {
                        _navigationService.Navigate<ListBillPage>();
                    }));
            }
        }
        #endregion

        #region 导航到关于页面 NavigatedToAboutCommand
        /// <summary>
        /// Navigated To About Page
        /// 导航到关于页面
        /// </summary>
        public RelayCommand NavigatedToAboutCommand
        {
            get { return _navigatedToAboutCommand ?? (_navigatedToAboutCommand = new RelayCommand(() => _navigationService.Navigate<AboutPage>())); }
        }
        #endregion

        #region 导航到设置页面 NavigatedToSettingCommand
        /// <summary>
        /// Navigated To About Page
        /// 导航到设置页面
        /// </summary>
        public RelayCommand NavigatedToSettingCommand
        {
            get { return _navigatedToSettingCommand ?? (_navigatedToSettingCommand = new RelayCommand(() => _navigationService.Navigate<View.Setting.SettingPage>())); }
        }
        #endregion

        #region 导航到资产配置页面 NavigatedToSettingCommand

        /// <summary>
        /// Navigated To Assert Page
        /// 导航到资产配置页面
        /// </summary>
        public RelayCommand<string> NavigatedToAssertCommand
        {
            get
            {
                return _navigatedToAssertCommand ?? (_navigatedToAssertCommand = new RelayCommand<string>(type =>
                {
                    _navigationService.Navigate<AllocationAssertPage>();
                    Messenger.Default.Send(int.Parse(type), Constants.ASSERT_BILLTYPE_TOKEN);
                }));

            }
        }

        #endregion

        #region 刷新同步数据 RefreshSyncDataCommand
        /// <summary>
        /// 刷新同步数据
        /// </summary>
        public RelayCommand RefreshSyncDataCommand
        {
            get
            {
                return _refreshSyncDataCommand ?? (_refreshSyncDataCommand = new RelayCommand(async () =>
                    {
                        await _dataService.PullAsync<BillModel>();
                        await _dataService.PushAsync();
                    }));
            }
        }
        #endregion
        #endregion

        #region [ Method ]
        #region 初始化页面数据 InitialAsync
        private async Task InitialAsync()
        {
            LastestAccount = new ObservableCollection<BillModel>(await _dataService.GetQueryAsync<BillModel>().OrderByDescending(a => a.DateTime).Take(Constants.LASTESTACCOUNT_COUNT).ToListAsync());

            TotalExpanseMoney = AppSettingHelper.GetValueOrDefault(BillHelper.GetSettingConstantEveryMoney((int)BillTypeEnum.Expanse), 0d);
            TotalIncomeMoney = AppSettingHelper.GetValueOrDefault(BillHelper.GetSettingConstantEveryMoney((int)BillTypeEnum.Income), 0d);

            if (LastestBalance == null) LastestBalance = new ObservableCollection<KeyValueData<DateTime, double>>();
            else LastestBalance.Clear();

            double balance = 0;
            for (int i = LastestAccount.Count - 1; i >= 0; i--)
            {
                
                if (i == LastestAccount.Count - 1)
                    balance = LastestAccount[i].BillType == (int)BillTypeEnum.Expanse ? TotalMoney - LastestAccount[i].Money : TotalMoney + LastestAccount[i].Money;
                else
                {
                    balance = LastestAccount[i].BillType == (int)BillTypeEnum.Expanse ? balance - LastestAccount[i].Money : balance + LastestAccount[i].Money;
                }
                LastestBalance.Add(new KeyValueData<DateTime, double>(LastestAccount[i].DateTime, balance));
            }

        }
        #endregion

        #region 载入授权验证信息 LoadAuthenticateAsync
        private async Task LoadAuthenticateAsync()
        {
            MobileServiceUser user;
            var provider = "MicrosoftAccount";

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;
            try
            {
                // Try to get an existing credential from the vault.
                credential = vault.FindAllByResource(provider).FirstOrDefault();
            }
            catch (Exception)
            {
                // When there is no matching resource an error occurs, which we ignore.
            }

            if (credential != null)
            {
                // Create a user from the stored credentials.
                user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                App.MobileService.CurrentUser = user;

                try
                {
                    // Try to return an item now to determine if the cached credential has expired.
                    await App.MobileService.GetTable<BillModel>().Take(1).ToListAsync();
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Remove the credential with the expired token.
                        vault.Remove(credential);
                        credential = null;
                    }
                }
            }
        }
        #endregion
        #endregion
    }

}
