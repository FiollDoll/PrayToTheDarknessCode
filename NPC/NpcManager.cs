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
            if (npc.nameInWorld == "") continue;
            GameObject obj = GameObject.Find(npc.nameInWorld);
            if (!obj) continue;
            try
            {
                npc.IHumanable = obj.GetComponent<IHumanable>();
                npc.IHumanable.NpcEntity = npc;
                npcTempInfo.Add(npc, new NpcTempInfo());
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