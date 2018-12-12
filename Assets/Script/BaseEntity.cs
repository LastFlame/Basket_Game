using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    private Transform m_Transform = null;
    public new Transform transform
    {
        get { return m_Transform ? m_Transform : m_Transform =  this.GetComponent<Transform>(); }
    }

#if UNITY_EDITOR
    [SerializeField]
    private bool m_EnableDebugMessages = false;

    protected void LogCriticalError(string msg, params object[] args)
    {
        Debug.LogErrorFormat(msg, args);
        this.enabled = false;
    }

    protected void LogMessage(string msg, params object[] args)
    {
        if(m_EnableDebugMessages)
        {
            Debug.LogFormat(msg, args);
        }
    }
#endif

    protected void CallEvent(System.Action action)
    {
        if(action == null)
        {
            return;
        }
        action.Invoke();
    }

    protected void CallEvent<T0>(System.Action<T0> action, T0 type)
    {
        if(action == null)
        {
            return;
        }
        action.Invoke(type);
    }

    protected void CallEvent<T1,T2>(System.Action<T1,T2> action, T1 firstType, T2 secondType)
    {
        if (action == null)
        {
            return;
        }
        action.Invoke(firstType, secondType);
    }

    protected void GetCriticalComponent<T>(out T componentRef) where T : Component
    {
        componentRef = this.GetComponentInChildren<T>();

        if(componentRef == null)
        {
            LogCriticalError("{0} CAN'T FIND {1} COMPONENT ON THE GAME OBJECT", this.gameObject.name.ToUpper(), componentRef.GetType().ToString().ToUpper());
        }
    }
}
