using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    // Start is called before the first frame update
   
    private int timeLeft;
    private int periodLast;
    private float startTime;
    private Text text;
    private bool isGameRunning;
    void Awake()
    {
        text = gameObject.GetComponent<Text>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void setTime(int seconds)
    {
        periodLast = seconds;
        timeLeft = seconds;
        startTime = Time.fixedTime;
        text.text = timeLeft.ToString() + "S";
        isGameRunning = true;
    }

    void FixedUpdate()
    {
        if (isGameRunning)
        {
            int nowTimeLeft = periodLast - (int)(Time.fixedTime - startTime);
            if (nowTimeLeft != timeLeft)
            {
                timeLeft = periodLast - (int)(Time.fixedTime - startTime);
                text.text = timeLeft.ToString() + "S";
            }
        }
    }

}
