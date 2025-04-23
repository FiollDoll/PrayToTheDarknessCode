using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public static NpcManager Instance { get; private set; }
    public Npc[] allNpc = new Npc[0];

    private void Awake() => Instance = this;
    
    public void Initialize()
    {
        // Инициализация информации об НПС
        foreach (Npc npc in allNpc)
        {
            npc.UpdateNpcStyleDict();
            GameObject npcObj = GameObject.Find(npc.nameInWorld);
            npc.NpcController = npcObj?.GetComponent<IHumanable>();
            if (npc.NpcController != null) // Если существо
                npc.NpcController.npcEntity = npc;

            npc.animator = npcObj?.GetComponent<Animator>();
        }
    }

    public Npc GetNpcByName(string name)
    {
        foreach (Npc npc in allNpc)
        {
            if (npc.nameInWorld == name)
                return npc;
        }

        return null;
    }
    
    /// <summary>
    /// Насильное пермещение кого-либо или чего-либо
    /// </summary>
    public void MoveTo(Transform target, float speed, Transform pos, SpriteRenderer spriteRenderer, Animator animator)
    {
        Vector3 changeTarget = new Vector3(target.position.x, pos.position.y, pos.position.z);
        pos.position = Vector3.MoveTowards(pos.position, changeTarget, speed * Time.deltaTime);
        animator.SetBool("walk", true);
        spriteRenderer.flipX = (target.position.x > pos.position.x);
    }
}