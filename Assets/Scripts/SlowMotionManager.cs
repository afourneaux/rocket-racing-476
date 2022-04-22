using UnityEngine;


// Singleton used to change whether the slow motion effect should be enabled or not
public class SlowMotionManager : MonoBehaviour
{
    private static SlowMotionManager Instance;
    private bool slowMotionIsEnabled = true;


    private void Awake()
    {
        if (Instance != null)
        {
            slowMotionIsEnabled = Instance.slowMotionIsEnabled;
        }
        Instance = this;
    }

    public static bool SlowMotionIsEnabled() 
    { 
        return Instance.slowMotionIsEnabled && CountdownController.Instance.getCountdownTime() <= 0.0f; 
    }

    public static void EnableSlowMotion(bool val) { Instance.slowMotionIsEnabled = val; }
}
