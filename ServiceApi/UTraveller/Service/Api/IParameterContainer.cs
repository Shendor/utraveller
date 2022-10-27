using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface IParameterContainer<T>
    {
        object this[T key] { get; }

        void AddParameter(T key, object parameter);

    }
}
