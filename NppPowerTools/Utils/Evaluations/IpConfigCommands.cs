
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class IpConfigCommands : IVariableEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.Equals("ipconfig", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = IpConfig();
            }
            else if (e.Name.Equals("ips", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = string.Join("\r\n", Regex.Matches(IpConfig(),
                    @"(?<name>Carte.*?)$(\s*(?!Carte)(^(\s*(?<ipv4>A.*IPv4.*\s(?<ip>(\d+\.){3}\d+).*)|.*)$))+",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Where(match => match.Groups["ip"].Success)
                    .Select(match => match.Groups["name"].Value.Trim() + "\r\n" + match.Groups["ip"].Value));
            }
            else if (e.Name.Equals("jips", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = string.Join("\r\n", Regex.Matches(IpConfig(),
                    @"(?<name>Carte.*?)$(\s*(?!Carte)(^(\s*(?<ipv4>A.*IPv4.*\s(?<ip>(\d+\.){3}\d+).*)|.*)$))+",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Where(match => match.Groups["ip"].Success)
                    .Select(match => match.Groups["ip"].Value));
            }
            else if(e.Name.StartsWith("ip_", StringComparison.OrdinalIgnoreCase))
            {
                string find = Regex.Replace(e.Name.Substring(3), ".", "(.*?)$&");

                e.Value = string.Join("\r\n", Regex.Matches(IpConfig(),
                    @"(?<name>Carte\s+find.*?)$(\s*(?!Carte)(^(\s*(?<ipv4>A.*IPv4.*\s(?<ip>(\d+\.){3}\d+).*)|.*)$))+".Replace("find", find),
                    RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Where(match => match.Groups["ip"].Success)
                    .Select(match => match.Groups["name"].Value.Trim() + "\r\n" + match.Groups["ip"].Value));
            }
            else if(e.Name.StartsWith("jip_", StringComparison.OrdinalIgnoreCase))
            {
                string find = Regex.Replace(e.Name.Substring(4), ".", "(.*?)$&");

                e.Value = string.Join("\r\n", Regex.Matches(IpConfig(),
                    @"(?<name>Carte\s+find.*?)$(\s*(?!Carte)(^(\s*(?<ipv4>A.*IPv4.*\s(?<ip>(\d+\.){3}\d+).*)|.*)$))+".Replace("find", find),
                    RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Where(match => match.Groups["ip"].Success)
                    .Select(match => match.Groups["ip"].Value));
            }
            else if(e.Name.StartsWith("aip_", StringComparison.OrdinalIgnoreCase))
            {
                string find = Regex.Replace(e.Name.Substring(4), ".", "(.*?)$&");

                e.Value = string.Join("\r\n", Regex.Matches(IpConfig(),
                    @"(?<name>Carte\s+find.*?)$(\s*(?!Carte)(^(\s*(?<ipv4>A.*IPv4.*\s(?<ip>(\d+\.){3}\d+).*)|.*)$))+".Replace("find", find),
                    RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Where(match => match.Groups["ip"].Success)
                    .Select(match => match.Value));
            }

            return e.HasValue;
        }

        private string IpConfig()
        {
            const string command = "ipconfig /all";

            ProcessStartInfo procStartInfo = new("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,

                // Set encoding if command is window command
                StandardOutputEncoding = Encoding.GetEncoding(850)
            };

            Process proc = new()
            {
                StartInfo = procStartInfo
            };
            proc.Start();

            return proc.StandardOutput.ReadToEnd();
        }
    }
}
