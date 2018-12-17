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
    public static Winner MatchWinner
    {
        get { return m_MatchWinner; }

        set { m_MatchWinner = value; }
    }

    [SerializeField]
    private Text m_Victory = null;
    

    [SerializeField]
    private RectTransform m_PlayerImageTransform = null;

    [SerializeField]
    private RectTransform m_AiImageTransform = null;

    [SerializeField]
    private Vector3 m_WinnerImageOffset = Vector3.zero;

    private RectTransform m_VictoryRect = null;


    private void Start()
    {
        if(m_MatchWinner == Winner.DRAW)
        {
            return;
        }

        m_Victory.enabled = true;

        m_VictoryRect = m_Victory.GetComponent<RectTransform>();

        RectTransform winnerRect = SelectWinnerRect();

        m_VictoryRect.localPosition = SetWinnerImageV3(winnerRect);
       
    }
    
    private RectTransform SelectWinnerRect()
    {
        RectTransform selectedRectTransform = m_PlayerImageTransform;

        if(m_MatchWinner == Winner.AI)
        {
            selectedRectTransform = m_AiImageTransform;
        }

        return selectedRectTransform;
    }

    private Vector3 SetWinnerImageV3(RectTransform winnerRect)
    {
        Vector3 retV3 = Vector3.zero;

        retV3 = winnerRect.localPosition + Vector3.up * winnerRect.sizeDelta.y / 2f;
        retV3 = retV3 + m_WinnerImageOffset;
        
        return retV3;
    }
}
