using System;
using System.Collections.Generic;
using System.Text;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BinLaw.Account.Controls
{
    public class NumericTextBox : TextBox
    {
        //返回键，数字键和小数点
        private readonly VirtualKey[] numeric = new VirtualKey[] { VirtualKey.Back, VirtualKey.Number0, VirtualKey.Number1, VirtualKey.Number2, VirtualKey.Number3, VirtualKey.Number4, VirtualKey.Number5, VirtualKey.Number6, VirtualKey.Number7, VirtualKey.Number8, VirtualKey.Number9, 
            VirtualKey.NumberPad0,VirtualKey.NumberPad1,VirtualKey.NumberPad2,VirtualKey.NumberPad3,VirtualKey.NumberPad4,VirtualKey.NumberPad5,VirtualKey.NumberPad6,VirtualKey.NumberPad7,VirtualKey.NumberPad8,VirtualKey.NumberPad9,VirtualKey.Decimal };

        /// <summary>
        /// 是否存在小数点
        /// </summary>
        private bool isPoint;

        public NumericTextBox()
        {
            this.InputScope = new InputScope();
            this.InputScope.Names.Add(new InputScopeName() { NameValue = InputScopeNameValue.NameOrPhoneNumber });
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            //如果是数字键或者返回键则设置e.Handled = true; 表示事件已经处理
            //注： e.key == 190 wp里的小数点为190，VirtualKey无对应枚举值
            if (Array.IndexOf(numeric, e.Key) == -1 && (int)e.Key != 190)
            {
                e.Handled = true;
            }
            else if (this.Text.Length == 0 && (e.Key == VirtualKey.Decimal || (int)e.Key == 190))
            {
                e.Handled = true;
            }
            else if ((e.Key == VirtualKey.Decimal || (int)e.Key == 190) && !isPoint)
            {
                isPoint = true;
            }
            else if (e.Key == VirtualKey.Back)
            {
                //确保只有1个小数点
                if (this.Text.Length > 1 && this.Text[this.Text.Length - 1] == '.')
                {
                    isPoint = false;
                }
            }
            else if ((e.Key == VirtualKey.Decimal || (int)e.Key == 190) && isPoint)
            {
                //避免2个小数点
                e.Handled = true;
            }
            else if (this.Text.Length == 1 && this.Text == "0" && (e.Key != VirtualKey.Decimal || (int)e.Key != 190))
            {
                //避免重复2个0
                e.Handled = true;
            }
            base.OnKeyDown(e); // important, if not called the back button is not handled
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.Text == "0")
                this.Text = string.Empty;
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
                this.Text = "0";
            base.OnLostFocus(e);
        }
    }

}
