using UnityEngine;

public interface IHumanable
{
    public Npc NpcEntity { get; set; }
    public string SelectedStyle { get; set; }

    public void ChangeStyle(string newStyle);
    public void MoveTo(Transform target);
}