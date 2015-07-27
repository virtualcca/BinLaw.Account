using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace BinLaw.Account.ViewModel
{
    public class AppViewModel:ViewModelBase
    {
        private bool _isBusy;
        private bool _isWaiting;

        private bool _isLock;

        public bool IsBusy { get { return _isBusy; } set { Set(() => IsBusy, ref _isBusy, value); } }

        public bool IsWaiting { get { return _isWaiting; } set { Set(() => IsWaiting, ref _isWaiting, value); } }

        public bool IsLock { get { return _isLock; } set { Set(() => IsLock, ref _isLock, value); } }
    }
}
