using UnityEngine;
using System.Collections.Generic;

public class DialogTempInfo
{
    public bool dialogLock;
    public bool dialogHasRead;
    public Dictionary<DialogStepNode, LockedChoices> lockedChoices = new Dictionary<DialogStepNode, LockedChoices>();

    public LockedChoices FindLockedChoice(string name)
    {
        foreach (var choice in lockedChoices.Values)
        {
            if (choice.choiceName == name)
                return choice;
        }

        Debug.Log("Locked choice " + name + " not found");
        return null;
    }
}

public class LockedChoices
{
    public string choiceName;
    public bool choiceLock;

    public LockedChoices(string choiceName, bool choiceLock)
    {
        this.choiceName = choiceName;
        this.choiceLock = choiceLock;
    }
}