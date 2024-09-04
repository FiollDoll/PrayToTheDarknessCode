using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class main : MonoBehaviour
{
    public Sprite nullSprite;
    private string charsOnString = "ÁаÇĞĔÖŌØŸÚÑÍDsViOlWQcx[]pljt1234567890CvmdSueWsadsczxvbh;qoewqri";

    [SerializeField] private allScripts scripts;

    private string CursedText(int len)
    {
        string totalString = "";
        char[] chars = charsOnString.ToCharArray();

        for (int i = 0; i < len; i++)
            totalString += chars[Random.Range(0, chars.Length)];
        return totalString;
    }

    public void SetCursedText(TextMeshProUGUI text, int len) => StartCoroutine(GenerateCursedText(text, len));
    public void EndCursedText() => StopCoroutine("GenerateCursedText");
    private IEnumerator GenerateCursedText(TextMeshProUGUI text, int len) // Сделать остановку
    {
        while (true)
        {
            text.text = CursedText(len);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
