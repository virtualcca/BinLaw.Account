using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using BinLaw.Account.FE.Model.Enum;

namespace BinLaw.Account.FE.Foundation.Helper
{
    public static class MessageDialogHelper
    {
        /// <summary>
        /// Displays the specified message using platform-specific
        /// user-messaging capabilities.
        /// </summary>
        /// <param name="message">The message to be displayed to the user.</param>
        /// <param name="title">The title of the message.</param>
        /// <returns>A <see cref="T:MessagingResult"/>value representing the users response.</returns>
        public static async Task<MessagingResult> ShowAsync(string message, string title)
        {
            MessagingResult result = await ShowAsync(message, title,
              MessagingButtons.OK);
            return result;
        }
        /// <summary>
        /// Displays the specified message using platform-specific
        /// user-messaging capabilities.
        /// </summary>
        /// <param name="message">The message to be displayed to the user.</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="buttons">The buttons to be displayed.</param>
        /// <exception cref="T:ArgumentException"/>
        /// The specified value for message or title is <c>null</c> or empty.
        /// <exception>
        /// <exception cref="T:NotSupportedException"/>
        /// The specified <see cref="T:MessagingButtons"/> value is not supported.
        /// </exception>
        /// <returns>A <see cref="T:MessagingResult"/>
        /// value representing the users response.</returns>
        public static async Task<MessagingResult> ShowAsync(
          string message, string title, MessagingButtons buttons)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(
                  "The specified message cannot be null or empty.", "message");
            }
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException(
                  "The specified title cannot be null or empty.", "title");
            }
            MessageDialog dialog = new MessageDialog(message, title);
            MessagingResult result = MessagingResult.None;
            switch (buttons)
            {
                case MessagingButtons.OK:
                    dialog.Commands.Add(new UICommand("OK",
                      (o) => result = MessagingResult.OK));
                    break;
                case MessagingButtons.OKCancel:
                    dialog.Commands.Add(new UICommand("OK",
                      (o) => result = MessagingResult.OK));
                    dialog.Commands.Add(new UICommand("Cancel",
                      (o) => result = MessagingResult.Cancel));
                    break;
                case MessagingButtons.YesNo:
                    dialog.Commands.Add(new UICommand("Yes",
                      (o) => result = MessagingResult.Yes));
                    dialog.Commands.Add(new UICommand("No",
                      (o) => result = MessagingResult.No));
                    break;
                default:
                    throw new NotSupportedException(
                      string.Format("MessagingButtons.{0} is not supported.",
                      buttons.ToString()));
            }
            dialog.DefaultCommandIndex = 1;
            // Determine whether the calling thread is the
            // thread associated with the Dispatcher.
            if (Window.Current.Dispatcher.HasThreadAccess)
            {
                await dialog.ShowAsync();
            }
            else
            {
                // Execute asynchronously on the thread the Dispatcher is associated with.
                await Window.Current.Dispatcher.RunAsync(
              CoreDispatcherPriority.Normal, async () =>
              {
                  await dialog.ShowAsync();
              });
            }
            return result;
        }
    }
}
