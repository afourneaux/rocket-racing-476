using System.Collections.Generic;
using UnityEngine;

// Singleton responsible for holding information about the racers in the current race
public class RacerManager : MonoBehaviour
{
    private static RacerManager Instance;

    private List<Rigidbody> racerRigidbodies = new List<Rigidbody>();

    private void Awake()
    {
        Instance = this;
        racerRigidbodies.Clear(); // Clear previous information
    }

    public static void AddRacer(Rigidbody rb) { Instance.racerRigidbodies.Add(rb); }

    public static List<Rigidbody> GetRacers() { return Instance.racerRigidbodies; }
}
