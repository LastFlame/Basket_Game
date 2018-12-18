using UnityEngine;

public class ShootingSpotsGroup : BaseEntity
{
    [SerializeField]
    private BallThrower m_BallThrower = null;

    [SerializeField]
    private Transform m_TrDestinationPoint = null;

    private ShootingSpotEntity[] m_ShootingSpots = null;
    private int m_SpotsCounter = 0;



    private void Awake()
    {
#if UNITY_EDITOR
        if (m_TrDestinationPoint == null)
        {
            LogCriticalError("TARGET DESTINATION ON {0} IS NULL", this.gameObject.name.ToUpper());
            return;
        }

        if (m_BallThrower == null)
        {
            LogCriticalError("BALL THROWER ON {0} IS NULL", this.gameObject.name.ToUpper());
            return;
        }
#endif

        m_ShootingSpots = transform.GetComponentsInChildren<ShootingSpotEntity>();
        m_BallThrower.OnThrowEntityReady += SetThrowParameters;

    }

    private void SetThrowParameters(ShotStatus stauts)
    {
        if(stauts == ShotStatus.FAILED)
        {
            m_BallThrower.RepeatShot();
            return;
        }

        ShotData currentThrowData = m_ShootingSpots[m_SpotsCounter].GetShotData(m_TrDestinationPoint.position);
        m_BallThrower.SetNextShot(currentThrowData);
        m_SpotsCounter = m_SpotsCounter + 1 <= m_ShootingSpots.Length - 1 ? m_SpotsCounter + 1 : 0;
    }
}
