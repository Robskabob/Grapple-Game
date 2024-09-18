
using UnityEngine.UI;

public class Folder : Hierarchy.Element
{
    public bool Open;
    public SaveFolder SaveFolder;
    public UnityEngine.UI.Button button;

    public void Toggel() 
    {
        Open = !Open;
    }
}