using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public static NpcManager Instance { get; private set; }
    public Npc[] allNpc = new Npc[0];

    private void Awake() => Instance = this;
    
    private void Start()
    {
        // Инициализация информации об НПС
        foreach (Npc npc in allNpc)
        {
            npc.UpdateNpcStyleDict();
            GameObject npcObj = GameObject.Find(npc.nameInWorld);
            npc.NpcController = npcObj.GetComponent<IHumanable>();
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
        pos.position = Vector3.MoveTowards(pos.position, target.position, speed * Time.deltaTime);
        animator.SetBool("walk", true);
        spriteRenderer.flipX = !(target.position.x > pos.position.x);
    }
}