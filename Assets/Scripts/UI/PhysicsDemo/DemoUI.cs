using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoUI : MonoBehaviour
{
    public void OnBackClicked() {
        SceneManager.LoadScene("StartMenu");
    }
}
