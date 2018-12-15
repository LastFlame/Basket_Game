using UnityEngine;

public class ShotPowerGUI : BaseEntity
{
    [SerializeField]
    private UnityEngine.UI.Slider m_ShotSlider = null;

    [SerializeField]
    private UnityEngine.UI.Image m_PerfectPowerBar = null;

    private RectTransform m_SliderRectTransform = null;
    private RectTransform m_PerfectPowerBarRectTransform = null;

    private float m_SliderHeight = 0;

    private void Awake()
    {
        m_SliderRectTransform = m_ShotSlider.GetComponent<RectTransform>();
        m_PerfectPowerBarRectTransform = m_PerfectPowerBar.GetComponent<RectTransform>();

        m_SliderHeight = m_SliderRectTransform.sizeDelta.y;
    }

    private void Start()
    {
        UserBallThrower.OnShotDataSet += ResetSlider;
        UserBallThrower.OnAimingInProgress += SetSliderValue;
    }

    private void OnDestroy()
    {
        UserBallThrower.OnShotDataSet -= ResetSlider;
        UserBallThrower.OnAimingInProgress -= SetSliderValue;
    }

    private void ResetSlider(ShotData data)
    {
        m_ShotSlider.minValue = data.forceMinMagnitude;
        m_ShotSlider.maxValue = data.forceMaxMagnitude;
        m_ShotSlider.value = m_ShotSlider.minValue;

        m_PerfectPowerBarRectTransform.anchoredPosition = Vector2.up * (m_SliderHeight * Constant.NormalizedData(data.forceToScoreV3.magnitude, data.forceMinMagnitude, data.forceMaxMagnitude));

    }

    private void SetSliderValue(Vector2 mouseV2,float currentForce)
    {
        m_ShotSlider.value = currentForce;
    }


}
