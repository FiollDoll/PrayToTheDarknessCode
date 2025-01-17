using UnityEngine;

public interface IHumanable
{
    public Npc npcEntity { get; set; }
    public string selectedStyle { get; set; }

    public void ChangeStyle(string newStyle);
}