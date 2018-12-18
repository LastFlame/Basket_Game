using UnityEngine;

public class CameraEntity : BaseEntity
{
    [SerializeField]
    private Transform m_CameraToMove = null;

    [SerializeField]
    private Vector3 m_OffsetOnEndV3 = new Vector3(0, -1f, 2f);

    [SerializeField]
    private Vector3 m_OffSetOnStartV3 = Vector3.zero;

    [SerializeField]
    private float m_Speed = 0;

    private System.Collections.IEnumerator m_OnBallThrownLateUpdate = null;

    private Vector3 m_EndV3 = Vector3.zero;
    private Quaternion m_QuatToReach = Quaternion.identity;

    private float m_EndDistanceToCover = 0f;
    private float m_EndDistanceCovered = 0f;

    private void Awake()
    {
#if UNITY_EDITOR
        if(m_CameraToMove == null)
        {
            base.LogCriticalError("MISSING CAMERA TO MOVE REF ON {0}", this.gameObject.name.ToUpper());
            return;
        }
#endif

        m_OnBallThrownLateUpdate = LateUpdateCoroutine();
        UserBallThrower.OnShotDataSet += OnUserThrowerReadyToShot;
        UserBallThrower.OnAimingEnded += OnBallThrown;
    }

    private void OnDestroy()
    {
        UserBallThrower.OnShotDataSet -= OnUserThrowerReadyToShot;
        UserBallThrower.OnAimingEnded -= OnBallThrown;
    }

    private System.Collections.IEnumerator LateUpdateCoroutine()
    {
        float startTime = Time.time;

        while (m_CameraToMove.position != m_EndV3)
        {
            m_EndDistanceCovered = (Time.time - startTime) * m_Speed;
            float journeyTime = m_EndDistanceCovered / m_EndDistanceToCover;

            m_CameraToMove.localPosition = Vector3.Lerp(m_CameraToMove.localPosition, m_EndV3, journeyTime);
            m_CameraToMove.localRotation = Quaternion.Lerp(m_CameraToMove.localRotation, m_QuatToReach, journeyTime);
            yield return new WaitForEndOfFrame();
        }

        ResetCameraMovementData();
        StopCoroutine(m_OnBallThrownLateUpdate);
    }

    private void ResetCameraMovementData()
    {
        m_EndDistanceCovered = 0;

    }

    private void OnBallThrown()
    {
        StartCoroutine(m_OnBallThrownLateUpdate = LateUpdateCoroutine());
    }

    private void OnUserThrowerReadyToShot(ShotData data)
    {
        StopCoroutine(m_OnBallThrownLateUpdate);
        ResetCameraMovementData();
        //set camera initial offset behind the ball.
        Vector3 dirEndV3ToStartV3 = data.spotV3 - data.targetV3;
        dirEndV3ToStartV3.y = m_OffSetOnStartV3.y;
        dirEndV3ToStartV3.Normalize();
        m_CameraToMove.localPosition = data.spotV3 - (dirEndV3ToStartV3 * (m_OffSetOnStartV3.z));

        //set camera initial rot.
        Vector3 dirCameraToEndV3 = (data.targetV3 - m_CameraToMove.localPosition).normalized;
        dirCameraToEndV3.y = 0;
        m_CameraToMove.forward = dirCameraToEndV3;

        //set camera final offset.
        Vector3 dirEndV3ToCamera = m_CameraToMove.localPosition - data.targetV3;
        dirEndV3ToCamera.y = m_OffsetOnEndV3.y;
        dirEndV3ToCamera.Normalize();
        m_EndV3 = data.targetV3 + (dirEndV3ToCamera * (m_OffsetOnEndV3.z));
        m_EndDistanceToCover = m_EndV3.magnitude;

        //set camera final rot.
        Vector3 localEulerAngles = m_CameraToMove.localRotation.eulerAngles;
        m_QuatToReach = Quaternion.RotateTowards(m_CameraToMove.localRotation, Quaternion.Euler(30f, localEulerAngles.y, localEulerAngles.z), 50f);
    }

}

