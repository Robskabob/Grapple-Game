using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelUI : MonoBehaviour
{
    //public MainMenu main;
    public LevelSelectUI LS;
    //public leveldata ld;
    public bool Unlocked;
    public Text Name;
    public UnityEngine.UI.Button Start;
    public Image Icon;

    public void Create(LevelSelectUI ls, leveldata LD,Sprite sp) 
    {
        LS = ls;
        //main = MM;
        //ld = LD;
        Unlocked = LD.UnLocked;
        name = LD.Name;
        Name.text = LD.Name;
        Icon.sprite = sp;

        Start.onClick.AddListener(Load);

    }
    public void Load() 
    {
        LS.Requester.OnLevelSelected(name);
        /*
        main.inGame = true;
        main.game.CurrentLevel = ld.Name;
        main.Light.gameObject.SetActive(false);
        Level L = (new GameObject("Created Level")).AddComponent<Level>();
        L.LoadFile(ld.Name); 
        L.Load();
        //*/
        //SceneManager.LoadSceneAsync(ld.Name, LoadSceneMode.Additive);
    }
}