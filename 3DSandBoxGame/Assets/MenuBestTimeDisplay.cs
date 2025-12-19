using UnityEngine;
using TMPro;

public class MenuBestTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI bestTimeText;

    void Start()
    {
        if (PlayerPrefs.HasKey("BestTime"))
        {
            float bestTime = PlayerPrefs.GetFloat("BestTime");
            bestTimeText.text = "Best Time: " + bestTime.ToString("F1") + "s";
        }
        else
        {
            bestTimeText.text = "Best Time: --";
        }
    }
}

