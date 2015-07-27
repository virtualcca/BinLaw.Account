using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.FE.Model.Common;
using BinLaw.Account.Model;
using BinLaw.Account.Service;
using BinLaw.Account.View.Bill;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace BinLaw.Account.ViewModel.Bill
{
    public class ListBillViewModel : ViewModelBase
    {
        #region [ Data ]
        private readonly DataService _dataService;
        private readonly INavigationService _navigationService;

        private DateTime _startDateTime;
        private DateTime _endDateTime;

        private IncrementalLoadingModel<BillModel> _allBillModels;
        private IEnumerable<KeyValueData<int, IGrouping<int, BillModel>>> _weekBillModels;
        private IEnumerable<KeyValueData<DateTime, IGrouping<DateTime, BillModel>>> _monthBillModels;

        #region 所有账单 AllBillModels
        /// <summary>
        /// 所有账单
        /// 采用增量加载数据
        /// </summary>
        public IncrementalLoadingModel<BillModel> AllBillModels
        {
            get { return _allBillModels; }
            set { Set(() => AllBillModels, ref _allBillModels, value); }
        }
        #endregion

        #region 每周账单 WeekBillModels
        /// <summary>
        /// 每周账单
        /// 根据账单类型进行了分类
        /// </summary>
        public IEnumerable<KeyValueData<int, IGrouping<int, BillModel>>> WeekBillModels
        {
            get { return _weekBillModels; }
            set { Set(() => WeekBillModels, ref _weekBillModels, value); }
        }
        #endregion

        #region 每月账单 MonthBillModels
        /// <summary>
        /// 每周账单
        /// 根据账单类型进行了分类
        /// </summary>
        public IEnumerable<KeyValueData<DateTime, IGrouping<DateTime, BillModel>>> MonthBillModels
        {
            get { return _monthBillModels; }
            set { Set(() => MonthBillModels, ref _monthBillModels, value); }
        }
        #endregion


        public DateTime StartDateTime
        {
            get { return _startDateTime; }
            set { Set(() => StartDateTime, ref _startDateTime, value); }
        }

        public DateTime EndDateTime
        {
            get { return _endDateTime; }
            set { Set(() => EndDateTime, ref _endDateTime, value); }
        }

        #endregion

        #region [ Ctor ]
        public ListBillViewModel(INavigationService navigationService, DataService dataService)
        {
            _navigationService = navigationService;
            _dataService = dataService;

            Messenger.Default.Register<NotificationMessage>(this, Constants.SQLITEDATABASE_CHANGE_TOKEN_LIST, async m =>
            {
                var count = await _dataService.GetCountAsync<BillModel>();
                AllBillModels.TotalCount = count;
                AllBillModels.Clear();
                if (AllBillModels.HasMoreItems)
                    AllBillModels.LoadMoreItemsAsync((uint)count);
                RaisePropertyChanged(() => AllBillModels);

                
            });

            StartDateTime = DateTime.Now.AddMonths(-1).Date;
            EndDateTime = DateTime.Now.Date;
        }

        #endregion

        #region [ Command ]
        private RelayCommand _onPageLoadedCommand;
        private RelayCommand _queryBillCommand;
        private RelayCommand<BillModel> _navigatedToDetailCommand;

        #region 页面加载完毕后执行 OnPageLoadCommand
        /// <summary>
        /// It will execute when page loaded
        /// 当页面载入完毕后执行的事件
        /// </summary>
        public RelayCommand OnPageLoadedCommand
        {
            get
            {
                return _onPageLoadedCommand ?? (_onPageLoadedCommand = new RelayCommand(async () =>
                {
                    if (AllBillModels == null)
                        await QueryAllBill();

                    await QueryWeekBill();
                    await QueryMonthBill();

                   
                }));
            }
        }
        #endregion

        #region 查询所有账单列表 QueryBillCommand
        /// <summary>
        /// 查询所有账单列表
        /// </summary>
        public RelayCommand QueryBillCommand
        {
            get
            {
                return _queryBillCommand = _queryBillCommand ?? (_queryBillCommand = new RelayCommand(async () =>
                    {
                        await QueryAllBill();
                    }));
            }
        } 
        #endregion

        public RelayCommand<BillModel> NavigatedToDetailCommand
        {
            get
            {
                return _navigatedToDetailCommand ??
                       (_navigatedToDetailCommand =
                           new RelayCommand<BillModel>(e =>
                           {
                               _navigationService.Navigate<DetailBillPage>();
                               Messenger.Default.Send(e);
                           }, a => a != null));
            }
        }


        #endregion

        #region [ Method ]
        #region 查询所有账单 QueryAllBill
        /// <summary>
        /// 查询所有账单
        /// </summary>
        /// <returns></returns>
        private async Task QueryAllBill()
        {
            var endDate = EndDateTime.AddDays(1).Date;
            AllBillModels =
                            new IncrementalLoadingModel<BillModel>(
                                (uint)(await _dataService.GetCountAsync<BillModel>()),
                                async (startIndex, count) =>
                                    await _dataService.GetQueryAsync<BillModel>().Where(a => a.DateTime >= StartDateTime && a.DateTime <= endDate).OrderByDescending(a => a.DateTime).Skip(startIndex).Take(count).ToListAsync());
        } 
        #endregion

        #region 查询周账单 QueryWeekBill
        /// <summary>
        /// 查询周账单
        /// </summary>
        /// <returns></returns>
        private async Task QueryWeekBill()
        {
            var d = DateTime.Now.DayOfWeek;
            var day = d == DayOfWeek.Sunday ? 7 : (int)d;
            var foreDate = DateTime.Now.AddDays(-day + 1);
            var backData = DateTime.Now.AddDays(7 - day);
            var startDate = foreDate.Date;
            var endData = backData.AddDays(1).Date;
            WeekBillModels = from data in (
                                 await _dataService.GetQueryAsync<BillModel>().Where(
                                 a => a.DateTime >= startDate && a.DateTime <= endData).
                                 OrderByDescending(a => a.DateTime).ToListAsync()
                                 )
                             group data by data.Category into g
                             select new KeyValueData<int, IGrouping<int, BillModel>>(g.Key, g);
            RaisePropertyChanged(()=>WeekBillModels);
        }
        #endregion

        #region 查询月账单 QueryMonthBill
        /// <summary>
        /// 查询月账单
        /// </summary>
        /// <returns></returns>
        private async Task QueryMonthBill()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var endData = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23, 59, 59);
            MonthBillModels = from data in (await _dataService.GetQueryAsync<BillModel>().Where(a => a.DateTime >= startDate && a.DateTime <= endData).
                                  OrderByDescending(a => a.DateTime).OrderBy(a=>a.Category).ToListAsync())
                              group data by data.DateTime.Date into g
                              select new KeyValueData<DateTime, IGrouping<DateTime, BillModel>>(g.Key, g);
            RaisePropertyChanged(()=>MonthBillModels);
        }
        #endregion

        #endregion
    }
}
