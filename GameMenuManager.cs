using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenuManager
{
    public static GameMenuManager Instance { get; private set; }
    public bool LockAnyMenu;
    public Sprite NullSprite;

    private IMenuable[] _allGameMenu = new IMenuable[0];
    private GameObject _startViewMenu;
    private Image _noViewPanel;
    private Image _startViewMenuPanel;
    private TextMeshProUGUI _startViewMenuText;

    public Task Initialize(IMenuable[] allGameMenu, GameObject startView, GameObject noView, Sprite nullSprite)
    {
        Instance = this;
        _allGameMenu = allGameMenu;
        _startViewMenu = startView;
        _noViewPanel = noView.GetComponent<Image>();
        NullSprite = nullSprite;
        _startViewMenuText = _startViewMenu.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _startViewMenuPanel = _startViewMenu.transform.Find("Panel").GetComponent<Image>();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Проверка на возможость открытия любого меню
    /// </summary>
    /// <returns>true - открытие возможно, false - не возможно</returns>
    public bool CanMenuOpen()
    {
        foreach (IMenuable menuable in _allGameMenu)
        {
            if (menuable.menu.gameObject.activeSelf)
                return false;
        }

        if (LockAnyMenu)
            return false;

        return true;
    }

    public async Task ActivateNoVision() => await ChangeAlpha(_noViewPanel, 1f);

    /// <summary>
    /// Активирует временное затемнение
    /// </summary>
    public async void NoVisionForTime(float duration, Task actionAfterFade = null, Task actionAfterEnd = null)
    {
        await ChangeAlpha(_noViewPanel, 1f);
        if (actionAfterFade != null)
            await actionAfterFade;

        await Task.Delay(Mathf.RoundToInt(duration * 1000));

        await ChangeAlpha(_noViewPanel, 0f);
        if (actionAfterEnd != null)
            await actionAfterEnd;
    }

    public async Task DisableNoVision() => await ChangeAlpha(_noViewPanel, 0f);

    public async Task ChangeAlpha(Graphic graphic, float alphaValue, float speed = 1f)
    {
        float step = 0.05f * Mathf.Sign(alphaValue - graphic.color.a);

        while (Mathf.Abs(alphaValue - graphic.color.a) > Mathf.Epsilon)
        {
            float newAlpha = Mathf.Clamp(graphic.color.a + step, 0f, 1f);
            graphic.color = new Color(_noViewPanel.color.r, _noViewPanel.color.g, _noViewPanel.color.b, newAlpha);
            await Task.Delay(Mathf.RoundToInt(1 * speed));
        }
    }

    public async void ViewMenuActivate(string text)
    {
        _startViewMenu.SetActive(true);
        _startViewMenuText.text = text;
        _startViewMenuText.color = Color.white;
        _startViewMenuPanel.color = Color.black;
        await Task.Delay(4000);
        // Так и надо
        _ = ChangeAlpha(_startViewMenuText, 0f, 3f);
        _ = ChangeAlpha(_startViewMenuPanel, 0f, 3f);
        await Task.Delay(2000);
        _startViewMenu.SetActive(false);
    }
}