using System.Collections.Generic;

public class PlayerStats
{
    private float _hp, _hunger, _addiction, _sanity;

    public float Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            PlayerMenu.Instance?.UpdateHpSlider();
        }
    }

    public float Hunger
    {
        get => _hunger;
        set
        {
            _hunger = value;
            PlayerMenu.Instance?.UpdateHungerSlider();
        }
    }

    public float Addiction
    {
        get => _addiction;
        set
        {
            _addiction = value;
            PlayerMenu.Instance?.UpdateAddictionSlider();
        }
    }

    public float Sanity
    {
        get => _sanity;
        set
        {
            _sanity = value;
            PlayerMenu.Instance?.UpdateSanitySlider();
        }
    }

    public const float MoveSpeed = 4f;
    public float ChangeSpeed;
    public List<Npc> FamiliarNpc = new List<Npc>();

    public PlayerStats()
    {
        Hp = 1f;
        Hunger = 0f;
        Addiction = 0.15f;
        Sanity = 1f;
    }

    public PlayerStats(float hp, float hunger, float addiction, float sanity)
    {
        Hp = hp;
        Hunger = hunger;
        Addiction = addiction;
        Sanity = sanity;
    }
}