using UnityEngine;

public class UserBallThrower : BallThrower
{
    public static event System.Action<ShotData> OnShotDataSet = null;
    public static event System.Action OnAimingStart = null;
    public static event System.Action<Vector2, float> OnAimingInProgress = null;
    public static event System.Action OnAimingEnded = null;


    [SerializeField]
    private float m_MouseOffsetMultiplier = 50f;

    [SerializeField]
    private float m_MaxTimeAiming = 1f;

    private InputObserver m_InputObserver = null;

    private Vector2 m_MouseV2OnMousePressed = Vector3.zero;
    private float m_PreviouseMouseDeltaMagnitude = 0f;
    private float m_CurrentAimingTime = 0f;

    public override void SetNextShot(ShotData data)
    {
        base.SetNextShot(data);

        m_IsReadyToBeStopped = true;
        m_CurrentAimingTime = 0;
        m_CurrentForceV3 = Vector3.zero;

        m_InputObserver.RequireInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_PRESSED, OnLeftMouseButtonPressed),
                                                 new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_RELEASED, OnLeftMouseButtonReleased),
                                                 new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_HELD_DOWN, OnLeftMouseButtonHeldDown));
        CallEvent(OnShotDataSet, data);
    }

    protected override void Awake()
    {
        base.Awake();

        m_InputObserver = new InputObserver();

#if UNITY_EDITOR
        m_InputObserver.RequireInputNotification(new InputObserverData(HandledInputs.KEY_R, ResetShotDebug),
                                                 new InputObserverData(HandledInputs.KEY_D, DebugPerfectShot));
#endif


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

    private void OnLeftMouseButtonPressed()
    {
        if(m_IsPaused)
        {
            return;
        }

        //reset shooting force before the aiming process is started.
        m_CurrentForceV3 = Vector3.zero;

        //store the mouse position when the player start the interaction with the entity.
        //this v2 will be used later to control if the user has moved his mouse on the X/Y axis.
        m_MouseV2OnMousePressed = Input.mousePosition;

        //set a minimun mouse offset to be reached before throwing the ball.
        //this offset allows the user to press on the screen within a range without
        //shooting the ball.
        m_PreviouseMouseDeltaMagnitude = 0.5f;

        //remove left mouse button pressed from the notification of the input manager,
        //because we don't need it anymore until the next shot.
        m_InputObserver.RemoveInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_PRESSED, OnLeftMouseButtonPressed));

        //notify everybody that the aiming process is starting.
        CallEvent(OnAimingStart);
    }

    private void OnLeftMouseButtonHeldDown()
    {
        if(m_IsPaused)
        {
            return;
        }

        //get the time passed between frames and 
        //force the invoke of shoot ball method when the max
        //time to aiming is over, even if the user hasn't
        //released the amiming button.
        //this is a gameplay feature, we want the user to
        //act quckly.
        m_CurrentAimingTime += Time.deltaTime;
        if (m_CurrentAimingTime >= m_MaxTimeAiming)
        {
            OnLeftMouseButtonReleased();
            return;
        }

        //store the position of the mouse on current frame.
        Vector2 mousePosV2 = Input.mousePosition;

        //get the mouse delta position between the current frame and the last frame.
        //this V2 is needed to get the magnitude and prevent the ball force to go backwards.
        //we want the force of the throw to keep increasing every time or remain as it is.
        Vector2 currentMouseDeltaV2 = mousePosV2 - m_MouseV2OnMousePressed;

        //store the magnitude of the mouse delta.
        float currentMouseMagnitude = currentMouseDeltaV2.magnitude;

        //increase the throw force and call aiming in progress, only if the mouse delta,
        //is greater then the last frame. Otherwise the force will decrease and we don't
        //want that.
        if (currentMouseMagnitude > m_PreviouseMouseDeltaMagnitude)
        {
            //avoid the game over manager to invoke change scene,
            //while the player is aiming for the last time.
            if(m_IsReadyToBeStopped)
            {
                m_IsReadyToBeStopped = false;
            }

            //store the previous mouse delta magnitude for the next frame.
            m_PreviouseMouseDeltaMagnitude = currentMouseMagnitude;

            //m_CurrentForceV3 =  (m_PreviouseShotData.forceToScoreV3.normalized * m_PreviousMouseDelta.y) / m_MouseOffsetMultiplier/* + (Vector3.right * (m_PreviousMouseDelta.x / m_MouseOffsetMultiplier))*/;

            //the force to apply on the ball is calcutated by getting the direction of the "perfect force for this shot",
            //multiplied by the "minimum force magnitude" for this shot, plus the overall mouse y delta and mouse x delta
            //(devided by a multiplier to avoid crazy delta numbers).
            m_CurrentForceV3 = m_PreviouseShotData.forceToScoreV3.normalized * (m_PreviouseShotData.forceMinMagnitude + (currentMouseDeltaV2.y / m_MouseOffsetMultiplier));

            //the force magnitude to apply is clamped to a max force for this shot.
            //this allows us to predict where the ball will be on max force, and
            //prevent the force magnitude to be absurd.
            m_CurrentForceV3 = Vector3.ClampMagnitude(m_CurrentForceV3, m_PreviouseShotData.forceMaxMagnitude);

            //this notification isn't outside the if because we don't want to notify,
            //other objects when the magnitude of the force to apply,
            //isn't greated then the previous frame. this "hack" avoid,
            //bad behaviours to UI.
            CallEvent(OnAimingInProgress, mousePosV2, m_CurrentForceV3.magnitude);
        }

    }

    private void OnLeftMouseButtonReleased()
    {
        if(m_IsPaused)
        {
            return;
        }

        //we don't want to throw the ball when the force to apply
        //has not be set. so when the V3 force is zero,
        //we go back to on left mouse button pressed and repeat the process.
        //this check allow the player to navigate the UI without throw the ball.
        if (m_CurrentForceV3 == Vector3.zero)
        {
            m_InputObserver.RequireInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_PRESSED, OnLeftMouseButtonPressed));
            return;
        }

        //remove the input notification form button held down,
        //we don't want the user to keep increasing the ball force,
        //when the aiming process is over.
        m_InputObserver.RemoveInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_HELD_DOWN, OnLeftMouseButtonHeldDown));
        
        //apply the current force to the ball and remove button released notification,
        //thi prevent the user to throw the ball without the aiming process checks.
        base.ThrowEntity(m_CurrentForceV3);
        m_InputObserver.RemoveInputNotification(new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_RELEASED, OnLeftMouseButtonReleased));

        //notify that the aiming process of the user is over.
        CallEvent(OnAimingEnded);
    }

}
