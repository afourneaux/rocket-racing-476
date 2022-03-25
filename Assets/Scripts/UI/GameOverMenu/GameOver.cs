using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
   public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
