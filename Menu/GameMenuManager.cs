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
    public async Task NoVisionForTime(float duration, System.Action actionAfterFade = null, System.Action actionAfterEnd = null)
    {
        await ChangeAlpha(_noViewPanel, 1f);
        await Task.Delay(Mathf.RoundToInt(1000));
        actionAfterFade?.Invoke();
        await Task.Delay(Mathf.RoundToInt(duration * 1000));
        await ChangeAlpha(_noViewPanel, 0f);
        await Task.Delay(Mathf.RoundToInt(1000));
        actionAfterEnd?.Invoke();
    }

    public async Task DisableNoVision() => await ChangeAlpha(_noViewPanel, 0f);

    public async Task ChangeAlpha(Graphic graphic, float alphaValue, float speed = 1f)
    {
        float step = 0.1f * Mathf.Sign(alphaValue - graphic.color.a);
        float targetAlpha = Mathf.Clamp(alphaValue, 0f, 1f);

        while (Mathf.Abs(graphic.color.a - targetAlpha) > 0.01f)
        {
            float newAlpha = Mathf.Clamp(graphic.color.a + step, 0f, 1f);
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, newAlpha);
            await Task.Delay(Mathf.RoundToInt(10 * speed));
        }
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, targetAlpha); // Фикс на случай неточности
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