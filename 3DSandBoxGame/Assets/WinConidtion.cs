using UnityEngine;
 using UnityEngine.SceneManagement;
public class WinConidtion : MonoBehaviour
{
 
    public StopwatchTimer stopwatch;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stopwatch.StopTimer();
            SceneManager.LoadScene(2);
        }
    }
     
}

