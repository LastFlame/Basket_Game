using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class SubtractMoneyGUI : BaseEntity
{
    public event System.Action OnMoneyCountAnimationOver = null;

    [SerializeField]
    private Text m_MoneyCount = null;

    [SerializeField]
    private float m_TimeToCompleteAnimation = 5f;

    [SerializeField]
    private UnityEvent OnMoneyCountAnimationOverEvent = null;


    private void Awake()
    {
#if UNITY_EDITOR
        if (m_MoneyCount == null)
        {
            LogCriticalError("MONEY COUNT REF ON {0} IS NULL", this.gameObject.name.ToUpper());
            return;
        }
#endif
        OnMoneyCountAnimationOver += NotifyOnMoneyCountAnimationOverEvent;
    }

    private void Start()
    {
        m_MoneyCount.text = Constant.BET_MONEY.ToString();

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
        int moneySubtraction = (int)(Constant.BET_MONEY / m_TimeToCompleteAnimation);
        int maxMoney = Constant.BET_MONEY;
        float currentAnimMoney = maxMoney;

        while(currentAnimMoney > 0)
        {
            currentAnimMoney = Mathf.Clamp(currentAnimMoney -= (int)(moneySubtraction * Time.deltaTime), 0, maxMoney);
            m_MoneyCount.text = currentAnimMoney.ToString();
            yield return null;
        }


        CallEvent(OnMoneyCountAnimationOver);
    }


}
