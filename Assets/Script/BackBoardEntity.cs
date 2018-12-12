using UnityEngine;

[RequireComponent(typeof(CollisionEntity))]
public class BackBoardEntity : BaseEntity
{
    private static Constant.ShotScores m_CurrentScoreBonus = Constant.ShotScores.NORMAL_BONUS;
    public static Constant.ShotScores GetCurrentScoreBonus
    {
        get { return m_CurrentScoreBonus; }
    }

    [SerializeField]
    private float m_ChanceOfBonus = 0.1f;

    [SerializeField]
    private SpriteRenderer m_BonusMultiplierSprite = null;

    [SerializeField]
    private SpriteRenderer m_X2ScoreBonusSprite = null;

    [SerializeField]
    private SpriteRenderer m_X2_5ScoreBonusSprite = null;

    [SerializeField]
    private float m_BonusMultiplierSpriteBlinkSpeed = 0.5f;

    private CollisionEntity m_CollisionEntity = null;
    private SpriteRenderer m_CurrentBonusSprite = null;

    private System.Collections.IEnumerator OnBonusActiveUpdate = null;
    private bool m_ScoreBonusIsActive = false;


    private void Awake()
    {
        GetCriticalComponent(out m_CollisionEntity);

        if(m_ChanceOfBonus >= 0f && m_ChanceOfBonus <= 1f)
        {
            return;
        }

        //safe fall back to avoid weird behaviours on BounusChance mehtod.
        m_ChanceOfBonus = Mathf.Clamp01(m_ChanceOfBonus);
    }

    private void Start()
    {
        m_CollisionEntity.OnCollisionEvent += CollectCollisionInformation;
        BallEntity.OnScore += BonusChance;
    }

    private void OnDestroy()
    {
        m_CollisionEntity.OnCollisionEvent -= CollectCollisionInformation;
    }

    private System.Collections.IEnumerator BonusActiveUpdate()
    {

        Color bonusMultiplierSpriteColor = m_BonusMultiplierSprite.color;
        bonusMultiplierSpriteColor.a = 1f;
        m_BonusMultiplierSprite.color = bonusMultiplierSpriteColor;
        m_CurrentBonusSprite.color = bonusMultiplierSpriteColor;

        while (true)
        {
            bonusMultiplierSpriteColor = m_BonusMultiplierSprite.color;

            if (bonusMultiplierSpriteColor.a >= 1 || bonusMultiplierSpriteColor.a <= 0)
            {
                m_BonusMultiplierSpriteBlinkSpeed *= -1;
            }

            bonusMultiplierSpriteColor.a += m_BonusMultiplierSpriteBlinkSpeed * Time.deltaTime;

            m_BonusMultiplierSprite.color = bonusMultiplierSpriteColor;
            m_CurrentBonusSprite.color = bonusMultiplierSpriteColor;

            yield return null;
        }

    }

    private void CollectCollisionInformation(Collision collision)
    {
        BallEntity ball = collision.collider.GetComponent<BallEntity>();

        if (ball == null)
        {
            return;
        }

        ball.CurrentThrowScoreStatus = m_ScoreBonusIsActive ? ball.CurrentThrowScoreStatus | ShotStatus.BONUS | ShotStatus.NORMAL : ShotStatus.NORMAL;
    }

    private void BonusChance(BallScoreData scoreData)
    {
        if (!m_ScoreBonusIsActive && m_ChanceOfBonus >= Random.Range(0f, 1f))
        {
            m_ScoreBonusIsActive = true;

            if (Random.Range(0f, 1f) <= 0.8f)
            {
                m_CurrentScoreBonus = Constant.ShotScores.NORMAL_BONUS;
                m_CurrentBonusSprite = m_X2ScoreBonusSprite;
            }
            else
            {
                m_CurrentScoreBonus = Constant.ShotScores.EXTRA_BONUS;
                m_CurrentBonusSprite = m_X2_5ScoreBonusSprite;
            }

            StartCoroutine(OnBonusActiveUpdate = BonusActiveUpdate());
            return;
        }

        if (scoreData.status == (ShotStatus.BONUS | ShotStatus.NORMAL))
        {
            StopCoroutine(OnBonusActiveUpdate);

            Color boardColor = m_BonusMultiplierSprite.color;
            boardColor.a = 0;
            m_BonusMultiplierSprite.color = boardColor;
            m_CurrentBonusSprite.color = boardColor;
            m_ScoreBonusIsActive = false;
        }

    }


}
