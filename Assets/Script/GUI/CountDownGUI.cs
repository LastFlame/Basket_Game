using UnityEngine;
using UnityEngine.UI;

public class CountDownGUI : BaseEntity
{
    [SerializeField]
    private Text m_ElapsedTime = null;

    [SerializeField]
    private Image m_FrameImage = null;

    private void Awake()
    {
        if(m_ElapsedTime == null)
        {
            LogCriticalError("MISSING ELAPSED TIME REF ON {0}", this.gameObject.name.ToUpper());
            return;
        }

        if(m_FrameImage == null)
        {
            LogCriticalError("MISSING FRAME IMAGE REF ON {0}", this.gameObject.name.ToUpper());
            return;
        }

        m_ElapsedTime.enabled = true;
        m_FrameImage.enabled = true;

        //we change the elasped text when the time manager,
        //is subtracting the time. this entity doesn't know if
        //the game has been paused or not, so we don't subtract,
        //the time here.
        TimerManager.OnCountDownTimeInProgress += SetCountDownText;

        //we gonna hide this UI when the countdown time has elapsed,
        //since we don't need it anymore. 
        TimerManager.OnCountDownOver += HideUI;
    }

    private void OnDestroy()
    {
        TimerManager.OnCountDownTimeInProgress -= SetCountDownText;
        TimerManager.OnCountDownOver -= HideUI;
    }

    private void SetCountDownText(int value)
    {
        m_ElapsedTime.text = value.ToString();
    }

    private void HideUI()
    {
        //we don't disable the game object attached on this script,
        //because we don't if is the same game object of the text
        //and the image.
        m_ElapsedTime.enabled = false;
        m_FrameImage.enabled = false;
    }

}

