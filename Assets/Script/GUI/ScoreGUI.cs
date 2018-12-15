using UnityEngine;
using UnityEngine.UI;

public class ScoreGUI : BaseEntity, IPauseEntity
{
    [SerializeField]
    private Text m_PlayerScore = null;

    [SerializeField]
    private Text m_AIScore = null;

    [SerializeField]
    private Text m_MovingScore = null;

    private bool m_IsPaused = false;
    public bool IsPaused
    {
        get { return m_IsPaused; }
    }

    private RectTransform m_MovingScoreTransform = null;
    private System.Collections.IEnumerator m_OnMovingScore = null;
    private Vector3 m_MovingScoreInitialV2 = Vector3.zero;

    public void OnPause()
    {
        m_IsPaused = true;
    }

    public void OnUnPause()
    {
        m_IsPaused = false;
    }


    private void Awake()
    {
        SetScoreText(m_PlayerScore, "0");
        SetScoreText(m_AIScore, "0");

        m_MovingScoreTransform = m_MovingScore.GetComponent<RectTransform>();
        m_MovingScoreInitialV2 = m_MovingScoreTransform.localPosition;
    }

    private void Start()
    {
        ScoreManager.OnScoreDataSet += SetGUIScore;
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreDataSet -= SetGUIScore;
    }

    private System.Collections.IEnumerator MovingScoreUpdate(string str)
    {
        m_MovingScoreTransform.localPosition = m_MovingScoreInitialV2;
        Vector3 movingScoreDir = new Vector3(0.25f, 1, 0).normalized;

        float maxTime = 0.8f;
        float speed = 100f;

        m_MovingScore.text = Constant.MOVING_SCORE_TEMPLATE.Replace("{0}", str);
        m_MovingScore.enabled = true;

        while (maxTime > 0)
        {
            if(m_IsPaused)
            {
                yield return null;
                continue;
            }

            m_MovingScoreTransform.localPosition += (movingScoreDir * speed * Time.deltaTime);
            maxTime -= Time.deltaTime;
            yield return null;
        }

        m_MovingScore.enabled = false;
    }

    private void SetScoreText(Text component, string str)
    {
        component.text = str;
    }

    private void SetGUIScore(ScoreData data)
    {
        string totalScore = data.nTotalThrowerScore.ToString();

        if(data.thrower.GetType() == typeof(UserBallThrower))
        {
            SetScoreText(m_PlayerScore, totalScore);
            StartCoroutine(m_OnMovingScore = MovingScoreUpdate(data.nCurrentShotScore.ToString()));
            return;
        }

        SetScoreText(m_AIScore,totalScore);

    }


}
