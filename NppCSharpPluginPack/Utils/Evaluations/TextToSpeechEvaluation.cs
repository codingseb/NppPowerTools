using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;

namespace NppPowerTools.Utils.Evaluations
{
    public class TextToSpeechEvaluation : IFunctionEvaluation
    {
        public bool TryEvaluate(object sender, FunctionEvaluationEventArg e)
        {
            if (e.Name.Equals("say", StringComparison.OrdinalIgnoreCase))
            {
                string text = e.EvaluateArg(0).ToString();
                var synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();

                if (e.Args.Count > 1)
                {
                    var builder = new PromptBuilder();
                    string voice = e.EvaluateArg(1).ToString();
                    List<CultureInfo> availableCultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures).ToList();
                    CultureInfo cultureInfo = availableCultures.Find(c => c.Name.Equals(voice, StringComparison.OrdinalIgnoreCase));
                    if (cultureInfo != null)
                    {
                        builder.StartVoice(cultureInfo);
                        builder.AppendText(text);
                        builder.EndVoice();
                        synthesizer.Speak(builder);
                        e.Value = text + " said in " + voice;
                    }
                    else
                    {
                        List<InstalledVoice> installedVoices = synthesizer.GetInstalledVoices().ToList();
                        InstalledVoice installedvoice = installedVoices.Find(v => v.VoiceInfo.Gender.ToString().Equals(voice, StringComparison.OrdinalIgnoreCase))
                            ?? installedVoices.Find(v => v.VoiceInfo.Name.Equals(voice, StringComparison.OrdinalIgnoreCase))
                            ?? installedVoices.Find(v => v.VoiceInfo.Name.Replace("Microsoft", "").Replace("Desktop", "").Trim().Equals(voice, StringComparison.OrdinalIgnoreCase));

                        if (installedvoice != null)
                        {
                            synthesizer.SelectVoice(installedvoice.VoiceInfo.Name);
                            synthesizer.Speak(text);
                            e.Value = text + " said with " + voice;
                        }
                        else
                        {
                            synthesizer.Speak(text);
                            e.Value = text + " said";
                        }
                    }
                }
                else
                {
                    synthesizer.Speak(text);
                    e.Value = text + " said";
                }
            }
            else if (e.Name.Equals("voices", StringComparison.OrdinalIgnoreCase))
            {
                var synthesizer = new SpeechSynthesizer();
                e.Value = synthesizer.GetInstalledVoices();
            }
            else if (e.Name.Equals("voicesnames", StringComparison.OrdinalIgnoreCase))
            {
                var synthesizer = new SpeechSynthesizer();
                e.Value = synthesizer.GetInstalledVoices().Select(v => v.VoiceInfo.Name).ToList();
            }
            else if (e.Name.Equals("culturesnames", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = CultureInfo.GetCultures(CultureTypes.NeutralCultures).Select(c => c.Name).ToList();
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
