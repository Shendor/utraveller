using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.Common.Control.ProgressBar
{
    class ProgressBarChangedMessage
    {
        public ProgressBarChangedMessage(bool isOpened, int value, int maxValue)
        {
            IsOpened = isOpened;
            Value = value;
            MaxValue = maxValue;
        }

        public bool IsOpened { get; set; }

        public int Value { get; set; }

        public int MaxValue { get; set; }
    }
}
