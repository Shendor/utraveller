using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.Common.Control
{
    public class NotificationChangedMessage : ObjectMessage<bool>
    {
        public NotificationChangedMessage(bool isOpen)
            : this(isOpen, Microsoft.Phone.Controls.PageOrientation.Portrait)
        {
        }

        public NotificationChangedMessage(bool isOpen, Microsoft.Phone.Controls.PageOrientation orientation)
            : base(isOpen)
        {
            Orientation = orientation;
        }

        public Microsoft.Phone.Controls.PageOrientation Orientation { get; set; }
    }
}
