using Windows.ApplicationModel.Resources;
using BinLaw.Account.FE.Foundation.Interface;
using BinLaw.Account.Service;
using BinLaw.Account.ViewModel.Asset;
using BinLaw.Account.ViewModel.Bill;
using BinLaw.Account.ViewModel.Setting;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace BinLaw.Account.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    /// <author>
    /// BinLaw
    /// </author>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            #region [ Service Register ]
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<DataService>();
            SimpleIoc.Default.Register<IToastService,ToastService>();
            #endregion

            #region [ ViewModel Register ]
            SimpleIoc.Default.Register<InitialFirstViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AppViewModel>();

            SimpleIoc.Default.Register<AddBillViewModel>();
            SimpleIoc.Default.Register<DetailBillViewModel>();
            SimpleIoc.Default.Register<ListBillViewModel>();

            SimpleIoc.Default.Register<AllocationAssertViewModel>();

            SimpleIoc.Default.Register<SettingViewModel>();
            #endregion
        }

        public static AppViewModel AppViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AppViewModel>(); }
        }

        public InitialFirstViewModel InitialFirstViewModel
        {
            get { return ServiceLocator.Current.GetInstance<InitialFirstViewModel>(); }
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public AddBillViewModel AddBillViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddBillViewModel>(); }
        }

        public DetailBillViewModel DetailBillViewModel
        { get { return ServiceLocator.Current.GetInstance<DetailBillViewModel>(); } }

        public ListBillViewModel ListBillViewModel
        { get { return ServiceLocator.Current.GetInstance<ListBillViewModel>(); } }

        public AllocationAssertViewModel AllocationAssertViewModel
        { get { return ServiceLocator.Current.GetInstance<AllocationAssertViewModel>(); } }


        public SettingViewModel SettingViewModel
        { get { return ServiceLocator.Current.GetInstance<SettingViewModel>(); } }

        public static void Cleanup()
        {
            SimpleIoc.Default.Reset();
        }
    }
}
