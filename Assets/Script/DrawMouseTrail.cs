using UnityEngine;

public class DrawMouseTrail : BaseEntity
{
    private TrailRenderer m_TrailRenderer = null;

    private void Awake()
    {
        base.GetCriticalComponent(out m_TrailRenderer);
    }

    private void Start()
    {
        UserBallThrower.OnAimingInProgress += DrawTrail;
        UserBallThrower.OnAimingStart += () => 
        { this.transform.position = Camera.main.transform.position + Vector3.forward * 2; };
        
    }


    private void DrawTrail(Vector2 mousePosition)
    {
        Camera cam = Camera.main;
        Vector3 mouse = Input.mousePosition;
        

        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, cam.nearClipPlane + 1));

        this.transform.localPosition = point;
        
    }
}
