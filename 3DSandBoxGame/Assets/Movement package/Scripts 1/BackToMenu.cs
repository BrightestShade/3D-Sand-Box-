using UnityEngine;
using UnityEngine.SceneManagement;
public class BackToMenu : MonoBehaviour
{
    public void ToMenu()
    {

        SceneManager.LoadSceneAsync(0);
    }

}