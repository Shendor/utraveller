using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Control
{
    public interface IActivateablePage
    {
        void Activate();

        void Deactivate();
    }
}
