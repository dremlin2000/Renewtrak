using System.Collections.Generic;

namespace Puzzle2
{
    public static class StringExtension
    {
        private static string[] Split(string inputString, char delimiter)
        {
            var result = new List<string>();
            var str = string.Empty;

            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString[i] == delimiter)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        result.Add(str);
                        str = string.Empty;
                    }
                    result.Add(inputString[i].ToString());
                }
                else
                {
                    str += inputString[i];
                }
            }

            if (!string.IsNullOrEmpty(str))
            {
                result.Add(str);
            }

            return result.ToArray();
        }

        private static string ReverseWord(string word)
        {
            var wordChars = new char[word.Length];

            for (int i = 0, j = word.Length - 1; i <= j; i++, j--)
            {
                wordChars[i] = word[j];
                wordChars[j] = word[i];
            }
            return new string(wordChars);
        }

        public static string ReverseWords(this string sentence)
        {
            var result = string.Empty;

            //Split input sentence
            var splitSentence = Split(sentence, ' ');

            //Reverse each word
            for (int i = 0; i < splitSentence.Length; i++)
            {
                result += ReverseWord(splitSentence[i]);
            }

            return result;
        }
    }
}
