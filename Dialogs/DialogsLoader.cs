using UnityEngine;
using System.IO;

public class DialogsLoader : MonoBehaviour
{
    public static DialogsLoader Instance { get; private set; }
    

    private void Awake() => Instance = this;

    public Dialog[] LoadDialogs()
    {
        TextAsset json =  Resources.Load<TextAsset>("dialogues");
        Dialog dialog = new Dialog();
        return JsonUtility.FromJson<DialogCollection>(json.text).dialogs;
    }
       
    [System.Serializable]
    private class DialogCollection
    {
        public Dialog[] dialogs;
    }
}