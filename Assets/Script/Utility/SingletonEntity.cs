public class SingletonEntity<T> : BaseEntity where T : BaseEntity
{
    private static T m_Instance = null;
    public static T Instance
    {
        get { return m_Instance; }
    }

    protected virtual void Awake()
    {
        if(m_Instance == null)
        {
            m_Instance = GetComponent<T>();
            DontDestroyOnLoad(this.gameObject);
            return;
        }

        Destroy(this.gameObject);
    }
}
