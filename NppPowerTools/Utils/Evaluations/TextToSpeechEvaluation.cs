using CodingSeb.ExpressionEvaluator;
using System;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;

namespace NppPowerTools.Utils.Evaluations
{
    public class TextToSpeechEvaluation : IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if(e.Name.Equals("say", StringComparison.OrdinalIgnoreCase))
            {
                string text = e.EvaluateArg(0).ToString();
                var synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.Speak(text);
                e.Value = text + " said";
            }
            else if(e.Name.Equals("voices", StringComparison.OrdinalIgnoreCase))
            {
                var synthesizer = new SpeechSynthesizer();
                e.Value = synthesizer.GetInstalledVoices();
            }
            else if(e.Name.Equals("voicesnames", StringComparison.OrdinalIgnoreCase))
            {
                var synthesizer = new SpeechSynthesizer();
                e.Value = synthesizer.GetInstalledVoices().Select(v => v.VoiceInfo.Name).ToList();
            }

            return e.FunctionReturnedValue;
        }

        #region Singleton          

        private static TextToSpeechEvaluation instance;

        public static TextToSpeechEvaluation Instance => instance ??= new TextToSpeechEvaluation();

        private TextToSpeechEvaluation()
        { }

        #endregion Singleton

    }
}
