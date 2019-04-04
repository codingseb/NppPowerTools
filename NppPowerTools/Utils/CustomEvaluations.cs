using CodingSeb.ExpressionEvaluator;
using NppPowerTools.Utils.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NppPowerTools.Utils
{
    internal class CustomEvaluations
    {
        private static readonly Random random = new Random();

        private static readonly List<IVariableEvaluation> variablesEvaluations = new List<IVariableEvaluation>
        {
            new LoremIspumEvaluation(),
            new NppTabEvaluation(),
            new JsonEvaluation(),
            new StringJoinEvaluation(),
        };

        private static readonly List<IFunctionEvaluation> functionsEvaluations = new List<IFunctionEvaluation>
        {
            new LoopEvaluation(),
            new HttpEvaluation(),
            new StringJoinEvaluation(),
        };

        public static string Print { get; set; }

        public static void Evaluator_EvaluateVariable(object sender, VariableEvaluationEventArg e)
        {
            if (e.Name.ToLower().Equals("hw") && e.This == null)
            {
                e.Value = "!!! Hello World !!!";
            }
            else if (e.Name.Equals("random") || e.Name.ToLower().Equals("rand") || e.Name.ToLower().Equals("rnd"))
            {
                e.Value = random;
            }
            else if ((e.Name.ToLower().Equals("hex") || e.Name.ToLower().Equals("hexd")) && e.This is int intHexValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0x")}{intHexValue.ToString("X")}";
            }
            else if ((e.Name.ToLower().Equals("hex") || e.Name.ToLower().Equals("hexd")) && e.This is double doubleHexValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0x")}{((int)doubleHexValue).ToString("X")}";
            }
            else if ((e.Name.ToLower().Equals("bin") || e.Name.ToLower().Equals("bind")) && e.This is int intBinValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0b")}{Convert.ToString(intBinValue, 2)}";
            }
            else if ((e.Name.ToLower().Equals("bin") || e.Name.ToLower().Equals("bind")) && e.This is double doubleBinValue)
            {
                e.Value = $"{(e.Name.ToLower().EndsWith("d") ? string.Empty : "0b")}{Convert.ToString((int)doubleBinValue, 2)}";
            }
            else if (e.Name.Equals("guid"))
            {
                e.Value = Guid.NewGuid().ToString();
            }
            else if (e.Name.Equals("clipboard") || e.Name.ToLower().Equals("cb"))
            {
                e.Value = Clipboard.GetText();
            }
            else
                variablesEvaluations.FirstOrDefault(eval => eval.TryEvaluate(sender, e));
        }

        public static void Evaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
        {
            if(e.Name.ToLower().Equals("print") && e.This == null)
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
            else
                functionsEvaluations.FirstOrDefault(eval => eval.TryEvaluate(sender, e));
        }
    }
}
