using System;
using BinLaw.Account.Common;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Common;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.View;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227
using BinLaw.Account.ViewModel;
using GalaSoft.MvvmLight.Threading;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using BinLaw.Account.Service;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using SuspensionManager = BinLaw.Account.FE.Foundation.Common.SuspensionManager;
using SuspensionManagerException = BinLaw.Account.FE.Foundation.Common.SuspensionManagerException;

namespace BinLaw.Account
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {

        //// This MobileServiceClient has been configured to communicate with your local
        //// test project for debugging purposes.
        //public static MobileServiceClient MobileService = new MobileServiceClient(
        //    "http://localhost:50526");
        // This MobileServiceClient has been configured to communicate with your Mobile Service's url
        // and application key. You're all set to start working with your Mobile Service!
        public static MobileServiceClient MobileService = new MobileServiceClient(
            AppConfig.BACKEND_URL,
            AppConfig.APPLICATION_KEY
        );

        public static Frame ContentFrame { get; set; }

#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await MessageDialogHelper.ShowAsync(e.Exception.StackTrace.ToString(), "error");
        }



        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 6;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                //if (rootFrame.ContentTransitions != null)
                //{
                //    this.transitions = new TransitionCollection();
                //    foreach (var c in rootFrame.ContentTransitions)
                //    {
                //        this.transitions.Add(c);
                //    }
                //}

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                #region Add By BinLaw
                DispatcherHelper.Initialize();

                var version = AppSettingHelper.GetValueOrDefault(Constants.VERISION_FILENAME, string.Empty);
                if (string.IsNullOrEmpty(version) || version !=
                   DeviceHelper.GetCurrentAppVersion())
                {
                    rootFrame.Navigated += this.RootFrame_FirstNavigated;

                    //首次运行或者程序更新
                    if (!rootFrame.Navigate(typeof(InitialFirstPage)))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
                else
                {
                    if (!rootFrame.Navigate(typeof(BasePage)))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                    //SuspensionManager.RegisterFrame(MainFrame, "mainFrame");
                }

#if WINDOWS_PHONE_APP
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += OnBackPressed;

#endif

                #endregion
            }

            // Check Lock, If locked, Active lock screen 
            if (AppSettingHelper.GetValueOrDefault(Constants.APP_PASSWORD, string.Empty) != string.Empty)
            {
                ViewModelLocator.AppViewModel.IsLock = true;
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        private void OnBackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            var rootframe = Window.Current.Content as Frame;
            if (rootframe == null || !rootframe.CanGoBack)
            {
                this.Exit();
                return;
            }
            else
            {
                rootframe.GoBack();
            }

        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame == null) return;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

#if WINDOWS_PHONE_APP
            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                App.MobileService.LoginComplete(args as WebAuthenticationBrokerContinuationEventArgs);
            }
#endif
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }


    }
}