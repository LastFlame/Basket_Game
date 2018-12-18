using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoneyGUI : BaseEntity
{
    [SerializeField]
    private Text m_Money = null;


    private void Start()
    {
        DisplayCurrentMoney();
    }

    public void DisplayCurrentMoney()
    {
        m_Money.text = Constant.PLAYER_MONEY.ToString();
    }

}
