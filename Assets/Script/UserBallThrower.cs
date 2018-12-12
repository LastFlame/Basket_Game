using UnityEngine;

public struct ShotData
{
    public Vector3 forceToScoreV3;
    public Vector3 spotV3;
    public Vector3 targetV3;
    public Quaternion spotQuat;
    public float forceMaxMagnitude;
    public float forceMinMagnitude;
    public float spotMaxHeight;

    public static ShotData Default
    {
        get { return new ShotData(Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity, 0, 0, 0); }
    }

    public ShotData(Vector3 forceToScoreV3, Vector3 spotV3, Vector3 targetV3, Quaternion spotQuat, float forceMaxMagnitude, float forceMinMagnitude, float spotMaxHeight)
    {
        this.forceToScoreV3 = forceToScoreV3;
        this.spotV3 = spotV3;
        this.targetV3 = targetV3;
        this.spotQuat = spotQuat;
        this.forceMaxMagnitude = forceMaxMagnitude;
        this.forceMinMagnitude = forceMinMagnitude;
        this.spotMaxHeight = spotMaxHeight;
    }

}

public class UserBallThrower : BallThrower
{
    public static event System.Action<ShotData> OnShotDataSet = null;
    public static event System.Action OnAimingStart = null;
    public static event System.Action OnAimingInProgress = null;
    public static event System.Action OnAimingEnded = null;


    [SerializeField]
    private float m_MouseOffsetMultiplier = 50f;

    [SerializeField]
    private float m_MaxTimeAiming = 1f;

    [Header("DEBUG")]

    [SerializeField]
    private UnityEngine.UI.Slider m_DEBUG_ShootSlider = null;

    private InputObserver m_InputObserver = null;

    private Vector2 m_MouseV2OnMousePressed = Vector3.zero;
    private Vector2 m_PreviousMouseDelta = Vector3.zero;
    private float m_CurrentAimingTime = 0f;

    public override void SetNextShot(ShotData data)
    {
        base.SetNextShot(data);

        m_PreviouseShotData = data;
        m_PreviousMouseDelta = Vector3.zero;
        m_CurrentAimingTime = 0;



        LogMessage("Required force: {0}", data.forceToScoreV3);
        LogMessage("Required force magnitude {0}", data.forceToScoreV3.magnitude);
        LogMessage("Possible max magnitude: {0}", data.forceMaxMagnitude);


        SetPerfectShotSliderV2();

        m_InputObserver.RequireInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_PRESSED, OnLeftMouseButtonPressed),
                                                 new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_RELEASED, OnLeftMouseButtonReleased),
                                                 new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_HELD_DOWN, OnLeftMouseButtonHeldDown));
        CallEvent(OnShotDataSet, data);
    }

    protected override void Awake()
    {
        base.Awake();
        m_InputObserver = new InputObserver(new InputObserverData(HandledInputs.KEY_R, ResetShotDebug),
                                            new InputObserverData(HandledInputs.KEY_D, DebugPerfectShot));

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_InputObserver.RemoveAllInputNotification();
    }

#if UNITY_EDITOR
    private void ResetShotDebug()
    {
        base.RepeatShot();
    }
#endif

#if UNITY_EDITOR
    private void DebugPerfectShot()
    {
        base.ThrowEntity(m_PreviouseShotData.forceToScoreV3);
    }
#endif

    //temp debug.
    private void SetPerfectShotSliderV2()
    {

        float height = m_DEBUG_ShootSlider.GetComponent<RectTransform>().sizeDelta.y; //max value.

        m_DEBUG_ShootSlider.minValue = m_PreviouseShotData.forceMinMagnitude;

        m_DEBUG_ShootSlider.maxValue = m_PreviouseShotData.forceMaxMagnitude;

        m_DEBUG_ShootSlider.transform.Find("Perfect_Shot").GetComponent<RectTransform>().anchoredPosition = Vector2.up * (height * Constant.NormalizedData(m_PreviouseShotData.forceToScoreV3.magnitude, m_PreviouseShotData.forceMinMagnitude, m_PreviouseShotData.forceMaxMagnitude));
        m_DEBUG_ShootSlider.value = m_DEBUG_ShootSlider.minValue;

    }


    private void OnLeftMouseButtonPressed()
    {
        //store the mouse position when the player start the interaction with the entity.
        //this var will be used later to control if the player has moved his mouse on the X axis.
        m_MouseV2OnMousePressed = Input.mousePosition;

        CallEvent(OnAimingStart);

        m_InputObserver.RemoveInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_PRESSED, OnLeftMouseButtonPressed));

    }

    private void OnLeftMouseButtonHeldDown()
    {

        m_CurrentAimingTime += Time.deltaTime;

        if (m_CurrentAimingTime >= m_MaxTimeAiming)
        {
            OnLeftMouseButtonReleased();
            return;
        }

        Vector2 currentMouseDelta = (Vector2)Input.mousePosition - m_MouseV2OnMousePressed;

        if (currentMouseDelta.magnitude > m_PreviousMouseDelta.magnitude)
        {
            //get the diffrence of the mouse position after the throw. 
            m_PreviousMouseDelta = currentMouseDelta;

            //m_CurrentForceV3 =  (m_PreviouseShotData.forceToScoreV3.normalized * m_PreviousMouseDelta.y) / m_MouseOffsetMultiplier/* + (Vector3.right * (m_PreviousMouseDelta.x / m_MouseOffsetMultiplier))*/;

            m_CurrentForceV3 =  m_PreviouseShotData.forceToScoreV3.normalized * (m_PreviouseShotData.forceMinMagnitude + (m_PreviousMouseDelta.y / m_MouseOffsetMultiplier));

            m_CurrentForceV3 = Vector3.ClampMagnitude(m_CurrentForceV3, m_PreviouseShotData.forceMaxMagnitude);


            m_DEBUG_ShootSlider.value = m_CurrentForceV3.magnitude;
        }

        CallEvent(OnAimingInProgress);
    }

    private void OnLeftMouseButtonReleased()
    {
        m_InputObserver.RemoveInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_HELD_DOWN, OnLeftMouseButtonHeldDown));


        base.ThrowEntity(m_CurrentForceV3);

        LogMessage("Final force {0}", m_CurrentForceV3);
        LogMessage("Final magnitude {0}", m_CurrentForceV3.magnitude);


        m_InputObserver.RemoveInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_RELEASED, OnLeftMouseButtonReleased));

        CallEvent(OnAimingEnded);
    }

}
