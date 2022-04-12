using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceSelector : MonoBehaviour
{
    private int trackSelector = 0;
    private int shipSelector = 0;
    
    public void StartGame() 
    {
        SceneManager.LoadScene(1);
    }

    public void setTrack(int Selector)
    {
        trackSelector = Selector;
        Debug.Log("set track " + trackSelector);
    }
    
    public void setShip(int Selector)
    {
        shipSelector = Selector;
        Debug.Log("set ship " + shipSelector);
    }

    public void StartRace()
    {
        switch (trackSelector)
        {
            case 0 :
                SceneManager.LoadScene("CanyonRaceTrack");
                break;
            case 1 :
                SceneManager.LoadScene("MountainRacetrack");
                break;
        }
    }
}