using UnityEngine;

public enum HandledInputs
{
    LEFT_MOUSE_BUTTON_PRESSED,
    LEFT_MOUSE_BUTTON_HELD_DOWN,
    LEFT_MOUSE_BUTTON_RELEASED,
    KEY_R,
    KEY_D
}

public class InputDispatcher : SingletonEntity<InputDispatcher>
{
    private System.Action[] m_InputsActions = null;

    public void AddListener(InputObserverData data)
    {
        if(data == InputObserverData.DefaultData)
        {
            return;
        }

        m_InputsActions[(int)data.input] += data.method;
    }

    public void RemoveListener(InputObserverData data)
    {
        if (data == InputObserverData.DefaultData)
        {
            return;
        }

        m_InputsActions[(int)data.input] -= data.method;
    }

    protected override void Awake()
    {
        base.Awake();
        m_InputsActions = new System.Action[System.Enum.GetNames(typeof(HandledInputs)).Length];
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            CallInputEvent(HandledInputs.LEFT_MOUSE_BUTTON_PRESSED);
        }

        if(Input.GetMouseButton(0))
        {
            CallInputEvent(HandledInputs.LEFT_MOUSE_BUTTON_HELD_DOWN);
        }

        if(Input.GetMouseButtonUp(0))
        {
            CallInputEvent(HandledInputs.LEFT_MOUSE_BUTTON_RELEASED);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            CallInputEvent(HandledInputs.KEY_R);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            CallInputEvent(HandledInputs.KEY_D);
        }
    }


    private void CallInputEvent(HandledInputs input)
    {
        if(m_InputsActions[(int)input] == null)
        {
            return;
        }

        m_InputsActions[(int)input].Invoke();
    }

}
