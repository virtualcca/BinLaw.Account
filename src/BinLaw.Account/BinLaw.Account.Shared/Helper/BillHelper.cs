using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Common;
using BinLaw.Account.FE.Foundation.Extension;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.FE.Model.Bill;
using BinLaw.Account.FE.Model.Enum;
using BinLaw.Account.Model;

namespace BinLaw.Account.Helper
{
    public static class BillHelper
    {
        /// <summary>
        /// 账单分类信息
        /// </summary>
        public static NotifyTaskCompletion<ObservableCollection<BillCategoryItem>> CateGoryItems { get; private set; }

        static BillHelper()
        {
            CateGoryItems = new NotifyTaskCompletion<ObservableCollection<BillCategoryItem>>(getCagegoryTask());
        }

        private static async Task<ObservableCollection<BillCategoryItem>> getCagegoryTask()
        {
            try
            {
                StorageFolder local = ApplicationData.Current.LocalFolder;
                var file = await local.GetFileAsync(Constants.CATEGORY_FILENAME);
                string categoryStr = await FileIO.ReadTextAsync(file);
                return JsonHelper.Deserialize<List<BillCategoryItem>>(categoryStr).ToObserableCollection();
            }
            catch (Exception)
            {

                throw;
            }

        }

        #region 同步金额修改 SyncTotalAccount
        /// <summary>
        /// 同步金额修改
        /// </summary>
        /// <param name="bill">账单</param>
        /// <param name="money">变更的金钱</param>
        /// <param name="operationType">操作类型</param>
        public static void SyncTotalAccount(BillModel bill, double money, DataOperationEnumType operationType)
        {
            if (bill.BillType == (int)BillTypeEnum.Expanse) //支出
            {
                var expandMoney = AppSettingHelper.GetValueOrDefault(Constants.EXPAND_MONEY, 0d);
                expandMoney = operationType == DataOperationEnumType.Delete ? expandMoney - money :
                    operationType == DataOperationEnumType.Insert ? expandMoney + money :
                    money - bill.Money + expandMoney;
                AppSettingHelper.AddOrUpdateValue(Constants.EXPAND_MONEY, expandMoney);
                AppSettingHelper.AddOrUpdateValue(GetSettingConstantEveryMoney(bill.BillType, bill.DateTime), expandMoney);
            }
            else//收入
            {
                var incomeMoeny = AppSettingHelper.GetValueOrDefault(Constants.INCOME_MONEY, 0d);
                incomeMoeny = operationType == DataOperationEnumType.Delete ? incomeMoeny - money :
                    operationType == DataOperationEnumType.Insert ? incomeMoeny + money :
                    money - bill.Money + incomeMoeny;
                AppSettingHelper.AddOrUpdateValue(Constants.INCOME_MONEY, incomeMoeny);
                AppSettingHelper.AddOrUpdateValue(GetSettingConstantEveryMoney(bill.BillType, bill.DateTime), incomeMoeny);
            }
        }
        #endregion

        #region 根据id返回对应的账单类型信息 CategoryConverter
        /// <summary>
        /// 根据id返回对应的账单类型信息
        /// </summary>
        /// <returns></returns>
        public static BillCategoryItem CategoryConverter(int categoryId)
        {
            var billType = categoryId / 10000;
            var category = (categoryId % 10000) / 100;
            return CateGoryItems.Result.FirstOrDefault(a => a.BillType == billType && a.Id == category);
        }
        #endregion

        public static string CategoryNameConverter(int categoryId)
        {
            var billType = categoryId / 10000;
            var category = (categoryId % 10000) / 100;
            var result = CateGoryItems.Result.FirstOrDefault(a => a.BillType == billType && a.Id == category);
            return result != null ? result.Name : "unknown";
        }

        /// <summary>
        /// 返回每月资金的设置保存字符串
        /// 配合AppSettingHelper，用于记录每月资产信息
        /// </summary>
        /// <param name="billType"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetSettingConstantEveryMoney(int billType, DateTime? date = null)
        {
            DateTime _date;
            _date = date != null ? date.Value : DateTime.Now;
            return string.Format("{0}{1}{2}", billType == (int)BillTypeEnum.Expanse ? Constants.EXPAND_MONEY : Constants.INCOME_MONEY, _date.Year, _date.Month);
        }

    }


}
