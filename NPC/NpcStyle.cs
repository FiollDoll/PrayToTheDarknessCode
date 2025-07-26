[System.Serializable]
public class NpcStyle
{
    public string nameOfStyle;
    public string animatorStyleName;
    public NpcIcon styleIcon;

    public NpcStyle(string nameOfStyle)
    {
        this.nameOfStyle = nameOfStyle;
    }
}