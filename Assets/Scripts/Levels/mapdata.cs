using Mirror;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
/// <summary>
/// 
/// </summary>
[System.Serializable]
public struct mapdata
{
    public List<IObject> Objects;
    public List<string> ObjectIDs;
    //public List<KeyValuePair<vec3,int>> SpawnMarkers;//unused
    public vec3 PlayerPos;
    public interface IObject
    {
        vec3 Pos { get; set; }
        vec3 Scale { get; set; }
        vec3 Rotation { get; set; }

#if UNITY_EDITOR
        void Draw(ref bool[] boolObs, ref int boolIndex);
#endif
        string Name();
        int BoolNum();
        void Load(Transform T,mapdata map);
    }
    [System.Serializable]
    public struct FabObject : IObject
    {
        public int ObjectID;
        public vec3 Pos { get; set; }
        public vec3 Scale { get; set; }
        public vec3 Rotation { get; set; }
        public savedata data;
        public FabObject(int objectID, Vector3 pos, Vector3 scale, Vector3 rotation, savedata data)
        {
            ObjectID = objectID;
            Pos = pos;
            Scale = scale;
            Rotation = rotation;
            this.data = data;
        }
#if UNITY_EDITOR
        public void Draw(ref bool[] boolObs, ref int boolIndex)
        {
            boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Data");
            if (boolObs[boolIndex++])
            {
                EditorGUILayout.IntField("ID", ObjectID);
                EditorGUILayout.Vector3Field("Position", Pos);
                EditorGUILayout.Vector3Field("Rotation", Rotation);
                EditorGUILayout.Vector3Field("Scale", Scale);
                EditorGUILayout.LabelField("Data");
            }
            boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Special");
            if (boolObs[boolIndex++])
            {
                EditorGUI.indentLevel++;
                data.Draw(ref boolObs, ref boolIndex);
                EditorGUI.indentLevel--;
            }
        }
#endif

        public int BoolNum()
        {
            if(data == null)
                return 2;
            else
                return 2 + data.BoolNum();
        }

        public string Name()
        {
            return "Object with ID:" + ObjectID;
        }

        public void Load(Transform T, mapdata map)
        {
            Transform G;
            if (ObjectID < 0)
            {
                G = new GameObject("Null").transform;
                G.parent = T;
                SavableFab sf2 = G.GetComponent<SavableFab>();
                sf2?.LoadFab(data);
                return;
            }
            Transform Pre = PrefabList.instance.GetFab(map.ObjectIDs[ObjectID]);
            if (Pre == null)
            {
                G = new GameObject("NullError").transform;
                G.parent = T;
            }
            else 
            {
                if (Pre.GetComponent<NetworkIdentity>() != null && NetworkClient.active)
                {
                    int t = 1;
                    if (Pre.GetComponent<Holdable>() != null)
                        t = 2;
                    for (int i = 0; i < t; i++)
                    {
                        if (NetworkServer.active)
                        {
                            G = UnityEngine.Object.Instantiate(Pre, T);
                            G.position = Pos;
                            G.rotation = Quaternion.Euler(Rotation);
                            G.localScale = Scale;
                            SavableFab sf2 = G.GetComponent<SavableFab>();
                            sf2.LoadFab(data);
                            NetworkServer.Spawn(G.gameObject);
                            //G = UnityEngine.Object.Instantiate(Pre, T);
                        }
                    }
                    return;
                }
                else
                {
                    G = UnityEngine.Object.Instantiate(Pre, T);
                }
            }
            G.position = Pos;
            G.rotation = Quaternion.Euler(Rotation);
            G.localScale = Scale;
            SavableFab sf = G.GetComponent<SavableFab>();
            sf.LoadFab(data);
        }
    }
    [System.Serializable]
    public struct Object : IObject
    {
        string IObject.Name()
        {
            return Name;
        }
        public string Name { get; set; }
        public vec3 Pos { get; set; }
        public vec3 Scale { get; set; }
        public vec3 Rotation { get; set; }
        public savedata data;
        public Object(string name, Vector3 pos, Vector3 scale, Vector3 rotation, savedata data)
        {
            Name = name;
            Pos = pos;
            Scale = scale;
            Rotation = rotation;
            this.data = data;
        }
#if UNITY_EDITOR
        public void Draw(ref bool[] boolObs, ref int boolIndex)
        {
            boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Data");
            if (boolObs[boolIndex++])
            {
                EditorGUILayout.TextField("Name", Name);
                EditorGUILayout.Vector3Field("Position", Pos);
                EditorGUILayout.Vector3Field("Rotation", Rotation);
                EditorGUILayout.Vector3Field("Scale", Scale);
                EditorGUILayout.LabelField("Data");
            }
            boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Special");
            if (boolObs[boolIndex++])
            {
                EditorGUI.indentLevel++;
                data.Draw(ref boolObs, ref boolIndex);
                EditorGUI.indentLevel--;
            }
        }
#endif
        public int BoolNum()
        {
            return 2 + data.BoolNum();
        }
        /// <summary>
        /// BROKEN PLS FIX
        /// </summary>
        /// <param name="T"></param>
        /// <param name="map"></param>
        public void Load(Transform T, mapdata map)
        {
            Transform G = data.LoadObject();
            G.parent = T;
            //SavableObject sf2 = G.GetComponent<SavableObject>();
            //sf2.LoadObject(data);
            G.position = Pos;
            G.rotation = Quaternion.Euler(Rotation);
            G.localScale = Scale;
            SavableFab sf = G.GetComponent<SavableFab>();
            sf.LoadFab(data);
        }
    }
    [System.Serializable]
    public struct Folder : IObject
    {
        public string Name;
        public vec3 Pos { get; set; }
        public vec3 Scale { get; set; }
        public vec3 Rotation { get; set; }
        public List<IObject> Objects;
        public Folder(string name, Vector3 pos, Vector3 rotation, Vector3 scale, List<IObject> objects)
        {
            Name = name;
            Pos = pos;
            Scale = scale;
            Rotation = rotation;
            Objects = objects;
        }
        public int BoolNum() 
        {
            int n = Objects.Count+2;
            for (int i = 0; i < Objects.Count; i++)
            {
                n += Objects[i].BoolNum();
            }
            return n;
        }
#if UNITY_EDITOR
        public void Draw(ref bool[] boolObs,ref int boolIndex)
        {
            boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Data");
            if (boolObs[boolIndex++])
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(Name);
                EditorGUILayout.Vector3Field("Position", Pos);
                EditorGUILayout.Vector3Field("Rotation", Rotation);
                EditorGUILayout.Vector3Field("Scale", Scale);
                EditorGUI.indentLevel--;
            }
            boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "SubObjects");
            if (boolObs[boolIndex++])
            {
                EditorGUI.indentLevel++;
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
                EditorGUI.indentLevel--;
            }
        }
#endif
        string IObject.Name()
        {
            return Name;
        }

        public void Load(Transform T, mapdata map)
        {
            Transform folder = new GameObject(Name).transform;
            folder.gameObject.AddComponent<SaveFolder>();
            folder.parent = T;
            for (int i = 0; i < Objects.Count; i++) 
            {
                Objects[i].Load(folder,map);
            }
        }
    }
    [System.Serializable]
    public struct vec3
    {
        public float x;
        public float y;
        public float z;

        public vec3(Vector3 vec)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }
        public static implicit operator vec3(Vector3 v)
        {
            return new vec3(v);
        }
        public static implicit operator Vector3(vec3 v)
        {
            return new Vector3(v.x,v.y,v.z);
        }
    }
    public interface savedata
    {
#if UNITY_EDITOR
        void Draw(ref bool[] boolObs, ref int boolIndex);
#endif
        int BoolNum();
        Transform LoadObject();
    }

    public mapdata(Level L)
    {
        Objects = new List<IObject>();
        ObjectIDs = new List<string>();
        //SpawnMarkers = new List<KeyValuePair<vec3, int>>();
        PlayerPos = L.GetComponentInChildren<PlayerController>().transform.position;
        int dictIdCount = 0;
        int overcount = 0;
        Objects = SaveFolder(L.transform,ref dictIdCount, overcount);
    }

    public void Load(Level L)
    {
        //if (L.Player == null)
        //    L.Player = UnityEngine.Object.Instantiate(PrefabList.instance.GetFab("Player New")).GetComponent<PlayerController>();
        //L.transform.position = PlayerPos;

        for (int i = 0; i < Objects.Count; i++)
        {
            Objects[i].Load(L.transform, this);
        }
    }
    public void UnLoad(Level L)
    {
        for (int i = 0; i < L.transform.childCount; i++)
        {
			UnityEngine.Object.Destroy(L.transform.GetChild(i).gameObject);
        }
        UnityEngine.Object.Destroy(L.Player.gameObject);
        GameManager.GM.MainMenu.Camera.gameObject.SetActive(true);
    }

    private List<IObject> SaveFolder(Transform T,ref int dictIdCount,int overcount)
    {
        List<IObject> Objects = new List<IObject>();
        foreach (Transform child in T)
        {
            overcount++;
            if (overcount > 150) 
            {
                Debug.LogError("Saved more than 50 objects");
                return null;
            }
            SavableFolder<SaveFolder> fold = child.GetComponent<SavableFolder<SaveFolder>>();
            SavableFab fab = child.GetComponent<SavableFab>();
            SavableObject obj = child.GetComponent<SavableObject>();
            if (fold != null)
            //Debug.Log((fold == null) +"|"+ child.transform.name);
            if (child == T) 
            {
                continue;
            }
            if (fab != null)
            {
                Objects.Add(SaveFabObject(ref dictIdCount, child, fab));
                //Debug.Log("Save Object");
            }
            else if (fold != null)
            {
                Transform Tfold = fold.This.transform;
                //Debug.Log("Save Folder "+Tfold.name);
                Objects.Add(new Folder(Tfold.name, Tfold.position, Tfold.rotation.eulerAngles, Tfold.localScale, SaveFolder(child, ref dictIdCount, overcount)));
                //Objects.Add(new Folder("name",));
            }
            else if(obj != null)
            {
                Objects.Add(SaveObject(child,obj));
            }
            else
            {
                //Debug.Log("Save Not");
            }
        }

        return Objects;
    }

    private IObject SaveFabObject(ref int dictIdCount, Transform child, SavableFab fab)
    {
        savedata data = fab?.SaveFab();
        //Debug.Log(child.name+(data == null));
        //UnityEngine.Object Pre = PrefabUtility.GetPrefabInstanceHandle(child);
        //UnityEngine.Object Pre = PrefabUtility.GetOutermostPrefabInstanceRoot(child);
#if UNITY_EDITOR
        Transform Pre = PrefabUtility.GetCorrespondingObjectFromSource(child);//bad
#else
        Transform Pre = child;
#endif
        string name;
        if (Pre == null)
        {
            name = "None";
            //if (!ObjectIDs.Contains("None"))
            //    ObjectIDs.Add(name);
        }
        else
        {
            name = Pre.name;
            //Debug.Log(name);
        }
        int j;
        if (PrefabList.instance.PrefabsDict.TryGetValue(name, out int i))
        {
            j = ObjectIDs.FindIndex(a => a == name);
            if (j == -1)
            {
                j = dictIdCount++;
                ObjectIDs.Add(name);
            }
        }
        else
        {
            j = -1;
        }
        if (j == -1) 
        {
            SavableObject SO = child.GetComponent<SavableObject>();
            if (SO != null)
            {
                return new Object(child.name, child.position, child.localScale, child.rotation.eulerAngles, data);
            }
        }
        return new FabObject(j, child.position, child.localScale, child.rotation.eulerAngles, data);
    }
    private IObject SaveObject(Transform child, SavableObject obj)
    {
        savedata data = obj?.SaveObject();
        return new Object(child.name, child.position, child.localScale, child.rotation.eulerAngles, data);
    }
}

public interface SavableObject 
{
    Transform LoadObject(mapdata.savedata data);
    mapdata.savedata SaveObject();
}