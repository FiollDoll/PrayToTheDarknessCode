using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;
using LastFramework;

public class Main : MonoBehaviour
{
    [Header("Игровые переменные")] public float karma;
    public int hour, minute;

    [Header("Настройки")] public Npc[] allNpc = new Npc[0];
    public Image noViewPanel;
    public Sprite nullSprite;
    [SerializeField] private Material materialOfSelected;
    [HideInInspector] public bool lockAnyMenu;
    [HideInInspector] public float startCameraSize;

    private List<IMenuable> _allGameMenu = new List<IMenuable>();
    private AllScripts _scripts;

    public void Start()
    {
        _scripts = GetComponent<AllScripts>();

        startCameraSize = _scripts.player.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens
            .OrthographicSize;

        // Инициализация информации об НПС
        foreach (Npc npc in allNpc)
        {
            npc.UpdateNpcStyleDict();
            GameObject npcObj = GameObject.Find(npc.nameInWorld);
            npc.NpcController = npcObj.GetComponent<IHumanable>();
            if (npc.NpcController != null) // Если существо
                npc.NpcController.npcEntity = npc;

            npc.animator = npcObj?.GetComponent<Animator>();
        }

        _allGameMenu = FindObjectsOfType<MonoBehaviour>().OfType<IMenuable>().ToList();
    }

    /// <summary>
    /// Перемещение КОГО-ЛИБО
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    /// <param name="pos"></param>
    /// <param name="spriteRenderer"></param>
    /// <param name="animator"></param>
    public void MoveTo(Transform target, float speed, Transform pos, SpriteRenderer spriteRenderer, Animator animator)
    {
        pos.position = Vector3.MoveTowards(pos.position, target.position, speed * Time.deltaTime);
        animator.SetBool("walk", true);
        spriteRenderer.flipX = !(target.position.x > pos.position.x);
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

    /// <summary>
    /// Проверка на возможость открытия любого меню
    /// </summary>
    /// <returns>true - открытие возможно, false - не возможно</returns>
    public bool CheckAnyMenuOpen()
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

    /// <summary>
    /// Добавление ДОП материала на объект
    /// </summary>
    /// <param name="objRenderer"></param>
    /// <returns></returns>
    public void AddMaterial(MeshRenderer objRenderer)
    {
        Material[] newMaterials = new Material[objRenderer.materials.Length + 1];

        for (int i = 0; i < objRenderer.materials.Length; i++)
            newMaterials[i] = objRenderer.materials[i];

        newMaterials[^1] = materialOfSelected;

        objRenderer.materials = newMaterials;
    }

    /// <summary>
    /// Удаление последнего материала
    /// </summary>
    /// <param name="objRenderer"></param>
    public void RemoveMaterial(MeshRenderer objRenderer)
    {
        Material[] newMaterials = new Material[objRenderer.materials.Length - 1];

        int index = 0;
        for (int i = 0; i < objRenderer.materials.Length; i++)
        {
            if (objRenderer.materials[i] != materialOfSelected)
            {
                newMaterials[index] = objRenderer.materials[i];
                index++;
            }

            objRenderer.materials = newMaterials;
        }
    }

    private void Update()
    {
        TextManager.UpdateCursedText();
    }
}