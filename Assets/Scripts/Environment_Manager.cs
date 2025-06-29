using System.Collections.Generic;
using UnityEngine;

public class Environment_Manager : MonoBehaviour
{
    public static Environment_Manager instance;
    [SerializeField] Transform patrolPointFolder;
    public List<Transform> patrolDestinations;
    [Space(10)]
    [SerializeField] Transform keySpawnPointsFolder;
    [SerializeField] Transform keysFolder;
    [SerializeField] List<Transform> keySpawns;
    [Space(10)]
    [SerializeField] GameObject[] KeyPrefabs;
    [SerializeField] TileCullingSystem tileCullingSystem;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    [ContextMenu("Get Patrols")]
    void GetPatrols()
    {
        patrolDestinations.Clear();
        for (int i = 0; i < patrolPointFolder.childCount; i++)
        {
            patrolDestinations.Add(patrolPointFolder.GetChild(i));
        }
        keySpawns.Clear();
        for (int i = 0; i < keySpawnPointsFolder.childCount; i++)
        {
            keySpawns.Add(keySpawnPointsFolder.GetChild(i));
        }
    }

    public void SpawnKeys()
    {
        // Start Tile Culling System
        tileCullingSystem.Initialise();

        List<Transform> availableSpawns = new(keySpawns);

        for (int i = 0; i < KeyPrefabs.Length; i++)
        {
            if (availableSpawns.Count == 0) break;
            int spawnNum = Random.Range(0, availableSpawns.Count);
            GameObject key = Instantiate(KeyPrefabs[i], availableSpawns[spawnNum].position, Quaternion.identity);
            key.transform.SetParent(keysFolder);
            key.name = "Key" + i;
            availableSpawns.RemoveAt(spawnNum);
        }
        Debug.Log("Keys Spawned");
    }
}
