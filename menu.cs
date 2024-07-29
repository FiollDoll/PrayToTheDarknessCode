using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Sprite curseBg;
    [SerializeField] private TextMeshProUGUI textPlay, textPref, textExit;

    public void Play() => SceneManager.LoadScene("step1");
    //public void Preferens() =>
    //public void Exit()

    public void SetCurseStyle()
    {
        bg.sprite = curseBg;
        textPlay.text = "^I%HANH-?:{!ZGJ]W&F@/MW.";
        textPref.text = ",|WZT'VGR-&P";
        textExit.text = "ZKT'W[=O^T@K";
    }
}
