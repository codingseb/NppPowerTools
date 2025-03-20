using System;
using System.Text.RegularExpressions;

namespace NppPowerTools.Utils.Evaluations
{
    public sealed class IniEvaluation : IFunctionEvaluation, IVariableEvaluation
    {
        private static readonly Regex iniVariableEvalRegex = new("^(ini)(?<string>fromString|fs|s)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            Match iniVariableEvalMatch = iniVariableEvalRegex.Match(e.Name);

            if (iniVariableEvalMatch.Success && e.This == null)
            {
                if (iniVariableEvalMatch.Groups["string"].Success)
                {
                    IniFile inifile = new();

                    inifile.LoadFromString(e.EvaluateArg<string>(0));

                    e.Value = inifile;
                }
                else
                {
                    e.Value = new IniFile(e.EvaluateArg<string>(0));
                }
            }
            else if ((e.Name.Equals("gv", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("v", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile.GetValue(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<string>(2));
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile.GetValue(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                }
            }
            else if ((e.Name.Equals("gvw", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("vw", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile2)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile2.GetValueWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<string>(2));
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile2.GetValueWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                }
            }
            else if ((e.Name.Equals("gb", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("b", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile3)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile3.GetBool(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<bool>(2));
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile3.GetBool(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                }
            }
            else if ((e.Name.Equals("gbw", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("bw", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile4)
            {
                if (e.Args.Count == 3)
                {
                    e.Value = inifile4.GetBoolWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1), e.EvaluateArg<bool>(2));
                }
                if (e.Args.Count == 2)
                {
                    e.Value = inifile4.GetBoolWithoutCreating(e.EvaluateArg<string>(0), e.EvaluateArg<string>(1));
                }
            }
            else if ((e.Name.Equals("sections", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("sec", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("s", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile5)
            {
                e.Value = inifile5.SectionsNames;
            }
            else if ((e.Name.Equals("kfs", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("keys", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("kos", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("ks", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("k", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile6)
            {
                e.Value = inifile6.GetKeysOfSection(e.EvaluateArg<string>(0));
            }

            return e.FunctionReturnedValue;
        }

        public bool TryEvaluate(object sender, VariableEvaluationEventArg e)
        {
            if ((e.Name.Equals("sections", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("sec", StringComparison.OrdinalIgnoreCase)
                || e.Name.Equals("s", StringComparison.OrdinalIgnoreCase)) && e.This is IniFile inifile1)
            {
                e.Value = inifile1.SectionsNames;
            }

            return e.HasValue;
        }

        #region singleton

        private static IniEvaluation instance;

        public static IniEvaluation Instance
        {
            get
            {
                return instance ??= new IniEvaluation();
            }
        }

        private IniEvaluation()
        { }
        #endregion

    }
}
