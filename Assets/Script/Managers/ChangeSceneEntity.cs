using UnityEngine;

public class ChangeSceneEntity : BaseEntity
{
    [SerializeField]
    private Constant.GameScenes m_NextScene = Constant.GameScenes.MAIN_MENU;

    public void RequestSceneTransition()
    {
        SceneTransitionManager.Instance.StartSceneTransition(m_NextScene);
    }
}
