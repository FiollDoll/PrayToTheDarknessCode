using UnityEngine;

public class CoroutineContainer : MonoBehaviour
{
    public static CoroutineContainer Instance { get; private set; }

    private void Awake() => Instance = this;
}
