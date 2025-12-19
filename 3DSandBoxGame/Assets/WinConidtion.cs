using UnityEngine;
using UnityEngine.SceneManagement;

public class WinConidtion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopwatchTimer timer = FindObjectOfType<StopwatchTimer>();
            if (timer != null)
            {
                timer.StopTimer();
            }

            SceneManager.LoadScene(2);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}