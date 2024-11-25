using Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("Timer")]
    float currentTime = 0;
    float startingTime = /*5f * 60f*/ 2f;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] bool isRunning = false;
    bool hasCallPc = false;
    [Header("Manager")]
    PlayerController pc;
    GameManager gm;

    // Start is called before the first frame update
    private void Awake()
    {
        pc = ServiceLocator.Get<GameManager>().GetLocalPlayer();
        pc.SetTimer(this);
        currentTime = startingTime;
    }
    void Start()
    {
       gm = ServiceLocator.Get<GameManager>();
       timerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning) 
        {
            currentTime -= Time.deltaTime;
            timerText.text = GetTime();
            if (currentTime <= 0f && !hasCallPc)
            {
                isRunning = false;
                hasCallPc = true;
                currentTime = 0f;
                if(gm.IsHost())
                    gm.TimesUp();
            }
        }
      
    }
    string GetTime()
    {
        int minute = (int)(currentTime / 60f);
        int second = (int)(currentTime % 60);
        return minute + ":" + second;
    }
    public void StartTimer()
    {
        currentTime = startingTime;
        hasCallPc = false;
        isRunning = true;
    }
    public void StopTimer()
    {
        isRunning = false;
    }
}
