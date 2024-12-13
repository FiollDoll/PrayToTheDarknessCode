using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class Main : MonoBehaviour
{
    public int hour, minute;
    [SerializeField] private Image noViewPanel;
    public Sprite nullSprite;
    private string _charsOnString = "QWERTYUIOP{}ASDFGHJKLZXCVBNM<>/ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБ401";
    private Dictionary<TextMeshProUGUI, Coroutine> cursedTextCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();
    [SerializeField] private AllScripts scripts;
    public bool lockAnyMenu;
    public float startCameraSize;

    public void Start()
    {
        startCameraSize = scripts.player.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
    }

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
        if (scripts.player.playerMenu.gameObject.activeSelf)
            return true;
        else if (scripts.interactions.floorChangeMenu.activeSelf)
            return true;
        else if (GameObject.Find("devTools").GetComponent<DevTool>().menuTools.activeSelf)
            return true;
        else if (scripts.dialogsManager.dialogMenu.activeSelf)
            return true;
        return lockAnyMenu;
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
            cursedTextCoroutines.Remove(text);
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