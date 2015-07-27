using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.View;

namespace BinLaw.Account.Common
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasePage : Page
    {
        public BasePage()
        {
            this.InitializeComponent();

            if (MainFrame.Content == null)
            {
               
                var version = AppSettingHelper.GetValueOrDefault(Constants.VERISION_FILENAME, string.Empty);
                if (string.IsNullOrEmpty(version) || version !=
                    DeviceHelper.GetCurrentAppVersion())
                {
                    MainFrame.Navigated += this.RootFrame_FirstNavigated;

                    //首次运行或者程序更新
                    if (!MainFrame.Navigate(typeof(InitialFirstPage)))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
                else
                {
                    if (!MainFrame.Navigate(typeof(MainPage)))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                    //SuspensionManager.RegisterFrame(MainFrame, "mainFrame");
                }
                

                MainFrame.RequestedTheme = ElementTheme.Light;
                App.ContentFrame = MainFrame;
            }

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            StatusBar statusbar = StatusBar.GetForCurrentView();
            await statusbar.HideAsync();
        }

        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame == null) return;
            if (rootFrame.ContentTransitions==null)
                rootFrame.ContentTransitions = new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
    }
}
