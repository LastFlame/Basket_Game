using UnityEngine;

[RequireComponent(typeof(CollisionEntity))]
public class BasketEntity : BaseEntity
{
    private CollisionEntity m_CollisionEntity = null;

    private void Awake()
    {
        GetCriticalComponent(out m_CollisionEntity);
    }

    private void Start()
    {
        m_CollisionEntity.OnTriggerEvent += NotifyOnCollisionWithBasket;
        m_CollisionEntity.OnCollisionEvent += CollectCollisionInformation;
    }

    private void OnDestroy()
    {
        m_CollisionEntity.OnTriggerEvent -= NotifyOnCollisionWithBasket;
        m_CollisionEntity.OnCollisionEvent -= CollectCollisionInformation;
    }

    private void CollectCollisionInformation(Collision collision)
    {
        BallEntity ball = collision.collider.GetComponent<BallEntity>();

        if(ball == null)
        {
            return;
        }

        ball.CurrentThrowScoreStatus = ball.CurrentThrowScoreStatus | ShotStatus.NORMAL;

    }

    private void NotifyOnCollisionWithBasket(Collider other)
    {
        BallEntity ball = other.GetComponent<BallEntity>();

        if (ball == null)
        {
            return;
        }

        if( (ball.CurrentThrowScoreStatus & ShotStatus.NORMAL) != ShotStatus.NORMAL )
        {
            ball.CurrentThrowScoreStatus = ShotStatus.PERFECT;
        }

        ball.OnCollisionWithBasket();
    }
}
