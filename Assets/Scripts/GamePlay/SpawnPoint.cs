using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour , SavableFab
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController PC = other.GetComponent<PlayerController>();
        if (PC)
            PC.SpawnPoint = transform.position;
    }

    public void LoadFab(mapdata.savedata data)
    {

    }

    public mapdata.savedata SaveFab()
    {
        return default;
    }
}
