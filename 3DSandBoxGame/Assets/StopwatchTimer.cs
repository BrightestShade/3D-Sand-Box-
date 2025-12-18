using UnityEngine;
using TMPro;

public class StopwatchTimer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;
    

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
        timerText.text = time.ToString("F1"); // 1 decimal place
    }

    public void StopTimer()
    {
        if (!isRunning) return;

        isRunning = false;
        Debug.Log("GameWin");
    }


}