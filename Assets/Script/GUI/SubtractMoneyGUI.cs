using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class SubtractMoneyGUI : BaseEntity
{
    public event System.Action OnMoneyCountAnimationOver = null;


    [SerializeField]
    private Text m_MoneyCount = null;

    [SerializeField]
    private int m_TotalMoney = 1000;

    [SerializeField]
    private float m_TimeToCompleteAnimation = 5f;

    [SerializeField]
    private UnityEvent OnMoneyCountAnimationOverEvent = null;


    private void Awake()
    {
        if(m_MoneyCount == null)
        {
            LogCriticalError("MONEY COUNT REF ON {0} IS NULL", this.gameObject.name.ToUpper());
            return;
        }

        OnMoneyCountAnimationOver += NotifyOnMoneyCountAnimationOverEvent;
    }

    private void Start()
    {
        m_MoneyCount.text = m_TotalMoney.ToString();

        StartCoroutine(SubtractMoneyUpdate());
    }

    private void OnDestroy()
    {
        OnMoneyCountAnimationOver -= NotifyOnMoneyCountAnimationOverEvent;
    }

    private void NotifyOnMoneyCountAnimationOverEvent()
    {
        if(OnMoneyCountAnimationOverEvent == null)
        {
            return;
        }

        OnMoneyCountAnimationOverEvent.Invoke();
    }

    private System.Collections.IEnumerator SubtractMoneyUpdate()
    {
        int moneySubtraction = (int)(m_TotalMoney / m_TimeToCompleteAnimation);
        int maxMoney = m_TotalMoney;

        while(m_TotalMoney > 0)
        {
            m_TotalMoney = Mathf.Clamp(m_TotalMoney -= (int)(moneySubtraction * Time.deltaTime), 0, maxMoney);
            m_MoneyCount.text = m_TotalMoney.ToString();
            yield return null;
        }


        CallEvent(OnMoneyCountAnimationOver);
    }


}
