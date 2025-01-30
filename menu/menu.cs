using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Sprite curseBg;
    [SerializeField] private TextMeshProUGUI textPlay, textPref, textExit;

    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private Camera mainCamera;

    public void Play() => SceneManager.LoadScene("prehistory");
    //public void Preferens() =>
    //public void Exit()

    public void ChoiceLanguage(string language)
    {
        PlayerPrefs.SetString("language", language);
    }

    public void SetCurseStyle()
    {
        bg.sprite = curseBg;
        textPlay.text = "^I%HANH-?:{!ZGJ]W&F@/MW.";
        textPref.text = ",|WZT'VGR-&P";
        textExit.text = "ZKT'W[=O^T@K";
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane; 

        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 backgroundPosition = transform.position;

        backgroundPosition.x = worldMousePosition.x * cameraMoveSpeed; // Ограничиваем по x
        backgroundPosition.y = worldMousePosition.y * cameraMoveSpeed; // Ограничиваем по y

        bg.gameObject.transform.position = backgroundPosition;
    }
}
