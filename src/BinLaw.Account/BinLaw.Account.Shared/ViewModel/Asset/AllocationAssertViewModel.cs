using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Extension;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.FE.Model.Common;
using BinLaw.Account.Helper;
using BinLaw.Account.Model;
using BinLaw.Account.Service;
using BinLaw.Account.View.Bill;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace BinLaw.Account.ViewModel.Asset
{
    public class AllocationAssertViewModel : ViewModelBase
    {
        #region [ Data ]
        private readonly INavigationService _navigationService;
        private readonly DataService _dataService;
        private int _billType;

        private DateTime _startDateTime;
        private DateTime _endDateTime;
        private ObservableCollection<BillModel> _billModels;
        private ObservableCollection<KeyValueData<string, double>> _pieDataDetail;
        private ObservableCollection<double> _pieData;
        private double _totalMoney;
        private IEnumerable<KeyValueData<int, IGrouping<int, BillModel>>> _categoryBillModels;

        public int BillType
        {
            get { return _billType; }
            set { Set(() => BillType, ref _billType, value); }
        }

        #region 查询出的账单信息 BillModels
        /// <summary>
        /// 查询出的账单信息
        /// </summary>
        public ObservableCollection<BillModel> BillModels
        {
            get { return _billModels; }
            set { Set(() => BillModels, ref _billModels, value); }
        } 
        #endregion

        #region 柱状图的资产数据 PieDataDetail
        /// <summary>
        /// 柱状图的资产数据
        /// </summary>
        public ObservableCollection<KeyValueData<string, double>> PieDataDetail
        {
            get { return _pieDataDetail; }
            set { Set(() => PieDataDetail, ref _pieDataDetail, value); }
        } 
        #endregion

        #region 饼状图的资产数据 PieData
        /// <summary>
        /// 饼状图的资产数据
        /// </summary>
        public ObservableCollection<double> PieData
        {
            get { return _pieData; }
            set { Set(() => PieData, ref _pieData, value); }

        } 
        #endregion

        #region 查询开始时间 StartDateTime
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime StartDateTime
        {
            get { return _startDateTime; }
            set { Set(() => StartDateTime, ref _startDateTime, value); }
        } 
        #endregion

        #region 查询结束时间 EndDateTime
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime EndDateTime
        {
            get { return _endDateTime; }
            set { Set(() => EndDateTime, ref _endDateTime, value); }
        } 
        #endregion

        #region 按类别分组的账单 CategoryBillModels
        /// <summary>
        /// 按类别分组的账单
        /// </summary>
        public IEnumerable<KeyValueData<int, IGrouping<int, BillModel>>> CategoryBillModels
        {
            get { return _categoryBillModels; }
            set { Set(() => CategoryBillModels, ref _categoryBillModels, value); }
        }
        #endregion

        public double TotalMoney
        {
            get { return _totalMoney; }
            set { Set(() => TotalMoney, ref _totalMoney, value); }
        }
        #endregion

        #region [ Ctor ]

        public AllocationAssertViewModel(INavigationService navigationService, DataService dataService)
        {
            _navigationService = navigationService;
            _dataService = dataService;

            StartDateTime = DateTime.Now.AddMonths(-1).Date;
            EndDateTime = DateTime.Now.Date;

            Messenger.Default.Register<int>(this, Constants.ASSERT_BILLTYPE_TOKEN, m =>
            {
                BillType = m;
            });
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
                        await QueryAssert();
                    }));
            }
        }
        #endregion

        #region 根据时间查询账单 QueryBillCommand
        /// <summary>
        /// 根据时间查询账单
        /// </summary>
        public RelayCommand QueryBillCommand
        {
            get
            {
                return _queryBillCommand = _queryBillCommand ?? (_queryBillCommand = new RelayCommand(async () =>
                {
                    await QueryAssert();
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
        #region 账单和资产查询 QueryAssert
        /// <summary>
        /// 账单和资产查询
        /// </summary>
        /// <returns></returns>
        private async Task QueryAssert()
        {
            var endDate = EndDateTime.AddDays(1).Date;
            var result =
                await
                    _dataService.GetQueryAsync<BillModel>()
                        .Where(a => a.DateTime >= StartDateTime && a.DateTime <= endDate && a.BillType == _billType).OrderByDescending(a => a.DateTime)
                        .ToListAsync();
            BillModels = result.ToObserableCollection();

            PieData = (from data in BillModels
                       orderby data.Money
                       group data by data.Category
                           into g
                           select g.Sum(a => a.Money)).ToObserableCollection();

            PieDataDetail = (from data in BillModels
                             orderby data.Money
                             group data by data.Category
                                 into g
                                 select new KeyValueData<string, double>(BillHelper.CategoryNameConverter(g.Key), g.Sum(a => a.Money))).ToObserableCollection();

            TotalMoney = PieData.Sum();

            CategoryBillModels = from data in BillModels
                             group data by data.Category into g
                             select new KeyValueData<int, IGrouping<int, BillModel>>(g.Key, g);
        } 
        #endregion
        #endregion
    }
}
