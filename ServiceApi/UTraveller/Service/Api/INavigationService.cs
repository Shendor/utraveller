using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface INavigationService
    {
        void Navigate(string uri);

        void Navigate(string uri, string parameter);

        void Navigate(string uri, IDictionary<string, string> parameters);
    }
}
