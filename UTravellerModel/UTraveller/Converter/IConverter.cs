using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Converter
{
    public interface IConverter<T, C>
    {
        C Convert(T origin);
    }
}
