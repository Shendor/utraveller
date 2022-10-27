using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface ISocialClientAccessToken<T>
    {
        T AccessToken { get; set; }
    }
}
