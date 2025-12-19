using UnityEngine;
using TMPro;

public class StopwatchTimer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;

    public static float finalTime;   

    private float elapsedTime;
    private bool isRunning;

    void Start()
    {
        elapsedTime = 0f;
        isRunning = true;
        UpdateTimerText(0f);
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerText(elapsedTime);
    }

    private void UpdateTimerText(float time)
    {
        timerText.text = time.ToString("F1");
    }

    public void StopTimer()
    {
        if (!isRunning) return;

        isRunning = false;
        finalTime = elapsedTime;

       
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);

       
        if (finalTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", finalTime);
            PlayerPrefs.Save();
            Debug.Log("New Best Time!");
        }

        Debug.Log("Final Time: " + finalTime);
    }
}