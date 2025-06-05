using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Timer : MonoBehaviour
{
    //timer for game modes with time constraints
    public float timer;
    public Text timerText;
    // add options for difficulty
    public virtual void Start()
    {
        if (Optionz.diff != 0)
        {
            this.timer = (float) (this.timer * Optionz.diff);
        }
        this.SetTimerText();
    }

    //
    public virtual void Update()
    {
        if (this.timer > 0)
        {
            this.timer = this.timer - Time.deltaTime;
        }
        this.SetTimerText();
    }

    public virtual void SetTimerText()
    {
        this.timerText.text = "Time Remaining: " + (Mathf.Round(this.timer * 100f) / 100f);
        if (this.timer < 0)
        {
            SceneLoader.GameOver();
        }
    }

    public Timer()
    {
        this.timer = 15f;
    }

}