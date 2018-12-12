using UnityEngine;

public class ShootingSpotEntity : BaseEntity
{
    [SerializeField]
    float m_MaxMagnitudeOffSet = 0;

    [SerializeField]
    float m_MinMagnitudeOffset = 0;


    [SerializeField]
    private float m_CurveHeight = 3;

    public ShotData GetShotData(Vector3 destinationPoint)
    {
        Vector3 lookAtV3 = destinationPoint - this.transform.position;
        lookAtV3.y = 0;
        this.transform.forward = lookAtV3;

        Vector3 perfectVelocityV3 = Constant.GetProjectileVelocity(destinationPoint, this.transform.position, m_CurveHeight);

        float maxMagnitude = perfectVelocityV3.magnitude + m_MaxMagnitudeOffSet;

        float minMagnitude = perfectVelocityV3.magnitude - m_MinMagnitudeOffset;

        return new ShotData(perfectVelocityV3, this.transform.position, destinationPoint, this.transform.rotation, maxMagnitude, minMagnitude, m_CurveHeight);
    }

}

