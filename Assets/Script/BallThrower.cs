using UnityEngine;

public struct ShotData
{
    public Vector3 forceToScoreV3;
    public Vector3 spotV3;
    public Vector3 targetV3;
    public Quaternion spotQuat;
    public float forceMaxMagnitude;
    public float forceMinMagnitude;
    public float spotMaxHeight;

    public static ShotData Default
    {
        get { return new ShotData(Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity, 0, 0, 0); }
    }

    public ShotData(Vector3 forceToScoreV3, Vector3 spotV3, Vector3 targetV3, Quaternion spotQuat, float forceMaxMagnitude, float forceMinMagnitude, float spotMaxHeight)
    {
        this.forceToScoreV3 = forceToScoreV3;
        this.spotV3 = spotV3;
        this.targetV3 = targetV3;
        this.spotQuat = spotQuat;
        this.forceMaxMagnitude = forceMaxMagnitude;
        this.forceMinMagnitude = forceMinMagnitude;
        this.spotMaxHeight = spotMaxHeight;
    }

}

public class BallThrower : BaseEntity, IPauseEntity
{
    public event System.Action<ShotStatus> OnThrowEntityReady = null;

    [SerializeField]
    protected BallEntity m_ThrowEntity = null;

    protected Vector3 m_CurrentForceV3 = Vector3.zero;
    protected ShotData m_PreviouseShotData = ShotData.Default;

    protected bool m_IsPaused = false;
    public bool IsPaused
    {
        get { return m_IsPaused; }

    }

    protected bool m_IsReadyToBeStopped = true;
    public bool IsReadyToBeStopped
    {
        get { return m_IsReadyToBeStopped; }
    }

    public virtual void OnPause()
    {
        m_IsPaused = true;
    }

    public virtual void OnUnPause()
    {
        m_IsPaused = false;
    }

    public virtual void SetNextShot(ShotData data)
    {
        m_PreviouseShotData = data;
        m_CurrentForceV3 = Vector3.zero;
        m_ThrowEntity.ResetPosition(data);

    }

    public virtual void RepeatShot()
    {
        SetNextShot(m_PreviouseShotData);
    }

    protected virtual void Awake()
    {
        m_ThrowEntity.OnReadyToBeShot += CallThrowerReady;
    }

    protected virtual void OnDestroy()
    {
        m_ThrowEntity.OnReadyToBeShot -= CallThrowerReady;
    }

    protected void ThrowEntity(Vector3 force)
    {
        m_ThrowEntity.Shoot(force, this);
    }

    protected void CallThrowerReady(ShotStatus status)
    {
        CallEvent(OnThrowEntityReady, status);
    }
}
