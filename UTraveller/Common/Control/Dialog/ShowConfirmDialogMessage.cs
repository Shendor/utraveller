using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Control.Dialog
{
    public class ShowConfirmDialogMessage
    {
        public ShowConfirmDialogMessage(bool isVisible)
        {
            IsVisible = isVisible;
        }


        public bool IsVisible
        {
            get;
            set;
        }
    }
}
