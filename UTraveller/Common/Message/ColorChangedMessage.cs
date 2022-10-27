using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UTraveller.Common.Message
{
    public class ColorChangedMessage : ObjectMessage<Color>
    {
        public ColorChangedMessage(object token, Color color)
            : base(color)
        {
            Token = token;
        }


        public object Token
        {
            get;
            private set;
        }
    }
}
