using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    // PLAY BUTTON → loads scene index 0
    public void PlayGame()
    {
        SceneManager.LoadScene(0);
    }

    // QUIT BUTTON
    public void QuitGame()
    {
        Application.Quit();

        // Allows quit to work in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
