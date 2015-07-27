using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BinLaw.Account.ViewModel;

namespace BinLaw.Account.Helper
{
    public static class DoingHelper
    {
        public static void BusyAction(Action action)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                action.Invoke();
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static void BusyAction<T>(Action<T> action, T t1)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                action.Invoke(t1);
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static void BusyAction<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                action.Invoke(t1, t2);
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static void BusyAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                action.Invoke(t1, t2, t3);
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static TResult BusyFunc<TResult>(Func<TResult> action)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                return action.Invoke();
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static TResult BusyFunc<T1, TResult>(Func<T1, TResult> action, T1 t1)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                return action.Invoke(t1);
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static TResult BusyFunc<T1, T2, TResult>(Func<T1, T2, TResult> action, T1 t1, T2 t2)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                return action.Invoke(t1, t2);
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static TResult BusyFunc<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> action, T1 t1, T2 t2, T3 t3)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                return action.Invoke(t1, t2, t3);
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }


        public static async Task BusyActionAsync(Task<Action> action)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                await action;
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }

        public static async Task BusyActionAsync<T1>(Task<Action<T1>> action,T1 t1)
        {
            try
            {
                ViewModelLocator.AppViewModel.IsBusy = true;
                await action;
            }
            finally
            {
                ViewModelLocator.AppViewModel.IsBusy = false;
            }
        }
    }
}
