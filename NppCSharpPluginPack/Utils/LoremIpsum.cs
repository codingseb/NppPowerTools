using System;
using System.Text;

namespace NppPowerTools.Utils
{
    public class LoremIpsum
    {
        private string[] CurrentLanguageWords
        {
            get
            {
                string text = Resources.Latin;
                if (CurrentLanguage.Equals("en"))
                    text = Resources.English;
                else if (CurrentLanguage.Equals("fr"))
                    text = Resources.French;

                return text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string CurrentLanguage { get; set; } = "la";

        public string GetWords(int NumWords, int NumWordsPerLine)
        {
            StringBuilder Result = new();

            Random random = new();

            string[] words = CurrentLanguageWords;

            for (int i = 0; i < NumWords; i++)
            {
                Result.Append((i % NumWordsPerLine) == 0 && i > 0 ? "\r\n" : (i > 0 ? " " : "")).Append(words[random.Next(words.Length - 1)]);
            }

            Result.Append(".");
            return Result.ToString();
        }

        public string GetLines(int NumLines, int NumWordsPerLine)
        {
            StringBuilder Result = new();

            Random random = new();

            string[] words = CurrentLanguageWords;

            for (int i = 0; i < NumLines; i++)
            {
                if (i > 0)
                    Result.Append("\r\n");

                for (int j = 0; j < NumWordsPerLine; j++)
                {
                    Result.Append(j > 0 ? " " : "").Append(words[random.Next(words.Length - 1)]);
                }
            }

            Result.Append(".");
            return Result.ToString();
        }
    }
}
