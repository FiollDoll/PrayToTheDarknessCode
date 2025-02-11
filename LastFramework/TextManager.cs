using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LastFramework
{
    public static class TextManager
    {
        private const string CharsOnString = "qwertyuiop[]asdfghjkl;zxcvbnm12345678йцукенгшщзфывапролджжэхъячсмитьбю.";
        private static List<TextMeshProUGUI> _cursedTextCoroutines = new List<TextMeshProUGUI>();

        #region CursedText

        private static string CursedText(int len)
        {
            string totalString = "";
            char[] chars = CharsOnString.ToCharArray();

            for (int i = 0; i < len; i++)
            {
                if (Random.Range(0, 2) == 0)
                    totalString += char.ToUpper(chars[Random.Range(0, chars.Length)]);
                else
                    totalString += char.ToLower(chars[Random.Range(0, chars.Length)]);
            }

            return totalString;
        }

        public static void SetCursedText(TextMeshProUGUI text, int len)
        {
            EndCursedText(text); // Если такой уже есть - удаляем

            _cursedTextCoroutines.Add(text);
        }

        public static void EndCursedText(TextMeshProUGUI text)
        {
            if (_cursedTextCoroutines.Contains(text))
                _cursedTextCoroutines.Remove(text);
        }

        public static void EndAllCursedText()
        {
            foreach (TextMeshProUGUI text in _cursedTextCoroutines)
                _cursedTextCoroutines.Remove(text);
        }

        public static void UpdateCursedText()
        {
            foreach (TextMeshProUGUI text in _cursedTextCoroutines)
                text.text = CursedText(Random.Range(3, 60)); // БОЛЬШЕ ХАОТИЧНОСТИ
        }

        #endregion
    }
}