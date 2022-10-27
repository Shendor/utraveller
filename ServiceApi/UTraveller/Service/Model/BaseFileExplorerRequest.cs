using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public abstract class BaseFileExplorerRequest
    {
        public BaseFileExplorerRequest()
        {
            Extensions = new List<FileExtensionType>();
        }


        public IList<FileExtensionType> Extensions
        {
            get;
            set;
        }
    }
}
