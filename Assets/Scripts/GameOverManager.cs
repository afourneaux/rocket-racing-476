using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameOverManager : MonoBehaviour
{
    List<Rigidbody> finishedRacers;
    public GameObject gameOverScreen;
    // Start is called before the first frame update
    void Start()
    {
        finishedRacers = new List<Rigidbody>();
  
    }

    // Update is called once per frame
    void Update()
    {
        if (finishedRacers.Count == RacerManager.GetRacers().Count)
        {
            gameOverScreen.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Racer" && !finishedRacers.Contains(collision.rigidbody))
        {
            finishedRacers.Add(collision.rigidbody);
        }
    }
}
