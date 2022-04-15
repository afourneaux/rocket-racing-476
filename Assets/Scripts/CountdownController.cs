using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    private static float countdownTime = 3f;
    public Text countdownDisplay;

    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        while (countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        countdownDisplay.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownDisplay.gameObject.SetActive(false); 
    }
    public static float getCountdownTime()
    {
        return countdownTime;
    }
}
