using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using BinLaw.Account.Controls;
using BinLaw.Account.FE.Foundation.Interface;

namespace BinLaw.Account.Service
{
    public class ToastService : IToastService
    {
        public static Popup popup;
        private static ToastPrompt toast;

        /// <summary>
        /// 显示Toast消息
        /// </summary>
        /// <param name="Title">显示的标题</param>
        /// <param name="Content">显示的内容</param>
        public void ShowMessage(string Content)
        {
            if (toast == null)
                toast = new ToastPrompt();
            if (popup == null)
                popup = new Popup() { Width = App.ContentFrame.Width, Height = 30 };

            if (popup.Child == null)
            {
                popup.Child = toast;
            }
            toast.ShowMessage(Content);
            popup.IsOpen = true;
        }
    }
}
