using System.Threading.Tasks;
using UnityEngine;

public interface IHumanable
{
    public Npc NpcEntity { get; set; }
    public string SelectedStyle { get; set; }

    public Task Initialize();
    public void ChangeStyle(string newStyle);
    public void MoveTo(Transform target);
}