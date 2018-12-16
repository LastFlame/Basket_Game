using UnityEngine;
using UnityEngine.UI;

public class WinnerGUI : BaseEntity
{
    public enum Winner
    {
        DRAW,
        PLAYER,
        AI
    }

    private static Winner m_MatchWinner = Winner.PLAYER;

    [SerializeField]
    private Image m_Winner = null;
    

    [SerializeField]
    private RectTransform m_PlayerImageTransform = null;

    [SerializeField]
    private RectTransform m_AiImageTransform = null;

    [SerializeField]
    private Vector3 m_WinnerImageOffset = Vector3.zero;

    private RectTransform m_WinnerRect = null;

    public static void SetWinnerGUI(Winner winner)
    {
        m_MatchWinner = winner;
    }

    private void Start()
    {

        if(m_MatchWinner == Winner.DRAW)
        {
            return;
        }

        m_Winner.enabled = true;

        m_WinnerRect = m_Winner.GetComponent<RectTransform>();

        m_WinnerRect.localPosition = SelectWinnerRect();
       
    }
    
    private Vector3 SelectWinnerRect()
    {
        Vector3 retRect = m_PlayerImageTransform.localPosition;

        if(m_MatchWinner == Winner.AI)
        {
            retRect = m_AiImageTransform.localPosition;
        }

        return retRect;
    }

    private Vector3 SetWinnerImageV3(RectTransform winnerRect)
    {
        Vector3 retV3 = Vector3.zero;

        retV3 = winnerRect.localPosition + Vector3.up * winnerRect.sizeDelta.y;

        retV3 = retV3 + m_WinnerImageOffset;


        return retV3;
    }
}
