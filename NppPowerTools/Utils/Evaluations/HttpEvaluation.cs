using CodingSeb.ExpressionEvaluator;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace NppPowerTools.Utils.Evaluations
{
    public class HttpEvaluation : IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (!(e.Name.StartsWith("http", StringComparison.OrdinalIgnoreCase) && (e.This != null || e.Args.Count > 0)))
                return false;

            HttpClientHandler httpClientHandler = new HttpClientHandler();

            if (Config.Instance.UseProxy)
            {
                httpClientHandler.Proxy = Config.Instance.UseDefaultProxy
                    ? WebRequest.GetSystemWebProxy()
                    : new WebProxy(Config.Instance.ProxyPort == null ? Config.Instance.ProxyAddress : $"{Config.Instance.ProxyAddress}:{Config.Instance.ProxyPort}",
                        Config.Instance.ProxyBypassOnLocal)
                    {
                        BypassList = Config.Instance.ProxyBypassList.Split(';'),
                        UseDefaultCredentials = Config.Instance.UseDefaultCredentials
                    };

                if (!Config.Instance.UseDefaultCredentials && !string.IsNullOrEmpty(Config.Instance.ProxyUserName))
                {
                    string[] UserAndDomains = Config.Instance.ProxyUserName.Split('\\');
                    httpClientHandler.Proxy.Credentials = UserAndDomains.Length > 1
                        ? new NetworkCredential(UserAndDomains.Last(), Config.Instance.ProxyPassword, UserAndDomains[0])
                        : new NetworkCredential(Config.Instance.ProxyUserName, Config.Instance.ProxyPassword);
                }
            }

            var client = new HttpClient(httpClientHandler);

            HttpRequestMessage httpRequestMessage = null;

            string url = e.This?.ToString() ?? e.EvaluateArg(0).ToString();

            if (e.Name.Equals("httppost", StringComparison.OrdinalIgnoreCase))
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            }
            else if (e.Name.Equals("httpput", StringComparison.OrdinalIgnoreCase))
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            }
            else if (e.Name.Equals("httpoptions", StringComparison.OrdinalIgnoreCase))
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Options, url);
            }
            else if (e.Name.Equals("httphead", StringComparison.OrdinalIgnoreCase))
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, url);
            }
            else if (e.Name.Equals("httpdelete", StringComparison.OrdinalIgnoreCase))
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
            }
            else if (e.Name.Equals("httptrace", StringComparison.OrdinalIgnoreCase))
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Trace, url);
            }
            else if (e.Name.Equals("http", StringComparison.OrdinalIgnoreCase) || e.Name.Equals("httpget", StringComparison.OrdinalIgnoreCase))
            {
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            }

            if (httpRequestMessage != null)
            {
                try
                {
                    e.Value = e.Args.Last().Trim().Equals("f", StringComparison.OrdinalIgnoreCase)
                        ? client.SendAsync(httpRequestMessage).Result
                        : (object)client.SendAsync(httpRequestMessage).Result.Content.ReadAsStringAsync().Result;
                }
                catch (Exception exception)
                {
                    e.Value = exception;
                }
            }

            return true;
        }
    }
}
