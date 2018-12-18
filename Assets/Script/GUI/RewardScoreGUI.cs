using UnityEngine;
using UnityEngine.UI;

public class RewardScoreGUI : BaseEntity
{
    [SerializeField]
    private Text m_Victory = null;
    
    [SerializeField]
    private RectTransform m_PlayerImageTransform = null;

    [SerializeField]
    private Text m_PlayerScore = null;

    [SerializeField]
    private RectTransform m_AiImageTransform = null;

    [SerializeField]
    private Text m_AiScore = null;

    [SerializeField]
    private Vector3 m_WinnerImageOffset = Vector3.zero;

    private void Start()
    {
        //retrive the previous match score from the score manager.
        //score manager gameobject is inside shooting race scene,
        //and it update the scores of the entites.
        ScoreManager.Score lastMatchScore = ScoreManager.RetriveMatchScore();

        int nAiScore = lastMatchScore.AIScore;
        int nPlayerScore = lastMatchScore.playerScore;

        SetScores(nPlayerScore, nAiScore);

        RectTransform victoryRect = m_Victory.GetComponent<RectTransform>();

        m_Victory.enabled = true;

        if (nAiScore > nPlayerScore)
        {
            victoryRect.localPosition = SetWinnerImageV3(m_AiImageTransform);
            return;
        }

        if(nAiScore < nPlayerScore)
        {
            victoryRect.localPosition = SetWinnerImageV3(m_PlayerImageTransform);

            //temp hack
            Constant.PLAYER_MONEY += Constant.BET_MONEY;
            return;
        }


        m_Victory.text = "DRAW!";
    }

    private void SetScores(int playerScore, int aiScore)
    {
        m_PlayerScore.text = playerScore.ToString();
        m_AiScore.text = aiScore.ToString();

    }

    private Vector3 SetWinnerImageV3(RectTransform winnerRect)
    {
        Vector3 retV3 = Vector3.zero;

        retV3 = winnerRect.localPosition + Vector3.up * winnerRect.sizeDelta.y / 2f;
        retV3 = retV3 + m_WinnerImageOffset;
        
        return retV3;
    }
}
