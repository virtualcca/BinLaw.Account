using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinLaw.Account.FE.Model.Enum
{
    public enum MessagingButtons
    {
        /// <summary>
        /// Displays only the OK button.
        /// </summary>
        OK = 0,
        /// <summary>
        /// Displays both the OK and Cancel buttons.
        /// </summary>
        OKCancel = 1,
        /// <summary>
        /// Displays both the Yes and No buttons.
        /// </summary>
        YesNo = 2
    }

    public enum MessagingResult
    {
        /// <summary>
        /// This value is not currently used.
        /// </summary>
        None = 0,
        /// <summary>
        /// The user clicked the OK button.
        /// </summary>
        OK = 1,
        /// <summary>
        /// The user clicked the Cancel button.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// The user clicked the Yes button.
        /// </summary>
        Yes = 6,
        /// <summary>
        /// The user clicked the No button.
        /// </summary>
        No = 7
    }


}
