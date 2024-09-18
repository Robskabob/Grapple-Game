using UnityEngine;

public interface SavableFab : ISavable
{
    void LoadFab(mapdata.savedata data);
    mapdata.savedata SaveFab();
}

public interface ISavable//<T> where T : MonoBehaviour
{
    Transform transform { get; }
    GameObject gameObject { get; }
}