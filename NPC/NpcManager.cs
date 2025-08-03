using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NpcManager
{
    public static NpcManager Instance { get; private set; }
    public Npc[] AllNpc;
    public Dictionary<Npc, NpcTempInfo> npcTempInfo = new Dictionary<Npc, NpcTempInfo>();

    public async Task Initialize()
    {
        Instance = this;
        AllNpc = Resources.LoadAll<Npc>("NPC");

        // Инициализация информации об НПС
        foreach (Npc npc in AllNpc)
        {
            npcTempInfo.Add(npc, new NpcTempInfo());
            if (npc.nameInWorld == "") continue;
            GameObject obj = GameObject.Find(npc.nameInWorld);
            if (!obj) continue;
            try
            {
                npc.IHumanable = obj.GetComponent<IHumanable>();
                npc.IHumanable.NpcEntity = npc;
                await npc.IHumanable.Initialize();
            }
            catch (System.Exception e)
            {
                Debug.Log("npc init " + npc.nameInWorld + " error ");
            }
        }
    }

    public Npc GetNpcByName(string name)
    {
        foreach (Npc npc in AllNpc)
        {
            if (npc.nameInWorld == name)
                return npc;
        }

        return null;
    }

    public string GetDescriptionOfNpc(Npc npc)
    {
        string newDescription = "";
        NpcTempInfo npcTemp = npcTempInfo[npc];

        newDescription += new LanguageSetting("\nХарактер: ", "\nCharacter: ").Text + (npcTemp.knowCharacter ? npc.character.Text : "???");
        newDescription += new LanguageSetting("\nВозвраст: ", "\nAge: ").Text + (npcTemp.knowAge ? npc.age : "???");
        newDescription += new LanguageSetting("\nПрофессия: ", "\nProfession: ").Text + (npcTemp.knowProfession ? npc.profession.Text : "???");
        newDescription += new LanguageSetting("\nХобби: ", "\nHobby: ").Text + (npcTemp.knowHobby ? npc.hobby.Text : "???");

        return newDescription;
    }

    public string GetNpcRelationName(Npc npc)
    {
        if (npc.specialRelationName.Text != "") // Специальное имя
            return npc.specialRelationName.Text;

        switch (npcTempInfo[npc].relationshipWithPlayer)
        {
            case >= -10 and < -8:
                return new LanguageSetting("Ненависть", "Hate").Text;
            case >= -8 and < -5:
                return new LanguageSetting("Враждебность", "Hostile").Text;
            case >= -5 and < -2:
                return new LanguageSetting("Недоверие", "Mistrust").Text;
            case >= -2 and < -1:
                return new LanguageSetting("Сомнение", "Doubt").Text;
            case >= -1 and < 1:
                return new LanguageSetting("Пустота", "Nothing").Text;
            case >= 1 and < 3:
                return new LanguageSetting("Дружелюбие", "Friendliness").Text;
            case >= 3 and < 5:
                return new LanguageSetting("Доверие", "Trust").Text;
            case >= 5 and < 8:
                return new LanguageSetting("Симпатия", "Sympathy").Text;
            case >= 8:
                return new LanguageSetting("Привязанность", "Attachment").Text;
        }

        return "";
    }

    /// <summary>
    /// Насильное пермещение кого-либо или чего-либо
    /// </summary>
    public void MoveTo(Transform target, float speed, Transform pos, GameObject model, Animator animator)
    {
        Vector3 changeTarget = new Vector3(target.position.x, pos.position.y, pos.position.z);
        pos.position = Vector3.MoveTowards(pos.position, changeTarget, speed * Time.deltaTime);
        animator.SetBool("walk", true);
        float toTargetPos = target.position.x - pos.position.x;
        model.transform.localScale = toTargetPos switch
        {
            > 0 => new Vector3(-1, 1, 1),
            < 0 => new Vector3(1, 1, 1),
            _ => model.transform.localScale
        };
    }
}