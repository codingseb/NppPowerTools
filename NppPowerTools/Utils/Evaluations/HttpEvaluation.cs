using CodingSeb.ExpressionEvaluator;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace NppPowerTools.Utils.Evaluations
{
    public class HttpEvaluation : IFunctionEvaluation
    {
        public bool CanEvaluate(object sender, FunctionEvaluationEventArg e) => e.Name.ToLower().StartsWith("http") && (e.This != null || e.Args.Count > 0);

        public void Evaluate(object sender, FunctionEvaluationEventArg e)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();

            if (Config.Instance.UseProxy)
            {
                if (Config.Instance.UseDefaultProxy)
                    httpClientHandler.Proxy = WebRequest.GetSystemWebProxy();
                else
                {
                    httpClientHandler.Proxy = new WebProxy(Config.Instance.ProxyPort == null ? Config.Instance.ProxyAddress : $"{Config.Instance.ProxyAddress}:{Config.Instance.ProxyPort}",
                        Config.Instance.ProxyBypassOnLocal)
                    {
                        BypassList = Config.Instance.ProxyBypassList.Split(';'),
                        UseDefaultCredentials = Config.Instance.UseDefaultCredentials
                    };
                }

                if (!Config.Instance.UseDefaultCredentials && !string.IsNullOrEmpty(Config.Instance.ProxyUserName))
                {
                    string[] UserAndDomains = Config.Instance.ProxyUserName.Split('\\');
                    if (UserAndDomains.Length > 1)
                        httpClientHandler.Proxy.Credentials = new NetworkCredential(UserAndDomains.Last(), Config.Instance.ProxyPassword, UserAndDomains[0]);
                    else
                        httpClientHandler.Proxy.Credentials = new NetworkCredential(Config.Instance.ProxyUserName, Config.Instance.ProxyPassword);
                }
            }

            var client = new HttpClient(httpClientHandler);

            HttpRequestMessage httpRequestMessage = null;

            string url = e.This?.ToString() ?? e.EvaluateArg(0).ToString();

            if (e.Name.ToLower().Equals("httppost"))
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            else if (e.Name.ToLower().Equals("httpput"))
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            else if (e.Name.ToLower().Equals("httpoptions"))
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Options, url);
            else if (e.Name.ToLower().Equals("httphead"))
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, url);
            else if (e.Name.ToLower().Equals("httpdelete"))
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
            else if (e.Name.ToLower().Equals("httptrace"))
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Trace, url);
            else if (e.Name.ToLower().Equals("http") || e.Name.ToLower().Equals("httpget"))
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            if (httpRequestMessage != null)
            {
                try
                {
                    if (e.Args.Last().Trim().ToLower().Equals("f"))
                        e.Value = client.SendAsync(httpRequestMessage).Result;
                    else
                        e.Value = client.SendAsync(httpRequestMessage).Result.Content.ReadAsStringAsync().Result;
                }
                catch (Exception exception)
                {
                    e.Value = exception;
                }
            }
        }
    }

}
