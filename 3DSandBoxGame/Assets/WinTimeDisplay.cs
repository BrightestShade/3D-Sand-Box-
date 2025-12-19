using UnityEngine;
using TMPro;

public class WinTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI bestTimeText;

    void Start()
    {
        float finalTime = StopwatchTimer.finalTime;
        float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        finalTimeText.text = "Time: " + finalTime.ToString("F1") + "s";
        bestTimeText.text = "Best: " + bestTime.ToString("F1") + "s";
    }
}