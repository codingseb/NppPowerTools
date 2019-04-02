using CodingSeb.ExpressionEvaluator;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;

namespace NppPowerTools.Utils
{
    internal class CustomEvaluations
    {
        private static readonly Regex loremIspumVariableEvalRegex = new Regex(@"^(li|loremipsum|lorem|ipsum)(w(?<words>\d+)|wl(?<wordsPerLine>\d+)|(?<language>fr|en|la)|l?(?<lines>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex loopVariableEvalRegex = new Regex(@"^(lp|loop)(f(?<from>\d+)|(t()?<to>\d+)|[nc]?(?<count>\d+))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex tabVarRegex = new Regex(@"tab((?<tabIndex>\d+)|(?<all>all))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Random random = new Random();

        public static string Print { get; set; }

        public static void Evaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
        {
            Match loremIspumVariableEvalMatch = loremIspumVariableEvalRegex.Match(e.Name);
            Match tabVarMatch = tabVarRegex.Match(e.Name);

            if (loremIspumVariableEvalMatch.Success)
            {
                LoremIpsum li = new LoremIpsum();

                if (loremIspumVariableEvalMatch.Groups["language"].Success)
                    li.CurrentLanguage = loremIspumVariableEvalMatch.Groups["language"].Value.ToLower();

                int wordPerLine = loremIspumVariableEvalMatch.Groups["wordsPerLine"].Success ? int.Parse(loremIspumVariableEvalMatch.Groups["wordsPerLine"].Value, CultureInfo.InvariantCulture) : 10;

                if (loremIspumVariableEvalMatch.Groups["words"].Success)
                    e.Value = li.GetWords(int.Parse(loremIspumVariableEvalMatch.Groups["words"].Value, CultureInfo.InvariantCulture), wordPerLine);
                else if (loremIspumVariableEvalMatch.Groups["lines"].Success)
                    e.Value = li.GetLines(int.Parse(loremIspumVariableEvalMatch.Groups["lines"].Value, CultureInfo.InvariantCulture), wordPerLine);
                else
                    e.Value = li.GetWords(100, wordPerLine);
            }
            else if(tabVarMatch.Success)
            {
                string currentTab = BNpp.NotepadPP.CurrentFileName;

                if(tabVarMatch.Groups["all"].Success)
                {
                    List<string> texts = new List<string>();

                    BNpp.NotepadPP.GetAllOpenedDocuments.ForEach(tabName =>
                    {
                        BNpp.NotepadPP.ShowOpenedDocument(tabName);
                        texts.Add(BNpp.Text);
                    });

                    BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                    e.Value = string.Join(BNpp.CurrentEOL, texts);
                }
                else if(tabVarMatch.Groups["tabIndex"].Success)
                {
                    BNpp.NotepadPP.ShowTab(int.Parse(tabVarMatch.Groups["tabIndex"].Value));
                    string text = BNpp.Text;
                    BNpp.NotepadPP.ShowOpenedDocument(currentTab);
                    string doNothingWithItText = BNpp.Text;
                    e.Value = text;
                }

            }
            else if(e.Name.ToLower().Equals("hw") && e.This == null)
            {
                e.Value = "!!! Hello World !!!";
            }
            else if ((e.Name.ToLower().Equals("sjoin") || e.Name.ToLower().Equals("sj") || e.Name.ToLower().Equals("j")) && e.This is IEnumerable enumerableForJoin)
            {
                e.Value = string.Join(BNpp.CurrentEOL, enumerableForJoin.Cast<object>());
            }
            else if(e.Name.ToLower().Equals("json") && e.This != null)
            {
                e.Value = JsonConvert.SerializeObject(e.This, Formatting.Indented); 
            }
            else if(e.Name.Equals("random") || e.Name.ToLower().Equals("rand") || e.Name.ToLower().Equals("rnd"))
            {
                e.Value = random;
            }
            else if((e.Name.ToLower().Equals("hex") || e.Name.ToLower().Equals("hexd")) && e.This is int intHexValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0x")}{intHexValue.ToString("X")}";
            }
            else if((e.Name.ToLower().Equals("hex") || e.Name.ToLower().Equals("hexd")) && e.This is double doubleHexValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0x")}{((int)doubleHexValue).ToString("X")}";
            }
            else if((e.Name.ToLower().Equals("bin") || e.Name.ToLower().Equals("bind")) && e.This is int intBinValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0b")}{Convert.ToString(intBinValue, 2)}";
            }
            else if((e.Name.ToLower().Equals("bin") || e.Name.ToLower().Equals("bind")) && e.This is double doubleBinValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0b")}{Convert.ToString((int)doubleBinValue, 2)}";
            }
            else if(e.Name.Equals("guid"))
            {
                e.Value = Guid.NewGuid().ToString();
            }
            else if (e.Name.Equals("clipboard") || e.Name.ToLower().Equals("cb"))
            {
                e.Value =  Clipboard.GetText();
            }
        }

        public static void Evaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
        {
            Match loopVariableEvalMatch = loopVariableEvalRegex.Match(e.Name);

            if (loopVariableEvalMatch.Success)
            {
                List<object> results = new List<object>();

                int from = loopVariableEvalMatch.Groups["from"].Success ? int.Parse(loopVariableEvalMatch.Groups["from"].Value, CultureInfo.InvariantCulture) : 1;

                if (loopVariableEvalMatch.Groups["to"].Success)
                {
                    for (int i = from; i <= int.Parse(loopVariableEvalMatch.Groups["to"].Value, CultureInfo.InvariantCulture); i++)
                    {
                        e.Evaluator.Variables[e.Args.Count > 1 ? e.Args[1].Trim() : "i"] = i;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]).ToString());
                    }
                }
                else
                {
                    int count = loopVariableEvalMatch.Groups["count"].Success ? int.Parse(loopVariableEvalMatch.Groups["count"].Value, CultureInfo.InvariantCulture) : 10;

                    for (int i = 0; i < count; i++)
                    {
                        e.Evaluator.Variables[e.Args.Count > 1 ? e.Args[1].Trim() : "i"] = i + from;
                        results.Add(e.Evaluator.Evaluate(e.Args[0]));
                    }
                }

                e.Value = results;
            }
            else if ((e.Name.ToLower().Equals("sjoin") || e.Name.ToLower().Equals("sj") || e.Name.ToLower().Equals("j")) && (e.This is IEnumerable<object> || e.Args.Count > 1 && e.EvaluateArg(1) is List<object>))
            {
                if (e.This is List<object> list)
                {
                    e.Value = string.Join(e.Args.Count > 0 ? e.EvaluateArg<string>(0) : "\r\n", list);
                }
                else if (e.Args.Count > 1 && e.EvaluateArg(1) is List<object> list2)
                {
                    e.Value = string.Join(e.EvaluateArg<string>(0), list2);
                }
            }
            else if(e.Name.ToLower().Equals("print") && e.This == null)
            {
                if (e.Args.Count > 1)
                    Print += string.Format(e.EvaluateArg(0).ToString(), e.Args.Skip(1).ToArray()) + BNpp.CurrentEOL;
                else
                    Print += e.EvaluateArg(0).ToString() + BNpp.CurrentEOL;

                e.Value = null;
            }
            else if(e.Name.ToLower().Equals("range") && e.This == null)
            {
                if(e.Args.Count == 2)
                    e.Value = Enumerable.Range((int)e.EvaluateArg(0), (int)e.EvaluateArg(1)).Cast<object>();
                else if(e.Args.Count == 1)
                    e.Value = Enumerable.Range(1, (int)e.EvaluateArg(1)).Cast<object>();
            }
            else if(e.Name.ToLower().Equals("repeat"))
            {
                if(e.Args.Count == 2 && e.This == null)
                    e.Value = Enumerable.Repeat(e.EvaluateArg(0), (int)e.EvaluateArg(1)).Cast<object>();
                else if(e.Args.Count == 1 && e.This != null)
                    e.Value = Enumerable.Repeat(e.This, (int)e.EvaluateArg(1)).Cast<object>();
            }
            else if(e.Name.ToLower().StartsWith("http") && (e.This != null || e.Args.Count > 0))
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
}
