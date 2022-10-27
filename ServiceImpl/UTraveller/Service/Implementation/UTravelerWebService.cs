using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ServiceApi.UTraveller.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Implementation
{
    public class UTravelerWebService : IWebService
    {
        private INetworkConnectionCheckService networkCheckService;

        public UTravelerWebService(INetworkConnectionCheckService networkCheckService)
        {
            this.networkCheckService = networkCheckService;
        }

        public async Task<T> GetAsyncWithException<T>(string url)
        {
            T result = default(T);
            if (networkCheckService.HasConnection)
            {
                HttpClient client = CreateHttpClient();
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
                {
                    NoCache = true
                };
                client.DefaultRequestHeaders.IfModifiedSince = new DateTimeOffset(DateTime.UtcNow);

                var responseJson = await client.GetStringAsync(url);
                result = JsonConvert.DeserializeObject<T>(responseJson, new JSONCustomDateConverter());
            }
            return result;
        }


        public async Task<T> GetAsync<T>(string url)
        {
            T result = default(T);
            try
            {
                result = await GetAsyncWithException<T>(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot make GET request: " + ex.Message);
            }
            return result;
        }


        public async Task<Response> PostAsync<Request, Response>(string url, Request postData)
        {
            Response result = default(Response);
            if (networkCheckService.HasConnection)
            {
                HttpClient client = CreateHttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(postData,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                try
                {
                    var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<Response>(responseJson);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Cannot make POST request: " + ex.Message);
                }
            }
            return result;
        }


        public async Task<T> PostAsync<T>(string url)
        {
            T result = default(T);
            if (networkCheckService.HasConnection)
            {
                HttpClient client = CreateHttpClient();
                try
                {
                    var response = await client.PostAsync(url, new StringContent(string.Empty));
                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<T>(responseJson);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Cannot make POST request: " + ex.Message);
                }
            }
            return result;
        }


        private T HandleWebRequestResult<T>(string result, AsyncCompletedEventArgs e)
        {
            T responseJson = default(T);
            if (e.Error == null && !e.Cancelled)
            {
                responseJson = JsonConvert.DeserializeObject<T>(result);
            }
            return responseJson;
        }


        private static HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
            client.Timeout = TimeSpan.FromSeconds(20);
            return client;
        }
    }


    class JSONCustomDateConverter : DateTimeConverterBase
    {
        public override bool CanConvert(Type objectType)
        {
            var dateTimeType = typeof(DateTime);
            return objectType == dateTimeType || Nullable.GetUnderlyingType(objectType) == dateTimeType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
            {
                return new DateTime(1970, 1, 1).AddMilliseconds((long)reader.Value);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToDateTime(value).ToString());
            writer.Flush();
        }
    }
}
