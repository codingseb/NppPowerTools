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
        private static readonly Regex iniVariableEvalRegex = new Regex(@"^(ini)(?<string>fromString|fs|s)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
            else if ((e.Name.Equals("gv", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("v", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile.GetValue(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<string>(2));
                    return true;
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile.GetValue(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                    return true;
                }
            }
            else if ((e.Name.Equals("gvw", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("vw", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile2)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile2.GetValueWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<string>(2));
                    return true;
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile2.GetValueWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                    return true;
                }
            }
            else if ((e.Name.Equals("gb", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("b", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile3)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile3.GetBool(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<bool>(2));
                    return true;
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile3.GetBool(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                    return true;
                }
            }
            else if ((e.Name.Equals("gbw", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("bw", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile4)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile4.GetBoolWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<bool>(2));
                    return true;
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile4.GetBoolWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                    return true;
                }
            }

            return false;
        }
    }
}
