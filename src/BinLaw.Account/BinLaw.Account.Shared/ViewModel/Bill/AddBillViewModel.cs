using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.StartScreen;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Extension;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.FE.Model.Bill;
using BinLaw.Account.FE.Model.Enum;
using BinLaw.Account.Helper;
using BinLaw.Account.Model;
using BinLaw.Account.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Windows.Devices.Geolocation;

namespace BinLaw.Account.ViewModel.Bill
{
    public class AddBillViewModel : ViewModelBase
    {
        #region [ Data ]

        private readonly INavigationService _navigationService;
        private readonly DataService _dataService;
        private readonly IToastService _toastService;
        private readonly ResourceLoader _resourceLoader;

        private string _name;
        private DateTime _dateTime;
        private string _note;
        private double _money;
        private BillCategoryItem _selectedCategoryItem;
        private Geolocator _geolocator;
        private Geoposition _geoposition;


        private bool _isPin;
        private bool _isSelectingCategory;

        #region 账单名称 Name
        /// <summary>
        /// 账单名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }
        #endregion

        public ObservableCollection<BillCategoryItem> ExpanseCateGoryItems
        {
            get
            {
                return BillHelper.CateGoryItems.Result.Where(a => a.BillType == (int)BillTypeEnum.Expanse).ToObserableCollection();
            }
        }

        public ObservableCollection<BillCategoryItem> IncomeCateGoryItems
        {
            get
            {
                return BillHelper.CateGoryItems.Result.Where(a => a.BillType == (int)BillTypeEnum.Income).ToObserableCollection();
            }
        }

        #region 当前选中的主账单分类 SelectedCategoryItem
        /// <summary>
        /// 当前选中的主账单分类
        /// </summary>
        public BillCategoryItem SelectedCategoryItem
        {
            get { return _selectedCategoryItem; }
            set
            {
                Set(() => SelectedCategoryItem, ref _selectedCategoryItem, value);
            }
        }
        #endregion

        #region 账单金额 Money
        /// <summary>
        /// 账单金额
        /// </summary>
        public double Money
        {
            get { return _money; }
            set { Set(() => Money, ref _money, value); }
        }
        #endregion

        #region 记账时间 DateTime
        /// <summary>
        /// 记账时间
        /// </summary>
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { Set(() => DateTime, ref _dateTime, value); }
        }
        #endregion

        #region 账单备注 Note
        /// <summary>
        /// 账单备注
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { Set(() => Note, ref _note, value); }
        }
        #endregion

        public Geoposition GeoPosition
        {
            get { return _geoposition; }
            set { Set(() => GeoPosition, ref _geoposition, value); }
        }

        public bool IsPin
        {
            get { return _isPin; }
            set { Set(() => IsPin, ref _isPin, value); }
        }

        public bool IsSelectingCategory
        {
            get { return _isSelectingCategory; }
            set { Set(() => IsSelectingCategory, ref _isSelectingCategory, value); }
        }
        #endregion

        #region [ Ctor ]

        public AddBillViewModel(INavigationService navigationService, DataService dataService, IToastService toastService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
            _toastService = toastService;
            _resourceLoader = new ResourceLoader();
        }

        #endregion

        #region [ Command ]
        private RelayCommand _saveBillCommand;
        private RelayCommand _onPageLoadCommand;
        private RelayCommand _categorySelectedChangedCommand;
        private RelayCommand _pinAddBillTileCommand;
        private RelayCommand _selectingCategoryCommand;
        private RelayCommand<BillCategoryItem> _selectedCategoryCommand;
        private RelayCommand _positioningLocationCommand;

        #region 页面加载完毕后执行 OnPageLoadCommand
        /// <summary>
        /// It will execute when page loaded
        /// 当页面载入完毕后执行的事件
        /// </summary>
        public RelayCommand OnPageLoadCommand
        {
            get
            {
                return _onPageLoadCommand ?? (_onPageLoadCommand = new RelayCommand(() =>
                {
                    CleanUp();
                    IsPin = SecondaryTile.Exists(Constants.TILEKEY_ADDBILL);
                    IsSelectingCategory = false;
                    if (_geolocator == null)
                    {
                        _geolocator = new Geolocator();
                    }


                }));
            }
        }
        #endregion

        #region 保存账单事件 SaveBillCommand
        /// <summary>
        /// 保存账单事件
        /// </summary>
        public RelayCommand SaveBillCommand
        {
            get
            {
                return _saveBillCommand ?? (_saveBillCommand = new RelayCommand(async () =>
                    {
                        if (!ValidData())
                        {
                            await MessageDialogHelper.ShowAsync(_resourceLoader.GetString("AddBillValid"), _resourceLoader.GetString("Attention"), MessagingButtons.OK);
                            return;
                        }

                        BillModel model = new BillModel
                        {
                            Name = Name,
                            BillType = SelectedCategoryItem.BillType,
                            Category = SelectedCategoryItem.CategoryId,
                            DateTime = DateTime,
                            RecordTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            Note = Note,
                            Money = Money,
                            IsRegular = false,
                            IsSync = false,
                            DeviceId = AppSettingHelper.GetValueOrDefault(Constants.APP_DEVICEID, string.Empty),
                        };

                        //判断是否有地理位置信息
                        if (GeoPosition != null)
                        {
                            model.Latitude = GeoPosition.Coordinate.Point.Position.Latitude;
                            model.Longitude = GeoPosition.Coordinate.Point.Position.Longitude;
                        }
                        await _dataService.InsertAsync(model);

                        BillHelper.SyncTotalAccount(model, Money, DataOperationEnumType.Insert);
                        _toastService.ShowMessage(_resourceLoader.GetString("SaveSuccess"));

                        _navigationService.GoBack();
                        CleanUp();
                        await _dataService.PushAsync().ConfigureAwait(false);

                    }));
            }
        }
        #endregion

        public RelayCommand CategorySelectedChangedCommand
        {
            get
            {
                return _categorySelectedChangedCommand ?? (_categorySelectedChangedCommand = new RelayCommand(() =>
                {

                }));
            }
        }

        public RelayCommand PinAddBillTileCommand
        {
            get
            {
                return _pinAddBillTileCommand ?? (_pinAddBillTileCommand = new RelayCommand(async () =>
                    {
                        if (IsPin) return;

                        Uri square150x150Logo = new Uri("ms-appx:///Assets/Logo.scale-100.png");

                        const string tileActivationArguments = Constants.TILEKEY_ADDBILL + "AddBill";

                        SecondaryTile secondaryTile = new SecondaryTile(Constants.TILEKEY_ADDBILL,
                                                                        _resourceLoader.GetString("AppName"),
                                                                        tileActivationArguments,
                                                                        square150x150Logo,
                                                                        TileSize.Square150x150);
                        secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                        await secondaryTile.RequestCreateAsync();
                    }));
            }
        }

        public RelayCommand SelectingCategoryCommand
        {
            get
            {
                return _selectingCategoryCommand ?? (_selectingCategoryCommand = new RelayCommand(() =>
                {
                    IsSelectingCategory = true;
                }
                    ));
            }
        }

        public RelayCommand<BillCategoryItem> SelectedCategoryCommand
        {
            get
            {
                return _selectedCategoryCommand ?? (_selectedCategoryCommand = new RelayCommand<BillCategoryItem>((e) =>
                {
                    IsSelectingCategory = false;
                    var oldName = string.Empty;
                    if (SelectedCategoryItem != null)
                        oldName = SelectedCategoryItem.Name;
                    SelectedCategoryItem = e;
                    if (string.IsNullOrEmpty(Name) || (SelectedCategoryItem != null && Name == oldName))
                        Name = SelectedCategoryItem.Name;

                }));
            }
        }

        public RelayCommand PositioningLocationCommand
        {
            get
            {
                return _positioningLocationCommand ?? (_positioningLocationCommand = new RelayCommand(async () =>
                {
                    try
                    {
                        ViewModelLocator.AppViewModel.IsBusy = true;
                        GeoPosition = await _geolocator.GetGeopositionAsync();
                    }
                    finally
                    {
                        ViewModelLocator.AppViewModel.IsBusy = false;
                    }

                }));
            }
        }
        #endregion

        private void CleanUp()
        {
            DateTime = DateTime.Now;
            Name = string.Empty;
            Money = 0;
            Note = string.Empty;
            SelectedCategoryItem = null;
            GeoPosition = null;
        }

        private bool ValidData()
        {
            if (SelectedCategoryItem == null)
                return false;
            if (string.IsNullOrEmpty(Name))
                return false;
            if (Money == 0)
                return false;
            return true;
        }
    }

}
