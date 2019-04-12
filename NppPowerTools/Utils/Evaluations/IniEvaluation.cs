using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xaml;
using CodingSeb.ExpressionEvaluator;
using Media = System.Windows.Media;

namespace NppPowerTools.Utils.Evaluations
{
    public class IniEvaluation : IFunctionEvaluation
    {
        private static readonly Regex iniVariableEvalRegex = new Regex(@"^(ini)(?<string>fromString|fs)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match iniVariableEvalMatch = iniVariableEvalRegex.Match(e.Name);

            if (iniVariableEvalMatch.Success && e.This == null)
            {
                if (iniVariableEvalMatch.Groups["string"].Success)
                {
                    IniFile inifile = new IniFile();

                    inifile.LoadFromString(e.EvaluateArg<string>(0));

                    e.Value = inifile;
                }
                else
                    e.Value = new IniFile(e.EvaluateArg<string>(0));


                return true;
            }

            return false;
        }
    }
}
