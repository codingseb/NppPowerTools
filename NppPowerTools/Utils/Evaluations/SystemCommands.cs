using CodingSeb.ExpressionEvaluator;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public class SystemCommands : IVariableEvaluation
    {
        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.Equals("ipconfig", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = IpConfig();
            }
            else if(e.Name.Equals("ipwifi", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = Regex.Match(IpConfig(),
                    @"Carte\s+réseau\s+sans\s+fil\s+Wi-Fi.*$(\s*(?!Carte)(^(\s*(?<ipv4>A.*IPv4.*\s(?<ip>(\d+\.){3}\d+).*)|.*)$))+",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Groups["ip"].Value;
            }
            else if (e.Name.Equals("ips", StringComparison.OrdinalIgnoreCase))
            {
                e.Value =string.Join("\r\n", Regex.Matches(IpConfig(),
                    @"(?<name>Carte.*?)$(\s*(?!Carte)(^(\s*(?<ipv4>A.*IPv4.*\s(?<ip>(\d+\.){3}\d+).*)|.*)$))+",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Where(match => match.Groups["ip"].Success)
                    .Select(match => match.Groups["name"].Value.Trim() + "\r\n" + match.Groups["ip"].Value));
            }


            return e.HasValue;
        }

        private string IpConfig()
        {
            const string command = "ipconfig /all";

            var procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,

                // Set encoding if command is window command
                StandardOutputEncoding = Encoding.GetEncoding(850)
            };

            Process proc = new Process
            {
                StartInfo = procStartInfo
            };
            proc.Start();

            return proc.StandardOutput.ReadToEnd();
        }
    }
}
