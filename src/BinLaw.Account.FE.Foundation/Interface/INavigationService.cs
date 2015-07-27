using System;

namespace BinLaw.Account.FE.Foundation.Interface
{
    public interface INavigationService
    {
        void GoBack();


        void GoForward();


        bool Navigate<T>(object parameter = null);


        bool Navigate(Type source, object parameter = null);

        bool CanGoBack { get; }
    }
}
