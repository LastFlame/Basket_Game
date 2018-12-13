using UnityEngine;

public struct BallShotData
{
    public BallThrower thrower;
    public ShotStatus status;

    public static BallShotData Default
    {
        get { return new BallShotData(null, ShotStatus.NONE); }
    }

    public BallShotData(BallThrower thrower, ShotStatus status)
    {
        this.thrower = thrower;
        this.status = status;
    }

}

public class BallEntity : BaseEntity
{
    public static event System.Action<BallShotData> OnScore = null;
    public event System.Action<ShotStatus> OnReadyToBeShot = null;

    private BallShotData m_CurrentThrowScore = BallShotData.Default;
    public ShotStatus CurrentThrowScoreStatus
    {
        get { return m_CurrentThrowScore.status; }

        set { m_CurrentThrowScore.status = value; }
    }


    private Rigidbody m_EntityRb = null;

    public void Shoot(Vector3 force, BallThrower thrower)
    {
        //enable the gravity on the entity when the user decide to stops the interaction with this entity.
        m_EntityRb.useGravity = true;

        //add the stored force to the entity.
        m_EntityRb.AddRelativeForce(force, ForceMode.Impulse);

        m_CurrentThrowScore.thrower = thrower;

        m_CurrentThrowScore.status = ShotStatus.NONE;

        base.LogMessage("ball end force: {0}", force);
        base.LogMessage("balle end magnitude {0}", force.magnitude);



    }

    public void ResetPosition(ShotData data)
    {
        m_EntityRb.useGravity = false;
        m_EntityRb.velocity = Vector3.zero;
        m_EntityRb.angularVelocity = Vector3.zero;
        this.transform.position = data.spotV3;
        this.transform.rotation = data.spotQuat;
    }

    public void OnCollisionWithBasket()
    {
        CallEvent(OnScore, m_CurrentThrowScore);
        CallEvent(OnReadyToBeShot, m_CurrentThrowScore.status);
    }

    public void OnCollisionWithChage()
    {
        CallEvent(OnReadyToBeShot, m_CurrentThrowScore.status);
    }

    private void Awake()
    {
        base.GetCriticalComponent(out m_EntityRb);
    }

    private void Start()
    {
        m_EntityRb.useGravity = false;
        CallEvent(OnReadyToBeShot, m_CurrentThrowScore.status);
    }


}
