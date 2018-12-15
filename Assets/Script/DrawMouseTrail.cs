using UnityEngine;

public class DrawMouseTrail : BaseEntity
{
    private TrailRenderer m_TrailRenderer = null;
    private Camera m_MainCamera = null;

    private void Awake()
    {
        base.GetCriticalComponent(out m_TrailRenderer);
        m_MainCamera = Camera.main;
    }

    private void Start()
    {
        UserBallThrower.OnAimingInProgress += DrawTrail;
        UserBallThrower.OnAimingEnded += ResetTrail;
        
    }

    private void OnDestroy()
    {
        UserBallThrower.OnAimingInProgress -= DrawTrail;
        UserBallThrower.OnAimingEnded -= ResetTrail;
    }

    private void ResetTrail()
    {
        m_TrailRenderer.Clear();
    }

    private void DrawTrail(Vector2 mousePosition, float currentForce)
    {
        this.transform.position = m_MainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, m_MainCamera.nearClipPlane + 1));
    }
}
