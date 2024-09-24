using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace IO
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                GameObject G = new GameObject();
                _instance = G.AddComponent<SaveSystem>();
                return _instance;
            }
        }
        [SerializeField]
        private static SaveSystem _instance;

        [ExecuteAlways]
        void Awake()
        {
            DontDestroyOnLoad(this);
            if (_instance == null)
                _instance = this;
            else if (_instance != this)
            {
                Destroy(this);
                Debug.Log("Removed Extra KeySystem");
            }
        }
        #region Copy
        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.FullRect)
        {
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            Texture2D SpriteTexture = LoadTexture(FilePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

            return NewSprite;
        }
        public static Texture2D LoadTexture(string FilePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails
            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }
        #endregion
        /// <summary>
        /// Saves a file to the Game
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">the Object to save</param>
        /// <param name="Path">the path to save to</param>
        /// <param name="filename">name of the file(dont forget the file extension ex. .png, .dat)</param>
        public static void Save<T>(T data, string path, string filename, System.Runtime.Serialization.IFormatter Formatter)
        {
            string destination = Path.Combine(Application.persistentDataPath, path);
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);
            destination = Path.Combine(destination, filename);

            FileStream file;
            if (File.Exists(destination)) file = File.OpenWrite(destination);
            else file = File.Create(destination);

            //BinaryFormatter bf = new BinaryFormatter();
            Formatter.Serialize(file, data);
            Debug.Log(destination);
            file.Close();
        }
        public static void Save<T>(T data, string path, string filename)
        {
            Save(data, path, filename, new BinaryFormatter());
        }
        public static void Save(string data, string path, string filename)
        {
            string destination = Path.Combine(Application.persistentDataPath, path);
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);
            destination = Path.Combine(destination, filename);

            //FileStream file;
            //if (File.Exists(destination)) file = File.OpenWrite(destination);
            //else file = File.Create(destination);

            Debug.Log(destination);
            File.WriteAllText(destination,data);

            //file.Close();
        }

        public static T LoadFile<T>(string path, string filename)
        {
            string app = Application.persistentDataPath;
            string destination = Path.Combine(app, path, filename);
            FileStream file;

            if (File.Exists(destination)) file = File.OpenRead(destination);
            else
            {
                Debug.LogError("File not found at: " + destination);
                Debug.LogError(destination + "|" + app + path + filename);
                return default;
            }

            BinaryFormatter bf = new BinaryFormatter();
            T data = (T)bf.Deserialize(file);
            file.Close();

            return data;
        }
        public static T LoadFile<T>(string path, string filename,out bool Failed)
        {
            string app = Application.persistentDataPath;
            string destination = Path.Combine(app, path, filename);
            FileStream file;

            if (File.Exists(destination)) file = File.OpenRead(destination);
            else
            {
                Debug.Log("File not found at: " + destination);
                Debug.Log(destination + "|" + app + path + filename);
                Failed = true;
                return default;
            }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                T data = (T)bf.Deserialize(file);
                file.Close();
                Failed = false;
                return data;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Failed = true;
                return default;
            }
        }
    }
}
public class L33tFormater : System.Runtime.Serialization.IFormatter
{
    public SerializationBinder Binder { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public StreamingContext Context { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public ISurrogateSelector SurrogateSelector { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public object Deserialize(Stream serializationStream)
    {
        throw new System.NotImplementedException();
    }

    public void Serialize(Stream serializationStream, object graph)
    {
        var objectType = graph.GetType();
        var serializationSurrogate = SurrogateSelector?.GetSurrogate(objectType, Context, out var _);
        //if (serializationSurrogate != null)
        //    SerializeWithSurrogate(serializationStream, graph, objectType, serializationSurrogate);
        //else if (graph is ISerializable serializable)
        //    SerializeAsISerializable(serializationStream, graph, objectType, serializable);
        //else
        //    SerializeWithFormatterServices(serializationStream, graph, objectType);
        //GetCallbackDelegate(objectType, typeof(OnSerializedAttribute))
        //                   ?.DynamicInvoke(graph, Context);
    }
}