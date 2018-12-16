using System.Collections.Generic;
using System.Linq;

public struct InputObserverData
{
    public static InputObserverData DefaultData
    {
        get { return new InputObserverData(HandledInputs.LEFT_MOUSE_BUTTON_PRESSED, null); }
    }

    public static bool operator ==(InputObserverData first, InputObserverData second)
    {
        return (first.input == second.input) && (first.method == second.method);
    }

    public static bool operator !=(InputObserverData first, InputObserverData second)
    {
        return (first.input == second.input) && (first.method == second.method);
        
    }

    public HandledInputs input;
    public System.Action method;

    public InputObserverData(HandledInputs input, System.Action method)
    {
        this.input = input;
        this.method = method;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}

public class InputObserver
{
    private LinkedList<InputObserverData> m_InputObserverDatas = null;

    public InputObserver()
    {
        m_InputObserverDatas = new LinkedList<InputObserverData>();
    }

    public InputObserver(params InputObserverData[] datas)
    {
        m_InputObserverDatas = new LinkedList<InputObserverData>();

        RequireInputNotification(datas);
    }

    public void RequireInputNotification(InputObserverData data)
    {
        if(data.method == null)
        {
            return;
        }

        InputDispatcher.Instance.AddListener(data);

        m_InputObserverDatas.AddLast(data);
    }

    public void RequireInputNotification(params InputObserverData[] datas)
    {
        for(int i = 0; i < datas.Length; ++i)
        {
            RequireInputNotification(datas[i]);
        }
    }

    public void RemoveInputNotification(InputObserverData data)
    {
        if(data.method == null)
        {
            return;
        }

        InputObserverData dataToRemove = m_InputObserverDatas
                                         .FirstOrDefault(storedNotification => storedNotification == data);

        if(dataToRemove == InputObserverData.DefaultData)
        {
            return;
        }

        m_InputObserverDatas.Remove(dataToRemove);
        InputDispatcher.Instance.RemoveListener(dataToRemove);
        
    }

    public void RemoveAllInputNotification()
    {
        foreach(InputObserverData data in m_InputObserverDatas)
        {
            InputDispatcher.Instance.RemoveListener(data);
        }

        m_InputObserverDatas = null;

    }

}
