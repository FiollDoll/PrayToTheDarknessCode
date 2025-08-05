using UnityEngine;
using System.Collections.Generic;

public class DialogTempInfo
{
    public bool dialogLock;
    public bool dialogHasRead;
    public Dictionary<DialogStepNode, Choice> choices = new Dictionary<DialogStepNode, Choice>();

    public LockedChoices FindLockedChoice(Dialog dialog, string choiceName)
    {
        // Перебираем все ноды диалога и ищем тот самый choiceName
        foreach (DialogStepNode node in dialog.nodes)
        {
            if (node.choices == 1) continue;
            foreach (var choice in choices[node].lockedChoices)
            {
                if (choice.choiceName == choiceName)
                    return choice;
            }
        }

        Debug.Log("Locked choice " + choiceName + " not found");
        return null;
    }

    public bool ChoiceLocked(Dialog dialog, string choiceName)
    {
        LockedChoices lockedChoice = FindLockedChoice(dialog, choiceName);
        return lockedChoice != null && lockedChoice.choiceLock;
    }

    public void AddChoicesForNode(DialogStepNode node)
    {
        Choice newChoice = new Choice();
        for (int i = 0; i < node.choices; i++)
            newChoice.lockedChoices.Add(new LockedChoices(node.choiceName[i], node.choiceLock[i]));
        choices.Add(node, newChoice);
    }
}

// Тут пиздец с названиями

public class Choice
{
    public List<LockedChoices> lockedChoices = new List<LockedChoices>();
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