using IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Level : MonoBehaviour
{
    public leveldata ld;
    public mapdata map;
    public IconCamera IC;
    public PlayerController Player;
    //public const string dataPath = "C:\Users\daL33\AppData\LocalLow\DefaultCompany\Parkour Game";
    void Start()
    {
        
    }
    void Update()
    {

    }
    public void LevelComplete() 
    {
        
    }
    public void Load() 
    {
        map.Load(this);
    }
    public void UnLoad() 
    {
        map.UnLoad(this);
    }
    public void Save() 
    {
        if (ld.Name == "")
        {
            Debug.LogError("no name Selected");
            return;
        }
        Directory.CreateDirectory(Application.persistentDataPath + "/SaveData/Levels/" + ld.Name);
        SaveSystem.Save(ld, Application.persistentDataPath + "/SaveData/Levels/" + ld.Name + "", "Level.dat");
        map = new mapdata(this);
        SaveSystem.Save(map, Application.persistentDataPath + "/SaveData/Levels/" + ld.Name + "", "map.dat");
        IC.Render("/SaveData/Levels/" + ld.Name + "", "Icon");
        Debug.Log("Saved: " + ld.Name + " to " + Application.persistentDataPath + "/SaveData/Levels/" + ld.Name);
        //EditorUtility.SetDirty(this);
    }

    public void LoadFile(string mapName) 
    {
        ld = SaveSystem.LoadFile<leveldata>("SaveData/Levels/" + mapName + "", "Level.dat");
        map = SaveSystem.LoadFile<mapdata>("SaveData/Levels/" + mapName + "", "map.dat");
        //EditorUtility.SetDirty(this);
    }

    [System.Serializable]
    public enum Type { 
        Race,
        Complete,
        Find,
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private void OnEnable()
    {
        Level L = (Level)target;
        //int n = L.map.Objects.Count;
        //for (int i = 0; i < L.map.Objects.Count; i++)
        //{
        //    n += L.map.Objects[i].BoolNum();
        //}
        //boolObs = new bool[n];
    }
    bool Objects;
    bool[] boolObs;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Level L = (Level)target;

        if (GUILayout.Button("Save Level")) { PrefabList.instance.Repopulate(); L.Save(); }
        if (GUILayout.Button("Load File")) L.LoadFile(L.ld.Name);
        if (Application.isPlaying && GUILayout.Button("Load Level")) L.Load();
        Objects = EditorGUILayout.Foldout(Objects,"Map Objects");
        EditorGUI.indentLevel++;
        if (Objects && L.map.Objects != null) 
        {
            List<mapdata.IObject> Objects = L.map.Objects;
            int boolIndex = 0;
            int n = Objects.Count;
            for (int i = 0; i < Objects.Count; i++)
            {
                n += Objects[i].BoolNum();
            }
            if (boolObs == null || n != boolObs.Length) 
            {
                
                boolObs = new bool[n];
            }
            for (int i = 0; i < Objects.Count; i++)
            {
                boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], Objects[i].Name());
                if (boolObs[boolIndex++])
                {
                    EditorGUI.indentLevel++;
                    Objects[i].Draw(ref boolObs, ref boolIndex);
                    EditorGUI.indentLevel--;
                }
                else 
                {
                    boolIndex += Objects[i].BoolNum();
                }
            }
        }
        EditorGUI.indentLevel--;
    }
}
#endif