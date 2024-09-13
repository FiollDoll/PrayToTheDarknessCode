using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class main : MonoBehaviour
{
    public Sprite nullSprite;
    private string charsOnString = "ÁаÇĞĔÖŌØŸÚÑÍDsViOlWQcx[]pljt1234567890CvmdSueWsadsczxvbh;qoewqri";

    [SerializeField] private allScripts scripts;
    private Dictionary<TextMeshProUGUI, Coroutine> cursedTextCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();

    private string CursedText(int len)
    {
        string totalString = "";
        char[] chars = charsOnString.ToCharArray();

        for (int i = 0; i < len; i++)
            totalString += chars[Random.Range(0, chars.Length)];
        return totalString;
    }

    public void SetCursedText(TextMeshProUGUI text, int len) 
    {
        if (cursedTextCoroutines.ContainsKey(text))
        {
            StopCoroutine(cursedTextCoroutines[text]);
            cursedTextCoroutines.Remove(text);
        }
        Coroutine newCoroutine = StartCoroutine(GenerateCursedText(text, len));
        cursedTextCoroutines[text] = newCoroutine;
    }

    public void EndCursedText(TextMeshProUGUI text) 
    {
        if (cursedTextCoroutines.TryGetValue(text, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            cursedTextCoroutines.Remove(text); // Удаляем запись
        }
    }

    private IEnumerator GenerateCursedText(TextMeshProUGUI text, int len) 
    {
        while (true)
        {
            text.text = CursedText(len);
            yield return new WaitForSeconds(0.15f);
        }
    }
}