using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class NumbersAndArrowsMiniGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNumbers;
    private string needNumbers = "411"; // Пока фиксировано
    private int[] _playerNumbers = new int[3] { 0, 0, 0 };

    private void Start() => UpdateText();

    private void UpdateText()
    {
        textNumbers.text = "";
        foreach (int number in _playerNumbers)
            textNumbers.text += number + " ";
    }

    public void AddNumber(int id)
    {
        _playerNumbers[id]++;
        if (_playerNumbers[id] == 10)
            _playerNumbers[id] = 0;
        UpdateText();
    }

    public void RemoveNumber(int id)
    {
        _playerNumbers[id]--;
        if (_playerNumbers[id] == -1)
            _playerNumbers[id] = 9;
        UpdateText();
    }

    public async void ApplyAnswer()
    {
        string totalAnswer = "";
        foreach (int number in _playerNumbers)
            totalAnswer += number.ToString();
        if (totalAnswer == needNumbers)
        {
            if (MiniGamesManager.Instance.gameChangesAfterWin != null)
                await MiniGamesManager.Instance.gameChangesAfterWin.ActivateChanges();
            CloseMenu();
        }
    }

    public void CloseMenu() => Destroy(this.gameObject);
}
