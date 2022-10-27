using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class ParameterContainer : IParameterContainer<string>
    {
        private IDictionary<string, object> parameters;

        public ParameterContainer()
        {
            parameters = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get
            {
                object value = null;
                if (parameters.TryGetValue(key, out value))
                {
                    parameters.Remove(key);
                }
                return value;
            }
        }


        public void AddParameter(string key, object parameter)
        {
            if (parameters.ContainsKey(key))
            {
                parameters.Remove(key);
            }
            parameters.Add(key, parameter);
        }
    }
}
