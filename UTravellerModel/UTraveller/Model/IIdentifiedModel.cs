using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public interface IIdentifiedModel
    {
        long Id
        {
            get;
            set;
        }
    }
}
