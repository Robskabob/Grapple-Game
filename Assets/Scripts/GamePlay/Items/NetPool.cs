using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetPool<T> : MonoBehaviour where T : NetworkBehaviour
{
    public static NetPool<T> Instance;
    [Header("Settings")]
    public int startSize = 5;
    public int maxSize = 20;
    public T prefab;

    [Header("Debug")]
    [SerializeField] Queue<T> pool;
    [SerializeField] int currentCount;


    void Start()
    {
        if (Instance != null)
            Debug.LogError($"Instance {Instance.name} Already exists");
        Instance = this;
        InitializePool();
        NetworkClient.RegisterPrefab(prefab.gameObject, SpawnHandler, UnspawnHandler);
    }

    void OnDestroy()
    {
        NetworkClient.UnregisterPrefab(prefab.gameObject);
    }

    private void InitializePool()
    {
        pool = new Queue<T>();
        for (int i = 0; i < startSize; i++)
        {
            T next = CreateNew();

            pool.Enqueue(next);
        }
    }

    T CreateNew()
    {
        if (currentCount > maxSize)
        {
            Debug.LogError($"Pool has reached max size of {maxSize}");
            return null;
        }

        // use this object as parent so that objects dont crowd hierarchy
        T next = Instantiate(prefab, transform);
        next.name = $"{prefab.name}_pooled_{currentCount}";
        next.gameObject.SetActive(false);
        currentCount++;
        return next;
    }

    // used by ClientScene.RegisterPrefab
    GameObject SpawnHandler(SpawnMessage msg)
    {
        return GetFromPool(msg.position, msg.rotation).gameObject;
    }

    // used by ClientScene.RegisterPrefab
    void UnspawnHandler(GameObject spawned)
    {
        PutBackInPool(spawned.GetComponent<T>());
    }

    public static T GetInstance()
    {
        T next = Instance.pool.Count > 0
            ? Instance.pool.Dequeue()
            : Instance.CreateNew(); 

        if (next == null) { return null; }

        next.gameObject.SetActive(true);
        return next;
    }

    /// <summary>
    /// Used to take Object from Pool.
    /// <para>Should be used on server to get the next Object</para>
    /// <para>Used on client by ClientScene to spawn objects</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public T GetFromPool(Vector3 position, Quaternion rotation)
    {
        T next = pool.Count > 0
            ? pool.Dequeue() // take from pool
            : CreateNew(); // create new because pool is empty

        // CreateNew might return null if max size is reached
        if (next == null) { return null; }

        // set position/rotation and set active
        next.transform.position = position;
        next.transform.rotation = rotation;
        next.gameObject.SetActive(true);
        return next;
    }

    /// <summary>
    /// Used to put object back into pool so they can b
    /// <para>Should be used on server after unspawning an object</para>
    /// <para>Used on client by ClientScene to unspawn objects</para>
    /// </summary>
    /// <param name="spawned"></param>
    public static void PutBackInPool(T spawned)
    {
        // disable object
        spawned.gameObject.SetActive(false);

        // add back to pool
        Instance.pool.Enqueue(spawned);
    }
}
