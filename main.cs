using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using LastFramework;

public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }
    [Header("Игровые переменные")] public float karma;
    public int hour, minute;

    [Header("Настройки")] public Npc[] allNpc = new Npc[0];
    public Image noViewPanel;
    public Sprite nullSprite;
    [SerializeField] private Material materialOfSelected;
    [HideInInspector] public bool lockAnyMenu;
    [HideInInspector] public float startCameraSize;

    private List<IMenuable> _allGameMenu = new List<IMenuable>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        startCameraSize = Player.Instance.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens
            .FieldOfView;

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

    public Npc GetNpcByName(string name)
    {
        foreach (Npc npc in allNpc)
        {
            if (npc.nameInWorld == name)
                return npc;
        }

        return null;
    }
    
    /// <summary>
    /// Насильное пермещение кого-либо или чего-либо
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

    public void CameraZoom(float changeSize, bool smoothly = false)
    {
        if (smoothly)
            StartCoroutine(SmoothlyZoom(changeSize));
        else
            Player.Instance.virtualCamera.m_Lens.FieldOfView = Main.Instance.startCameraSize + changeSize;
    }

    private IEnumerator SmoothlyZoom(float changeSize)
    {
        if (changeSize < 0) // Если отрицательное
        {
            for (float i = 0; i > changeSize; i--)
            {
                Player.Instance.virtualCamera.m_Lens.FieldOfView -= 1f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (changeSize > 0)
        {
            for (float i = 0; i < changeSize; i++)
            {
                Player.Instance.virtualCamera.m_Lens.FieldOfView += 1f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
            StartCoroutine(SmoothlyZoom(startCameraSize - Player.Instance.virtualCamera.m_Lens.FieldOfView));
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