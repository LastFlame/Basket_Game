using UnityEngine;
using UnityEngine.Events;

public class LastShotManager : BaseEntity
{
    public static event System.Action OnLastShotCompleted = null;

    [SerializeField]
    private UnityEvent OnLastShotCompletedEvent = null;
    

    private int m_EntitiesToWait = 0;
    private int EntitiesToWait
    {
        get { return m_EntitiesToWait; }

        set
        {
            m_EntitiesToWait = value;

            if (m_EntitiesToWait != 0)
            {
                return;
            }

            CallEvent(OnLastShotCompleted);
        }
    }

    private void Awake()
    {
        OnLastShotCompleted += NotifyOnLastShotCompletedEvent;
        TimerManager.OnTimeLimitReached += ValidateLastShot;
    }


    private void OnDestroy()
    {
        TimerManager.OnTimeLimitReached += ValidateLastShot;
        OnLastShotCompleted -= NotifyOnLastShotCompletedEvent;
    }

    private void NotifyOnLastShotCompletedEvent()
    {
        if(OnLastShotCompletedEvent == null)
        {
            return;
        }

        OnLastShotCompletedEvent.Invoke();
    }

    private void OnThrowEntityReadyToBeStopped(ShotStatus status)
    {
        m_EntitiesToWait--;
    }

    private void ValidateLastShot()
    {
        //find every ball thrower entity on the scene,
        //to validate if the last shot had already been,
        //thrown.
        BallThrower[] throwersEntities = FindObjectsOfType<BallThrower>();
        m_EntitiesToWait = throwersEntities.Length;

        System.Action<ShotStatus> OnThrowEntityReady = null;

        foreach(BallThrower thrower in throwersEntities)
        {
            if (thrower.IsReadyToBeStopped)
            {
                EntitiesToWait--;
                continue;
            }

            OnThrowEntityReady = (status) =>
            {
                EntitiesToWait--;
                PauseGameEntity.Instance.PauseEntity(thrower as IPauseEntity);

            };

            thrower.OnThrowEntityReady += OnThrowEntityReady;
        }
    }
}
