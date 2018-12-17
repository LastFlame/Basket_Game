using UnityEngine;
using UnityEngine.UI;

public class FireBallGUI : BaseEntity, IPauseEntity
{
    [SerializeField]
    private BallThrower m_ThrowerEntity = null;

    [SerializeField]
    private Slider m_FireBallSlider = null;

    [SerializeField]
    private int m_ScoreToSetFireBallStatus = 12;

    [SerializeField]
    private float m_DecreaseSliderValueOnNormalStatus = 0.5f;

    [SerializeField]
    private float m_DecreaseSliderValueOnFireStatus = 1f;

    [SerializeField]
    private Material m_OnFireBallMaterial = null;

    private bool m_IsPaused = false;
    public bool IsPaused
    {
        get { return m_IsPaused; }
    }

    private System.Collections.IEnumerator m_DecreaseSliderOnNormalEffect = null;
    private System.Collections.IEnumerator m_DecreaseSliderOnFireEffect = null;

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
        m_FireBallSlider.maxValue = m_ScoreToSetFireBallStatus;
        m_FireBallSlider.value = 0;
    }

    private void Start()
    {
        ScoreManager.OnScoreDataSet += IncreaseSliderValue;
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreDataSet -= IncreaseSliderValue;
    }


    private System.Collections.IEnumerator DecreaseSliderValue(float decreaseMultiplier, System.Action callback)
    {
        while (m_FireBallSlider.value > 0)
        {
            m_FireBallSlider.value -= decreaseMultiplier * Time.deltaTime;
            yield return null;
        }

        CallEvent(callback);
    }

    private void InterruptCoroutine(ref System.Collections.IEnumerator coroutine)
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private void ShotFailedOnFireEffectActive(ShotStatus status)
    {
        if(status != ShotStatus.FAILED)
        {
            return;
        }

        
        InterruptCoroutine(ref m_DecreaseSliderOnFireEffect);
        InterruptOnFireEffect();

    }

    private void InterruptOnFireEffect()
    {
        m_ThrowerEntity.OnThrowEntityReady -= ShotFailedOnFireEffectActive;
        m_ThrowerEntity.DisableOnFireEffect();
        m_FireBallSlider.value = 0;
    }

    private void IncreaseSliderValue(ScoreData data)
    {
        //verify if the current score entity is the same
        //as this instanc reference.
        if(data.thrower != m_ThrowerEntity)
        {
            return;
        }

        //we don't want to do nothing else if the current
        //internal status is "ball on fire". while this
        //status is activated we just wait untill the 
        //slider value has reached 0.
        if(m_DecreaseSliderOnFireEffect != null)
        {
            return;
        }

        float sliderNextValue = m_FireBallSlider.value + data.nCurrentShotScore;
        m_FireBallSlider.value = sliderNextValue;

        //we wan't to activate the status "ball on fire" if
        //the current thrower has reached the max value of
        //the slider.
        if(sliderNextValue >= m_FireBallSlider.maxValue)
        {
            //activation of the fire stauts on the thrower when
            //the slider has reached the max value.
            //this effect multiply the score of the thrower.
            m_ThrowerEntity.ActivateOnFireEffect(m_OnFireBallMaterial);

            m_ThrowerEntity.OnThrowEntityReady += ShotFailedOnFireEffectActive;

            StartCoroutine(m_DecreaseSliderOnFireEffect = DecreaseSliderValue(m_DecreaseSliderValueOnFireStatus, () =>
            {
                //we deactivate the fire effect on decrease slider value callback (when is over).
                m_DecreaseSliderOnFireEffect = null;

                InterruptOnFireEffect();
            }));

            //we stop the normal decrease value coroutine, because we don't
            //need it while the ball is on fire.
            InterruptCoroutine(ref m_DecreaseSliderOnNormalEffect);
            return;
        }

        //we don't want to activate 2 times decrease slider value on 
        //normal status.
        if(m_DecreaseSliderOnNormalEffect != null)
        {
            return;
        }

        StartCoroutine(m_DecreaseSliderOnNormalEffect = DecreaseSliderValue(m_DecreaseSliderValueOnNormalStatus, () =>
        {
            //when the value of the slider has reached zero we stop the coroutine, we don't need it anymore.
            m_DecreaseSliderOnNormalEffect = null;
        }));
    }
}