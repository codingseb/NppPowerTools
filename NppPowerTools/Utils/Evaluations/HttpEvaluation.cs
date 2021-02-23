﻿using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class HttpEvaluation : IFunctionEvaluation
    {
        static HttpEvaluation()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

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

            List<object> args = e.EvaluateArgs().ToList();
            string url = e.This?.ToString() ?? args[0].ToString();
            IDictionary<string, object> config = args.Find(a => a is IDictionary<string, object>) as IDictionary<string, object>;
            IDictionary<string, object> header = null;

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
                    if(config != null)
                    {
                        config = new Dictionary<string, object>(config, StringComparer.OrdinalIgnoreCase);

                        if (config.ContainsKey("headers"))
                            header = config["headers"] as IDictionary<string, object>;
                        else if (config.ContainsKey("header"))
                            header = config["header"] as IDictionary<string, object>;
                        else if (config.ContainsKey("head"))
                            header = config["head"] as IDictionary<string, object>;
                        else if (config.ContainsKey("h"))
                            header = config["h"] as IDictionary<string, object>;

                        if(header != null)
                            header.Keys.ToList().ForEach(key => httpRequestMessage.Headers.Add(key, header[key].ToString()));
                    }

                    HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;

                    if (!e.Args.Last().Trim().Equals("f", StringComparison.OrdinalIgnoreCase))
                    {
                        if (response.Content.Headers.ContentType.MediaType.IndexOf("image", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            e.Value = new Bitmap(response.Content.ReadAsStreamAsync().Result);
                        }
                        else
                        {
                            e.Value = (object)response.Content.ReadAsStringAsync().Result;
                        }
                    }
                    else
                    {
                        e.Value = response;
                    }
                }
                catch (Exception exception)
                {
                    e.Value = exception;
                }

                return true;
            }

            return false;
        }
    }
}
