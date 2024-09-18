using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class IconCamera : MonoBehaviour
{
    public Camera C;
    public int width;
    public int height;
    public void Render()
    {
        Render("SaveData/Level/Icons",SceneManager.GetActiveScene().name);
    }
    public void Render(string path, string name) 
    {
        RenderTexture tempRT = new RenderTexture(width, height, 24);
        C.targetTexture = tempRT;
        C.Render();
        RenderTexture.active = tempRT;
        Texture2D tex = new Texture2D(width,height,TextureFormat.RGB24,false);


        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        C.targetTexture = null;
        DestroyImmediate(tempRT);

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        DestroyImmediate(tex);
        File.WriteAllBytes(Application.persistentDataPath + "/" + path + "/" + name + ".png", bytes);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IconCamera))]
public class IconCameraEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IconCamera IC = (IconCamera)target;

        if (GUILayout.Button("Render"))
            IC.Render();
    }
}
#endif