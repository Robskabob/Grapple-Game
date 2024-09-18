using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FileShare : NetworkBehaviour
{
    public Dictionary<int,NetFile> InboundFiles = new Dictionary<int, NetFile>();
    public Dictionary<int,NetFile> OutboundFiles = new Dictionary<int, NetFile>();
    public Dictionary<int, Action> OutboundOnComplete = new Dictionary<int, Action>();

    public class NetFile 
    {
        public string path;
        public string Hash;
        public byte[] data;
        public int ChunkSize;
        public int MessageCount;
    }


    public void Start()
    {
        Debug.Log("Register FileShare Handlers");
        NetworkClient.RegisterHandler<HeaderMessage>(SendHeader,false);
        NetworkClient.RegisterHandler<FileChunkMessage>(SendChunk, false);
        if (isServer)
        {
            NetworkServer.RegisterHandler<HeaderResponseMessage>(SendHeaderResponse, false);
            NetworkServer.RegisterHandler<FileCompletedMessage>(SendFileCompleted, false);
        }
    }

    System.Random R = new System.Random();
    //sender s->r
    [Server]
    public void SendFile(NetworkConnection con, string path, Action complete) 
    {
        //Debug.Log("Start Sending File");
        int FileID = R.Next();
        NetFile file = new NetFile();
        file.path = path;
        //Debug.Log($"Read all to: {Path.Combine(Application.persistentDataPath, file.path)}");
        file.data = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, file.path));
        file.Hash = GetHash(file.data);

        file.MessageCount = (file.data.Length / (Transport.active.GetMaxPacketSize() - 1))+1;
        file.ChunkSize = file.data.Length / file.MessageCount;

        OutboundOnComplete.Add(FileID,complete);
        OutboundFiles.Add(FileID,file);

        HeaderMessage Header = new HeaderMessage();
        Header.FileID = FileID;
        Header.FileSize = file.data.Length;
        Header.MessageCount = file.MessageCount;
        Header.FileHash = file.Hash;
        Header.filename = file.path;// Path.GetFileName(file.path);
        //Debug.Log(Header.filename);
        con.Send(Header);
        Debug.Log("Sent Header to: " + con);

        //wait?
        //return new TaskAwaiter();
    }
    //receiver r->s 
    public void SendHeader(HeaderMessage H)
    {
        //Debug.Log("DataPath"+Application.dataPath);
        //Debug.Log("persistentDataPath" + Application.persistentDataPath);
        //Debug.Log("persistentDataPath" + Path.Combine(Application.persistentDataPath, "icon.png"));
        //Debug.Log("streamingAssetsPath" + Application.streamingAssetsPath);
        //Debug.Log("temporaryCachePath" + Application.temporaryCachePath);
        //Debug.Log("consoleLogPath" + Application.consoleLogPath);
        Debug.Log("Receive Header");
        HeaderResponseMessage HR = new HeaderResponseMessage();
        HR.FileID = H.FileID;
        string path = Path.Combine(Application.persistentDataPath, H.filename);
        
        //Debug.Log($"Path: {path} Hash {GetHash(path)} == fileHash{H.FileHash}");
        if(File.Exists(path) && GetHash(path) == H.FileHash)//test
        {
            Debug.Log("Same Hash Don't Send");
            HR.SendFile = false;
            NetworkClient.Send(HR);
        }
        else 
        {
            NetFile file = new NetFile();
            file.MessageCount = H.MessageCount;
            file.path = H.filename;
            file.Hash = H.FileHash;
            file.ChunkSize = H.FileSize / H.MessageCount;
            file.data = new byte[H.FileSize];
            
            InboundFiles.Add(H.FileID,file);

            HR.SendFile = true;
            NetworkClient.Send(HR);
            //Debug.Log("Sent Header Response to: "+ NetworkClient);
        }
    }
    //sender s->r
    public void SendHeaderResponse(NetworkConnection con,HeaderResponseMessage HR)
    { 
        Debug.Log("Receive Header Response");
        if (HR.SendFile)
        {
            NetFile file = OutboundFiles[HR.FileID];
            for (int i = 0; i < file.MessageCount; i++)
            {
                FileChunkMessage Chunk = new FileChunkMessage();
                Chunk.ChunkNumber = i;
                Chunk.FileID = HR.FileID;
                Chunk.data = new byte[file.ChunkSize+(file.data.Length%file.ChunkSize)];
                Buffer.BlockCopy(file.data, i * file.ChunkSize, Chunk.data, 0, Chunk.data.Length);
                con.Send(Chunk);
                //Debug.Log("Chunk sent to: "+con);
            }
        }
        else
        {
            OutboundFiles.Remove(HR.FileID);
            OutboundOnComplete[HR.FileID]();
            OutboundOnComplete.Remove(HR.FileID);
        }
    }

    private static string GetHash(string path)
    {
        using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(path))
                return Encoding.Default.GetString(md5.ComputeHash(stream));
    }
    private static string GetHash(byte[] data)
    {
        using (var md5 = MD5.Create())
        {
            return Encoding.Default.GetString(md5.ComputeHash(data));
        }
    }
    //receiver r->file
    public void SendChunk(FileChunkMessage Chunk)
    {
        Debug.Log("Receive Chunk");
        NetFile file = InboundFiles[Chunk.FileID];

        Buffer.BlockCopy(Chunk.data,0 ,file.data,Chunk.ChunkNumber * file.ChunkSize, Chunk.data.Length);
        file.MessageCount--;

        //Debug.Log($"{file.Hash == GetHash(file.data)} == {file.Hash} == {GetHash(file.data)}");
        if (file.Hash == GetHash(file.data)) 
        {
            FileCompletedMessage FC = new FileCompletedMessage();
            FC.FileID = Chunk.FileID;
            string path = Path.Combine(Application.persistentDataPath, file.path);
            Debug.Log($"Write all to: {path}");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, file.data);
            InboundFiles.Remove(FC.FileID);
            NetworkClient.Send(FC);
        }
        else if (file.MessageCount == 0)
        {
            FileCompletedMessage FC = new FileCompletedMessage();
            FC.FileID = Chunk.FileID;
            Debug.Log($"File Failed");
            //File.WriteAllBytes(path, file.data);

            InboundFiles.Remove(FC.FileID);
            NetworkClient.Send(FC);
        }
    }
    //sender s->remove queue
    public void SendFileCompleted(NetworkConnection con, FileCompletedMessage FC)
    {
        //Debug.Log("SendFileCompleted");
        OutboundFiles.Remove(FC.FileID);
        string s = "";
        foreach (int k in OutboundOnComplete.Keys) 
        {
            s+=$"key:{k}";
        }
        //Debug.Log($"File Key: {FC.FileID}" + s);
        OutboundOnComplete[FC.FileID]();
        OutboundOnComplete.Remove(FC.FileID);
    }
    public struct HeaderMessage : NetworkMessage
    {
        public int FileID;
        public int FileSize;
        public int MessageCount;
        public string FileHash;
        public string filename;
    }
    public struct HeaderResponseMessage : NetworkMessage
    {
        public int FileID;
        public bool SendFile;
    }
    public struct FileChunkMessage : NetworkMessage
    {
        public int FileID;
        public int ChunkNumber;
        public byte[] data;
    }
    public struct FileCompletedMessage : NetworkMessage
    {
        public int FileID;
    }
}
