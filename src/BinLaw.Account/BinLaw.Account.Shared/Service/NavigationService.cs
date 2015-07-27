using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BinLaw.Account.FE.Foundation.Interface;

namespace BinLaw.Account.Service
{
    public class NavigationService : INavigationService
    {

        public bool CanGoBack
        {
            get { return RootFrame.CanGoBack; }
        }

        public void GoBack()
        {
            RootFrame.GoBack();
        }

        public void GoForward()
        {
            RootFrame.GoForward();
        }

        public bool Navigate<T>(object parameter = null)
        {
            var result = RootFrame.Navigate(typeof(T), parameter);
            return result;
        }

        public bool Navigate(Type source, object parameter = null)
        {
            return RootFrame.Navigate(source, parameter);
        }

        private static Frame _rootFrame;
        private static Frame RootFrame
        {
            get { return _rootFrame ?? (_rootFrame = App.ContentFrame); }
        }


    }
}
