using UnityEngine;
using UnityEngine.Events;

public class TimerManager : BaseEntity, IPauseEntity
{
    public static event System.Action<int> OnCountDownStarted = null;
    public static event System.Action<int> OnCountDownTimeInProgress = null;
    public static event System.Action OnCountDownOver = null;

    public static event System.Action<float> OnGameTimeStarted = null;
    public static event System.Action OnTimeLimitReached = null;

    [SerializeField]
    private int m_CountDownDelay = 3;

    [SerializeField]
    private float m_GameTime = 60f;

    [SerializeField]
    private UnityEvent OnCountDownStartedEvent = null;

    [SerializeField]
    private UnityEvent OnCountDownOverEvent = null;

    [SerializeField]
    private UnityEvent OnGameTimeStartedEvent = null;

    [SerializeField]
    private UnityEvent OnTimeLimitReachedEvent = null;


    private bool m_IsPaused = false;
    public bool IsPaused
    {
        get { return m_IsPaused; }

    }

    public void OnPause()
    {
        m_IsPaused = true;
    }

    public void OnUnPause()
    {
        m_IsPaused = false;
    }

    private void Awake()
    {
        //we call the unity event from the system actions event,
        //because the unity events can't be classic "events",
        //and we don't want other class to be able to invoke,
        //ours action directly, but only register to them.
        OnCountDownStarted += NotifyOnCountDownStartedEvent;
        OnCountDownOver += NotifyOnCountDownOverEvent;
        OnGameTimeStarted += NotifyOnGameTimeStartedEvent;
        OnTimeLimitReached += NotifyOnTimeLimitReachedEvent;
    }


    private void Start()
    {
        //pause all entity when the scene starts,
        //this avoid the time elapse and the Ai ball
        //throwing.
        PauseGameEntity.Instance.PauseAllEntity();

        //hack to pause all entity except this one.
        //if we don't unpause this entity at the start,
        //the countdown process will not take place.
        PauseGameEntity.Instance.UnPauseEntity(this as IPauseEntity);

        //start countdown routine, before subtrcting
        //the game time. When the method has over
        //his instruction we start to subtract the game 
        //time.
        StartCoroutine(CountDownUpdate());

    }

    private void OnDestroy()
    {
        OnCountDownStarted -= NotifyOnCountDownStartedEvent;
        OnCountDownOver -= NotifyOnCountDownOverEvent;

        OnGameTimeStarted -= NotifyOnGameTimeStartedEvent;
        OnTimeLimitReached -= NotifyOnTimeLimitReachedEvent;
    }

    private void CallUnityEvent(UnityEvent action)
    {
        if(action == null)
        {
            return;
        }

        action.Invoke();
    }

    private void NotifyOnCountDownOverEvent()
    {
        CallUnityEvent(OnCountDownOverEvent);
    }

    private void NotifyOnTimeLimitReachedEvent()
    {
        CallUnityEvent(OnTimeLimitReachedEvent);
    }

    private System.Collections.IEnumerator WaitTime(float timeToWait, System.Action<int> onTimeSubtracted)
    {
        //we don't want to subtract the waiting time,
        //if this instance has been paused.
        while (timeToWait >= 0)
        {
            if (!m_IsPaused)
            {
                timeToWait -= Time.deltaTime;
                CallEvent(onTimeSubtracted, (int)timeToWait);
            }

            yield return null;
        }

    }

    private System.Collections.IEnumerator CountDownUpdate()
    {
        CallEvent(OnCountDownStarted, m_CountDownDelay);

        //start a coroutine to wait before calling count down over,
        //and to start subtracting the game time. every time the time
        //has been subtracted, we invoke an event to inform how much time has
        //been elapsed.
        yield return StartCoroutine(WaitTime(m_CountDownDelay, OnCountDownTimeInProgress));

        CallEvent(OnCountDownOver);

        //we unpause all entity so the game can start normally,
        //after the countdown. if we don't call this method,
        //the game will stay freeze.
        PauseGameEntity.Instance.UnPauseAllEntity();

        StartCoroutine(SubtractGameTime());
    }

    private System.Collections.IEnumerator SubtractGameTime()
    {
        CallEvent(OnGameTimeStarted, m_GameTime);

        //wait untill the game time has elapsed before
        //calling the time reached event. we pass a null
        //callback here because we don't need any UI feedback or
        //stuff like that.
        yield return StartCoroutine(WaitTime(m_GameTime, null));

        CallEvent(OnTimeLimitReached);
    }
    
    private void NotifyOnCountDownStartedEvent(int value)
    {
        OnCountDownStartedEvent.Invoke();
    }

    private void NotifyOnGameTimeStartedEvent(float value)
    {
        OnGameTimeStartedEvent.Invoke();
    }

}
