using UnityEngine;
using UnityEngine.SceneManagement;

public class prehistory : MonoBehaviour
{
    [SerializeField] private GameObject prehistoryObj, trainingObj;
    private bool prehistoryEnded = true; // Потом убрать true

    private void Update()
    {
        if (prehistoryEnded)
        {
            if (Input.anyKey)
                SceneManager.LoadScene("step1");
        }
    }
}
