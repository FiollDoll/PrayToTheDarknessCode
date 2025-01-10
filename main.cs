using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour
{
    [Header("Игровые переменные")]
    public float karma;
    public int hour, minute;

    [Header("Настройки")]
    public Image noViewPanel;
    public Sprite nullSprite;
    public bool lockAnyMenu;
    public float startCameraSize;
    [SerializeField] private Material materialOfSelected;
    public NPC[] allNpc = new NPC[0];
    
    private readonly string _charsOnString = "QWERTYUIOP{}ASDFGHJKLZXCVBNM<>/ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБ401";
    private Dictionary<TextMeshProUGUI, Coroutine> _cursedTextCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();
    private AllScripts _scripts;

    public void Start()
    {
        // Точка инициализации всех скриптов по порядку
        _scripts = GetComponent<AllScripts>();

        // Список методов инициализации
        var initializers = new Action[]
        {
            () => _scripts.Initialize(),
            () => _scripts.cutsceneManager.Initialize(),
            () => _scripts.dialogsManager.Initialize(),
            () => _scripts.manageLocation.Initialize(),
            () => _scripts.questsSystem.Initialize(),
            () => _scripts.notebook.Initialize(),
            () => _scripts.interactions.Initialize(),
            () => _scripts.inventoryManager.Initialize(),
            () => _scripts.player.Initialize(),
            () => _scripts.devTool.Initialize(),
            () => _scripts.postProcessingController.Initialize()
        };

        // Выполнение инициализации без перерыва с обработкой ошибок
        foreach (var initializer in initializers)
        {
            try
            {
                initializer();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при инициализации: {ex.Message}");
            }
        }
        
        startCameraSize = _scripts.player.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
        foreach (NPC npc in allNpc)
        {
            GameObject npcObj = GameObject.Find(npc.nameInWorld);
            npc.npcMovement = npcObj?.GetComponent<NPC_movement>();
            npc.animator = npcObj?.GetComponent<Animator>();
        }
    }
    // TODO: прибраться в методах
    
    public void ActivateNoVision(float time)
    {
        Sequence sequence = DOTween.Sequence();
        Tween fadeAnimation = noViewPanel.DOFade(100f, time / 2).SetEase(Ease.InQuart);
        sequence.Append(fadeAnimation);
        sequence.Append(noViewPanel.DOFade(0f, time / 2).SetEase(Ease.OutQuart));
        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
    }

    public void MoveTo(Transform target, float speed, Transform pos, SpriteRenderer spriteRenderer, Animator animator)
    {
        pos.position = Vector3.MoveTowards(pos.position, target.position, speed * Time.deltaTime);
        animator.SetBool("walk", true);
        spriteRenderer.flipX = !(target.position.x > pos.position.x);
    }

    public bool CheckAnyMenuOpen()
    {
        if (_scripts.player.playerMenu.gameObject.activeSelf)
            return true;
        if (_scripts.interactions.floorChangeMenu.activeSelf)
            return true;
        if (_scripts.devTool.menuTools.activeSelf)
            return true;
        if (_scripts.dialogsManager.dialogMenu.activeSelf)
            return true;
        return lockAnyMenu;
    }
    
    /// <summary>
    /// Добавление ДОП материала на объект
    /// </summary>
    /// <param name="renderer"></param>
    /// <returns></returns>
    public void AddMaterial(MeshRenderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length + 1];

        for (int i = 0; i < renderer.materials.Length; i++)
            newMaterials[i] = renderer.materials[i];

        newMaterials[^1] = materialOfSelected;

        renderer.materials = newMaterials;
    }

    /// <summary>
    /// Удаление последнего материала
    /// </summary>
    /// <param name="renderer"></param>
    public void RemoveMaterial(MeshRenderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length - 1];

        int index = 0;
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            if (renderer.materials[i] != materialOfSelected)
            {
                newMaterials[index] = renderer.materials[i];
                index++;
            }

            renderer.materials = newMaterials;
        }
    }
    
    private string CursedText(int len)
    {
        string totalString = "";
        char[] chars = _charsOnString.ToCharArray();

        for (int i = 0; i < len; i++)
        {
            if (Random.Range(0, 2) == 0)
                totalString += char.ToUpper(chars[Random.Range(0, chars.Length)]);
            else
                totalString += char.ToLower(chars[Random.Range(0, chars.Length)]);
        }

        return totalString;
    }

    public void SetCursedText(TextMeshProUGUI text, int len)
    {
        if (_cursedTextCoroutines.ContainsKey(text))
        {
            StopCoroutine(_cursedTextCoroutines[text]);
            _cursedTextCoroutines.Remove(text);
        }

        Coroutine newCoroutine = StartCoroutine(GenerateCursedText(text, len));
        _cursedTextCoroutines[text] = newCoroutine;
    }

    public void EndCursedText(TextMeshProUGUI text)
    {
        if (_cursedTextCoroutines.TryGetValue(text, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            _cursedTextCoroutines.Remove(text);
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