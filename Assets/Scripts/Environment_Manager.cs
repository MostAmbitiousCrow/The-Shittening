using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment_Manager : MonoBehaviour
{
    public static Environment_Manager instance;
    public Transform patrolPointFolder;
    public List<Transform> patrolDestinations { get; private set; }

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
    }
}
