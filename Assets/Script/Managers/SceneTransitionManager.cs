using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitionManager : SingletonEntity<SceneTransitionManager>
{
    private Constant.GameScenes m_CurrentScene = Constant.GameScenes.MAIN_MENU;
    private bool m_IsExecutingSceneLoading = false;


    private void Start()
    {
        m_CurrentScene = (Constant.GameScenes)SceneManager.GetActiveScene().buildIndex;
    }

    private System.Collections.IEnumerator LoadSceneAsync(Constant.GameScenes scene)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync((int)scene);

        while(!asyncOp.isDone)
        {
            yield return null;
        }

        m_CurrentScene = scene;
        m_IsExecutingSceneLoading = false;
    }


    public void StartSceneTransition(Constant.GameScenes scene)
    {
        if(m_IsExecutingSceneLoading)
        {
            return;
        }

        if(scene == m_CurrentScene)
        {
            return;
        }

        m_IsExecutingSceneLoading = true;
        StartCoroutine(LoadSceneAsync(scene));
    }

}
