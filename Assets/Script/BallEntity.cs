using UnityEngine;

public struct BallShotData
{
    public BallThrower thrower;
    public ShotStatus status;
    public bool isOnFire;

    public static BallShotData Default
    {
        get { return new BallShotData(null, ShotStatus.NONE, false); }
    }

    public BallShotData(BallThrower thrower, ShotStatus status, bool isOnFire)
    {
        this.thrower = thrower;
        this.status = status;
        this.isOnFire = isOnFire;
    }

}

public class BallEntity : BaseEntity, IPauseEntity
{
    public static event System.Action<BallShotData> OnScore = null;
    public event System.Action<ShotStatus> OnReadyToBeShot = null;

    private BallShotData m_CurrentThrowScore = BallShotData.Default;
    public ShotStatus CurrentThrowScoreStatus
    {
        get { return m_CurrentThrowScore.status; }

        set { m_CurrentThrowScore.status = value; }
    }

    private bool m_IsPaused = false;
    public bool IsPaused
    {
        get { return m_IsPaused; }
    }

    private Rigidbody m_EntityRb = null;
    private MeshRenderer m_Renderer = null;
    private Material m_DefaultMaterial = null;

    private Vector3 m_BeforePauseVelocity = Vector3.zero;
    private Vector3 m_BeforePauseAngularVelocity = Vector3.zero;

    private bool m_BeforePauseGravityUse = false;

    public void OnPause()
    {
        m_IsPaused = true;

        //meaning the ball hasn't be thorwn.
        if(!m_EntityRb.useGravity)
        {
            m_BeforePauseGravityUse = false;
            return;
        }

        m_BeforePauseVelocity = m_EntityRb.velocity;
        m_BeforePauseAngularVelocity = m_EntityRb.angularVelocity;
        m_BeforePauseGravityUse = true;
        m_EntityRb.useGravity = false;
        m_EntityRb.velocity = Vector3.zero;
    }

    public void OnUnPause()
    {
        m_IsPaused = false;

        //don't reset rb parameters if the status before,
        //the pause was still.
        if(!m_BeforePauseGravityUse)
        {
            return;
        }

        m_EntityRb.useGravity = true;
        m_EntityRb.velocity = m_BeforePauseVelocity;
        m_EntityRb.angularVelocity = m_BeforePauseAngularVelocity;
    }

    public void Shoot(Vector3 force, BallThrower thrower, bool isOnFireEffect)
    {
        //enable the gravity on the entity when the user decide to stops the interaction with this entity.
        m_EntityRb.useGravity = true;

        //add the stored force to the entity.
        m_EntityRb.AddRelativeForce(force, ForceMode.Impulse);

        m_CurrentThrowScore.thrower = thrower;
        m_CurrentThrowScore.status = ShotStatus.NONE;
        m_CurrentThrowScore.isOnFire = isOnFireEffect;
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

    public void SetMaterial(Material material)
    {
        m_Renderer.material = material;
    }

    public void ResetDefaultMaterial()
    {
        m_Renderer.material = m_DefaultMaterial;
    }

    private void Awake()
    {
        base.GetCriticalComponent(out m_EntityRb);
        base.GetCriticalComponent(out m_Renderer);
        m_DefaultMaterial = m_Renderer.material;
    }

    private void Start()
    {
        m_EntityRb.useGravity = false;
        CallEvent(OnReadyToBeShot, m_CurrentThrowScore.status);

    }


}
