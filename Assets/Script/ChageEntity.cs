using UnityEngine;

public class ChageEntity : BaseEntity
{
    private CollisionEntity m_CollisionEntity = null;

    private void Awake()
    {
        base.GetCriticalComponent(out m_CollisionEntity);
    }

    private void Start()
    {
        m_CollisionEntity.OnCollisionEvent += OnChageCollision;
    }

    private void OnChageCollision(Collision collision)
    {
        BallEntity ball = collision.collider.GetComponent<BallEntity>();

        if(ball == null)
        {
            return;
        }

        ball.CurrentThrowScoreStatus = ShotStatus.FAILED;

        ball.OnCollisionWithChage();
    }

}
