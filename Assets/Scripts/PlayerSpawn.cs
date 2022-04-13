using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject [] rockets;

    private void Awake()
    {
        Instantiate(rockets[RaceSelector.shipSelector], transform);
    }
}
