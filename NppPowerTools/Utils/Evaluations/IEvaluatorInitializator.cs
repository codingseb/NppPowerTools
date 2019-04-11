using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppPowerTools.Utils.Evaluations
{
    public interface IEvaluatorInitializator
    {
        void Init(ExpressionEvaluator evaluator);
    }
}
