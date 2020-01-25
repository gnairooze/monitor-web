using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace MonitorWebAppConsole
{
    class WebClient
    {
        public static int Timeout { get; set; }

        const string MESSAGE_ERROR = "!!! Something wrong happened !!!";
        const string MESSAGE_OK = "Everything is OK";
        public static async Task<bool> PostAsync(string url, string mediatype, string payload)
        {
            bool result = false;
            var encoding = Encoding.UTF8;

            HttpContent content = new StringContent(payload, encoding, mediatype);

            Logger.Log($"{DateTime.Now} - POST {url}");
            Logger.Log("Payload");
            Logger.Log(await content.ReadAsStringAsync());

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, Timeout);
                    var response = await client.PostAsync(url, content);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Logger.Log(MESSAGE_ERROR);
                    }
                    else
                    {
                        result = true;
                        Logger.Log(MESSAGE_OK);
                    }

                    Logger.Log(response.StatusCode.ToString());
                    Logger.Log(await response.Content.ReadAsStringAsync());

                    response.Dispose();
                    response = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("!!! Error In Monitoring The Web App !!!");
                Logger.Log(ex.Message);

                ex = ex.InnerException;

                while (ex != null)
                {
                    Logger.Log(ex.Message);
                    Logger.Log(ex.StackTrace);

                    ex = ex.InnerException;
                }
            }

            content.Dispose();
            content = null;
           

            return result;
        }

        public static async Task<bool> GetAsync(string url)
        {
            bool result = false;

            Logger.Log($"{DateTime.Now} - GET {url}");

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, Timeout);
                    var response = await client.GetAsync(url);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Logger.Log(MESSAGE_ERROR);
                        Logger.Log(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        result = true;
                        Logger.Log(MESSAGE_OK);
                    }

                    Logger.Log(response.StatusCode.ToString());

                    response.Dispose();
                    response = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("!!! Error In Monitoring The Web App !!!");
                Logger.Log(ex.Message);
                Logger.Log(ex.StackTrace);

                ex = ex.InnerException;

                while(ex != null)
                {
                    Logger.Log(ex.Message);
                    Logger.Log(ex.StackTrace);

                    ex = ex.InnerException;
                }
            }

            return result;
        }

        public static async Task<bool> ExecuteAsync(string method, string url, string mediatype = null, string payload = null)
        {
            bool isValid = ValidateMethod(method);

            if (!isValid)
            {
                throw new NotSupportedException($"{method} is not supported");
            }

            bool result = false; ;

            switch (method.ToUpper())
            {
                case "POST":
                    result = await PostAsync(url, mediatype, payload);
                    break;
                case "GET":
                    result = await GetAsync(url);
                    break;
            }

            return result;
        }

        private static bool ValidateMethod(string method)
        {
            switch (method.ToUpper())
            {
                case "POST":
                    return true;
                case "GET":
                    return true;
                default:
                    return false;
            }
        }
    }
}
