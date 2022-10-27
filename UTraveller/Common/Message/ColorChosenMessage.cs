using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UTraveller.Common.Message
{
    public class ColorChosenMessage : ObjectMessage<SolidColorBrush>
    {
        public ColorChosenMessage(SolidColorBrush color)
            : base(color)
        {
        }
    }
}
