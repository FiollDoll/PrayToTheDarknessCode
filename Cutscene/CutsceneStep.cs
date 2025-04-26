using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class CutsceneStep
{
    public string name;

    public bool closeDialogMenu;

    [Header("MainChanges")] public FastChangesController fastChanges;

    [Header("Locks")] public bool lockAllMenu;
    public LocationsLock[] locksLocations = new LocationsLock[0];

    [Header("UltraChanges")] public ObjectState[] objectsChangeState = new ObjectState[0];
    public ObjectTransform[] objectsChangeTransform = new ObjectTransform[0];
    public ObjectSprite[] objectsChangeSprite = new ObjectSprite[0];
    public Animations[] animatorsChanges = new Animations[0];

    [Header("Other")] public string startViewMenuActivate;
    public float editCameraSize;
    public NpcMoveToPlayer[] npcMoveToPlayer = new NpcMoveToPlayer[0];
    public HumanMove[] humansMove = new HumanMove[0];
    public VolumeProfile newVolumeProfile;
    public float timeDarkStart;
    public float timeDarkEnd = 1f;
    public float delayAndNext;
    public string chapterNext;
}

[System.Serializable]
public class LocationsLock
{
    public string location;
    public bool lockLocation;
}

[System.Serializable]
public class NpcMoveToPlayer
{
    public NpcController npc;
    public bool move;
}

[System.Serializable]
public class ObjectState
{
    public GameObject obj;
    public bool newState;
}

[System.Serializable]
public class ObjectTransform
{
    public GameObject obj;
    public Transform newTransform;
}

[System.Serializable]
public class ObjectSprite
{
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;
}

[System.Serializable]
public class Animations
{
    public Animator animator;
    public string boolName;
    public bool boolStatus;
}

[System.Serializable]
public class HumanMove
{
    public GameObject human;
    public Transform pointMove;
}