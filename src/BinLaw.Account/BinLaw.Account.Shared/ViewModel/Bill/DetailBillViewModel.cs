using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.FE.Model.Bill;
using BinLaw.Account.FE.Model.Enum;
using BinLaw.Account.Helper;
using BinLaw.Account.Model;
using BinLaw.Account.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace BinLaw.Account.ViewModel.Bill
{
    public class DetailBillViewModel : ViewModelBase
    {
        #region [ Data ]

        private readonly INavigationService _navigationService;
        private readonly DataService _dataService;
        private readonly IToastService _toastService;
        private readonly ResourceLoader _resourceLoader;

        private bool _isEdit;
        private string _name;
        private DateTime _dateTime;
        private string _note;
        private double _money;
        private string _category;
        private int _billType;
        private BillModel _accountModel;



        #region 是否在编辑模式 IsEdit
        /// <summary>
        /// 是否在编辑模式
        /// </summary>
        public bool IsEdit
        {
            get { return _isEdit; }
            set { Set(() => IsEdit, ref _isEdit, value); }
        }
        #endregion

        #region 账单类型 BillType
        /// <summary>
        /// 账单类型
        /// </summary>
        public int BillType { get { return _billType; } set { Set(() => BillType, ref _billType, value); } }
        #endregion

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

        #region 记账时间 RecordTime
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

        #region 账单主分类 CateGoryItems
        /// <summary>
        /// 账单主分类
        /// </summary>
        public ObservableCollection<BillCategoryItem> CateGoryItems
        {
            get { return BillHelper.CateGoryItems.Result; }
        }
        #endregion

        #region 账单类型 Category
        /// <summary>
        /// 账单类型
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { Set(() => Category, ref _category, value); }
        }
        #endregion

        public BillModel AccountModel
        {
            get { return _accountModel; }
            set { Set(() => AccountModel, ref _accountModel, value); }
        }

        #endregion

        #region [ Ctor ]

        public DetailBillViewModel(INavigationService navigationService, DataService dataService, IToastService toastService)
        {
            _dataService = dataService;
            _toastService = toastService;
            _navigationService = navigationService;
            _resourceLoader = new ResourceLoader();

            Messenger.Default.Register<BillModel>(this, model =>
            {
                if (model == null) return;
                _accountModel = model;
                BillType = model.BillType;
                Name = model.Name;
                DateTime = model.DateTime;
                Note = model.Note;
                Money = model.Money;
                Category = model.Category / 10000 == 1 ? _resourceLoader.GetString("Expanse") : _resourceLoader.GetString("Income");
                Category += (model.Category / 100) % 100 != 0
                    ? "-" + CateGoryItems.FirstOrDefault(a => a.BillType == model.BillType && a.Id == (model.Category % 10000) / 100)
                    .Name : "";
            });
        }


        #endregion

        #region [ Command ]
        private RelayCommand _onPageLoadedCommand;
        private RelayCommand _editAccountCommand;
        private RelayCommand _saveAccountCommand;
        private RelayCommand _cancelEditCommand;
        private RelayCommand _deleteAccountCommand;

        #region 页面加载完毕后执行 OnPageLoadCommand
        /// <summary>
        /// It will execute when page loaded
        /// 当页面载入完毕后执行的事件
        /// </summary>
        public RelayCommand OnPageLoadedCommand
        {
            get
            {
                return _onPageLoadedCommand ?? (_onPageLoadedCommand = new RelayCommand(() =>
                {
                    IsEdit = false;
                }));

            }
        }
        #endregion

        public RelayCommand EditAccountCommand
        {
            get
            {
                return _editAccountCommand ?? (_editAccountCommand = new RelayCommand(() =>
                    {
                        IsEdit = true;
                    }));
            }
        }

        public RelayCommand SaveAccountCommand
        {
            get
            {
                return _saveAccountCommand ?? (_saveAccountCommand = new RelayCommand(async () =>
                    {
                        BillHelper.SyncTotalAccount(_accountModel, Money, DataOperationEnumType.Update);
                        _toastService.ShowMessage(_resourceLoader.GetString("SaveSuccess"));

                        _accountModel.Name = Name;
                        _accountModel.Money = Money;
                        _accountModel.Note = Note;
                        _accountModel.IsSync = false;
                        _accountModel.UpdateTime = DateTime.Now;
                        await _dataService.UpdateAsync(_accountModel);
                        BillHelper.SyncTotalAccount(_accountModel, Money, DataOperationEnumType.Update);
                        IsEdit = false;

                        await _dataService.PushAsync().ConfigureAwait(false);

                    }));
            }
        }

        public RelayCommand CancelEditCommand
        {
            get
            {
                return _cancelEditCommand ?? (_cancelEditCommand = new RelayCommand(() =>
                {
                    IsEdit = false;
                }));
            }
        }

        public RelayCommand DeleteAccountCommand
        {
            get
            {
                return _deleteAccountCommand ?? (_deleteAccountCommand = new RelayCommand(async () =>
                    {
                        var result = await MessageDialogHelper.ShowAsync(_resourceLoader.GetString("DeleteAttention"), _resourceLoader.GetString("Attention"), MessagingButtons.YesNo);
                        if (result == MessagingResult.Yes)
                        {
                            BillHelper.SyncTotalAccount(AccountModel, AccountModel.Money, DataOperationEnumType.Delete);
                            await _dataService.DeleteAsync(AccountModel);
                            _navigationService.GoBack();
                            await _dataService.PushAsync().ConfigureAwait(false);
                        }

                    }));
            }
        }
        #endregion
    }
}
