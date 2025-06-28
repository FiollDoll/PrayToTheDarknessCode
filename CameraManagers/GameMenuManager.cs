using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using LastFramework;
using DG.Tweening;
using TMPro;

public class GameMenuManager: MonoBehaviour
{
    public static GameMenuManager Instance { get; private set; }
    [HideInInspector] public bool lockAnyMenu;
    private List<IMenuable> _allGameMenu = new List<IMenuable>();
    public GameObject startViewMenu;
    public Image noViewPanel;
    public Sprite nullSprite;

    private void Awake() => Instance = this;
    
    private void Initialize()
    {
        _allGameMenu = FindObjectsOfType<MonoBehaviour>().OfType<IMenuable>().ToList();
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

        if (lockAnyMenu)
            return false;

        return true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Активирует затемнение
    /// </summary>
    /// <param name="time"></param>
    /// <param name="actionAfterFade"></param>
    public void ActivateNoVision(float time, Action actionAfterFade = null, Action actionAfterEnd = null)
    {
        Sequence sequence = DOTween.Sequence();
        Tween fadeAnimation = noViewPanel.DOFade(100f, time / 2).SetEase(Ease.InQuart);
        fadeAnimation.OnComplete(() => { actionAfterFade?.Invoke(); });
        sequence.Append(fadeAnimation);
        sequence.Append(noViewPanel.DOFade(0f, time / 2).SetEase(Ease.OutQuart));
        sequence.OnComplete(() => { actionAfterEnd?.Invoke(); });
        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
    }
    
    public void DisableNoVision() => noViewPanel.DOFade(0f, 2f).SetEase(Ease.OutQuart);
    
    private void Update()
    {
        TextManager.UpdateCursedText();
        if (Input.GetKeyDown(KeyCode.C))
            DevConsole.Instance.ManageDevMenu();
    }
    
    public async Task ViewMenuActivate(string text)
    {
        if (DevConsole.Instance.devMode) return;
        TextMeshProUGUI newText = startViewMenu.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Image newImage = startViewMenu.transform.Find("Panel").GetComponent<Image>();
        newText.text = text;
        newText.color = Color.white;
        newImage.color = Color.black;
        await Task.Delay(3000);
        newImage.DOFade(0f, 1f).SetEase(Ease.OutQuart);
        newText.DOFade(0f, 1f).SetEase(Ease.OutQuart);
    }
}