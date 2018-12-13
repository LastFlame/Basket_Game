using UnityEngine;
using UnityEngine.UI;

public class GameSceneGUIManager : BaseEntity
{
    [SerializeField]
    private Text m_PlayerScore = null;

    [SerializeField]
    private Text m_AIScore = null;


    private void Awake()
    {
        SetScoreText(m_PlayerScore, "0");
        SetScoreText(m_AIScore, "0");
    }

    private void Start()
    {
        ScoreManager.OnScoreDataSet += SetGUIScore;
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreDataSet -= SetGUIScore;
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
            return;
        }

        SetScoreText(m_AIScore,totalScore);

    }
}
