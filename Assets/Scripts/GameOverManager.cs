using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameOverManager : MonoBehaviour
{
    GameObject[] racerList;
    List<GameObject> finishedRacers;
    // Start is called before the first frame update
    void Start()
    {
        racerList = GameObject.FindGameObjectsWithTag("Racer");
        finishedRacers = new List<GameObject>();
        Debug.Log(racerList.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (finishedRacers.Count == racerList.Length)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Racer" && !finishedRacers.Contains(collision.gameObject))
        {
            finishedRacers.Add(collision.gameObject);
        }
    }
}
