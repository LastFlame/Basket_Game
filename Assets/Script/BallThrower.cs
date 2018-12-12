using UnityEngine;

public class BallThrower : BaseEntity
{
    public event System.Action<ShotStatus> OnThrowEntityReady = null;

    [SerializeField]
    protected BallEntity m_ThrowEntity = null;

    protected Vector3 m_CurrentForceV3 = Vector3.zero;
    protected ShotData m_PreviouseShotData = ShotData.Default;

    public virtual void SetNextShot(ShotData data)
    {
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
