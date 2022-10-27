using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class NavigationService : INavigationService
    {
        public static readonly string PARAMETER_NAME = "parameter";

        public void Navigate(string uri)
        {
            this.Navigate(uri, "");
        }

        public void Navigate(string uri, string parameter)
        {
            var parameters = new Dictionary<string, string>();
            if (parameter != null && parameter.Length > 0)
            {
                parameters.Add(PARAMETER_NAME, parameter);
            }

            this.Navigate(uri, parameters);
        }

        public void Navigate(string uri, IDictionary<string, string> parameters)
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                frame.Navigate(new Uri(GetUriWithParameters(uri, parameters), UriKind.RelativeOrAbsolute));
            }
        }

        private string GetUriWithParameters(string uri, IDictionary<string, string> parameters)
        {
            StringBuilder uriBuilder = new StringBuilder();
            uriBuilder.Append(uri);
            if (parameters != null && parameters.Count > 0)
            {
                uriBuilder.Append("?");
                bool prependAmp = false;
                foreach (KeyValuePair<string, string> parameterPair in parameters)
                {
                    if (prependAmp)
                    {
                        uriBuilder.Append("&");
                    }
                    uriBuilder.AppendFormat("{0}={1}", parameterPair.Key, parameterPair.Value);
                    prependAmp = true;
                }
            }
            return uriBuilder.ToString();
        }
    }
}
