using UnityEngine;

public interface IMenuable
{
    public GameObject menu { get; }

    public void ManageActivationMenu();
}