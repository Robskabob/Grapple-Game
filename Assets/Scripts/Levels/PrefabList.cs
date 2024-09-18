using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PrefabList : MonoBehaviour, ISerializationCallbackReceiver
{
    public static PrefabList instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            PrefabList find = FindObjectOfType<PrefabList>();
            if (find != null) 
            {
                _instance = find;
                return _instance;
            }
            GameObject G = new GameObject();
            _instance = G.AddComponent<PrefabList>();
            return _instance;
        }
    }
    [SerializeField]
    private static PrefabList _instance;




    public PrefabList()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Debug.Log("unable to Removed Extra PreFablist.");
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            DestroyImmediate(this);
            Debug.Log("Removed Extra PreFablist.");
        }
        Debug.Log("Register Prefabs");
        for (int i = 0; i < Prefabs.Count; i++)
        {
            Transform T = Prefabs[i];
            if (T.GetComponent<NetworkIdentity>() != null)
            {
                NetworkClient.RegisterPrefab(T.gameObject, SpawnDelegate, UnspawnHandler);
            }
        }
    }
    public Dictionary<uint, GameObject> NetFabs;
    GameObject SpawnDelegate(SpawnMessage msg)
    {
        GameObject g = NetFabs[msg.assetId];//AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(msg.assetId.ToString("N")));

        Debug.Log(g == null);
        if (g == null) 
        {
            return null;
        }
        return Instantiate(g, msg.position, msg.rotation);
    }
    void UnspawnHandler(GameObject spawned)
    {
        Destroy(spawned);
    }

    public Transform GetFab(string name) 
    {
        if (PrefabsDict == null || Prefabs == null) 
        {
            Repopulate();
        }
        if (PrefabsDict.TryGetValue(name, out int i))
        {
            return Prefabs[i];
        }
        return null;
    }

    public Dictionary<string, int> PrefabsDict;
    public List<Transform> Prefabs;

//#if UNITY_EDITOR
    public void Repopulate() 
    {
        PrefabsDict = new Dictionary<string, int>();
        Prefabs = Resources.LoadAll<Transform>("Prefab").ToList();

        /*
        List<string> paths = new List<string>(Directory.GetDirectories(Application.dataPath+ "/Prefab"));
        paths.Add(Application.dataPath + "/Prefab");
        Debug.Log(Application.dataPath + "/Prefab");
        //if (Application.platform != RuntimePlatform.WindowsEditor)
        while (paths.Count > 0)
        {
            int i = paths.Count - 1;
            string path = paths[i];
            paths.RemoveAt(i);

            paths.AddRange(Directory.GetDirectories(path));
            string[] filePaths = Directory.GetFiles(path);
            for (int j = 0; j < filePaths.Length; j++)
            {
                if (Path.GetExtension(filePaths[j]) == ".prefab")
                {
                    Prefabs.Add(Resources.LoadAll<Transform>(filePaths[j]));
                    //Prefabs.Add(PrefabUtility.LoadPrefabContents(filePaths[j]).transform);
                    //Prefabs.Add(PrefabUtility.LoadPrefabContentsIntoPreviewScene(filePaths[j], SceneManager.GetActiveScene()) .transform);
                }
            }
        }
        //*/
        PrefabsDict = new Dictionary<string, int>();
        for (int i = 0; i < Prefabs.Count; i++) 
        {
            string name = Path.GetFileName(Prefabs[i].name);
            if (PrefabsDict.ContainsKey(name))
            {
                int j = 0;
                while (PrefabsDict.ContainsKey(name+j))
                {
                    j++;
                }
                name += j;
            }
            PrefabsDict.Add(name, i);
        }
        #if UNITY_EDITOR
        NetFabs = new Dictionary<uint, GameObject>();
        for (int i = 0; i < Prefabs.Count; i++)
        {
            GameObject G = Prefabs[i].gameObject;
            if (G.GetComponent<NetworkIdentity>() != null)
            {
                //FIX
                //Guid -> uint
                //NetFabs.Add(new Guid(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(G))),G);
            }
        }
        #endif
        //EditorUtility.SetDirty(this);//editor utility
    }
//#endif

    public void InstantiateFab(int i) 
    {
        Instantiate(Prefabs[i]);
    }
    public void InstantiateFab(int i, mapdata.savedata data)
    {
        SavableFab fab = (SavableFab)Instantiate(Prefabs[i]);
        fab.LoadFab(data);
    }

    private void OnDestroy()
    {

        //for (int i = 0; i < Prefabs.Count; i++)
        //{
        //    PrefabUtility.RevertPrefabInstance(PrefabUtility.GetNearestPrefabInstanceRoot(Prefabs[i]),InteractionMode.UserAction);
        //}
    }

    [Serializable]
    private class DictData 
    {
        public string[] PrefabNames;
        public int[] PrefabId;
        public uint[] NetFabGuid;
        public GameObject[] NetFabObject;

        public DictData(PrefabList P)
        {
            PrefabNames = P.PrefabsDict.Keys.ToArray();
            PrefabId = P.PrefabsDict.Values.ToArray();

            NetFabGuid = new uint[P.NetFabs.Count];
            int a = 0;
            foreach (uint i in P.NetFabs.Keys)
            {
                NetFabGuid[a] = i;
                a++;
            }

            NetFabObject = P.NetFabs.Values.ToArray();
        }

        public void Load(PrefabList P) 
        {
            P.PrefabsDict = new Dictionary<string, int>();
            for (int i = 0; i < PrefabNames.Length; i++) 
            {
                P.PrefabsDict.Add(PrefabNames[i], PrefabId[i]);
            }
            P.NetFabs = new Dictionary<uint, GameObject>();
            for (int i = 0; i < NetFabGuid.Length; i++)
            {
                P.NetFabs.Add(NetFabGuid[i], NetFabObject[i]);
            }
        }
    }
    [SerializeField]
    DictData dictData;
    public void OnBeforeSerialize()
    {
        dictData = new DictData(this);
    }

    public void OnAfterDeserialize()
    {
        if (dictData == null)
        {
            Debug.Log("No Prefab Data!");
            return;
        }
        dictData.Load(this);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PrefabList))]
public class PrefabListEditor : Editor
{
    int index;
    bool dict;
    bool dict2;
    private void OnDisable()
    {
        //PreviewRenderUtility.Cleanup();
    }

    public override void OnInspectorGUI()
    {
        PrefabList PL = (PrefabList)target;
        DrawDefaultInspector();

        dict = EditorGUILayout.Foldout(dict, "Prefabs Dict");
        if (dict) 
        { 
            int count = PL.PrefabsDict.Count;

            string[] key = new string[count];
            PL.PrefabsDict.Keys.CopyTo(key, 0);
            int[] val = new int[count];
            PL.PrefabsDict.Values.CopyTo(val, 0);

            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(key[i]);
                EditorGUILayout.IntField(val[i]);
                EditorGUILayout.EndHorizontal();
            }
        }
        dict2 = EditorGUILayout.Foldout(dict2, "Netfabs Dict");
        if (dict2) 
        { 
            int count = PL.NetFabs.Count;

            uint[] key = new uint[count];
            PL.NetFabs.Keys.CopyTo(key, 0);
            GameObject[] val = new GameObject[count];
            PL.NetFabs.Values.CopyTo(val, 0);

            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.ObjectField(key[i].ToString(),val[i],typeof(GameObject),false);
            }
        }

        if (GUILayout.Button("Repopulate")) {
            PL.Repopulate();
            EditorUtility.SetDirty(PL);
        }

        if (GUILayout.Button("find Assets"))
        {
            for (int i = 0; i < PL.Prefabs.Count; i++)
            {
                Transform T = PL.Prefabs[i];
                if (T.GetComponent<NetworkIdentity>() != null)
                {
                    string path = AssetDatabase.GetAssetPath(T);
                    string foundpath = AssetDatabase.FindAssets(T.name)[0];
                    string pathGuid = AssetDatabase.AssetPathToGUID(path);
                    string pathfromGuid = AssetDatabase.GUIDToAssetPath(pathGuid);
                    string foundpathfromGuid = AssetDatabase.GUIDToAssetPath(foundpath);
                    Debug.Log(T.name + " ID: " + pathGuid + " path:" + path + " create:" + pathfromGuid + " find:" + foundpath + " create found:" + foundpathfromGuid+"|"+(pathGuid==foundpath)+"|"+(pathfromGuid==path)+"|"+ (foundpathfromGuid == path));
                    Debug.Log("5221a23d-eb2f-0b84-59ee-1e78ddc927a5 == " + pathGuid+" = "+ ("5221a23d-eb2f-0b84-59ee-1e78ddc927a5" == pathGuid));
                }
            }
        }

        index = EditorGUILayout.IntField("Fab index",index);

        if (GUILayout.Button("Instantiate")) PL.InstantiateFab(index);
    }
}
#endif