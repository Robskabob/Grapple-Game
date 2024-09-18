using IO;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour 
{
    public MainMenu Main;
    public List<LevelUI> Levels;
    public ScrollRect SV;
    public LevelUI fab;
    public RectTransform content;
    public Format format;
    const int Size = 150;
    const int Padding = 20;
    public RoomLobbyManager Requester;
    //public LevelRequester Requester;
    public interface LevelRequester
    {
        void OnLevelSelected(string levelName);
    }

    void Start()
    {
        CreateUI();
    }

    private void Update()
    {
        if (format == Format.List) 
        {
            format = Format.Branch;
            FormatList();
        }
    }

    public void ReFormat(Format format) 
    {
    
    }

    public void FormatBranch() { }
    public void FormatList()
    {
        const int hc = 3;//horizontal count
        content.sizeDelta = new Vector2(((Levels.Count + hc - 1) / hc) * Size, (hc) * Size);
        float pad = (Size / 2) + Padding;
        for (int i = 0; i < Levels.Count; i++) 
        {
            Levels[i].transform.localPosition = new Vector3((i / hc)*(Size + Padding) + pad, -(i % hc) * (Size + Padding) - pad, 0); 
        }
    }
    public void CreateUI()
    {
        DestroyUI();
        string[] paths = Directory.GetDirectories(Application.persistentDataPath + "/SaveData/Levels");
        for (int i = 0; i < paths.Length; i++) 
        {
            leveldata ld = SaveSystem.LoadFile<leveldata>(paths[i],"Level.dat");
            Sprite sp = SaveSystem.LoadNewSprite(Path.Combine(paths[i],"Icon.png"));
            LevelUI UI = Instantiate(fab,content);
            UI.Create(this,ld,sp);
            Levels.Add(UI);
        }
        FormatList();
    }
    public void DestroyUI()
    {
        if (Levels != null)
            for (int i = 0; i < Levels.Count; i++) 
                Destroy(Levels[i].gameObject);
    }

    public enum Format 
    {
        Branch,
        List,
        Race
    }
}
