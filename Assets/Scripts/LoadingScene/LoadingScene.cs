using UnityEngine;
using System.Collections;

/*
 * @brief       负责过渡，异步加载playscene场景
 */
public class LoadingScene : MonoBehaviour {
    AsyncOperation async;
    void Start () 
    {
	}
    public void LoadingNextScene(string levelStr)
    {
        //加载场景
        StartCoroutine(AsyncLoadingNextScene(levelStr));
    }

    IEnumerator AsyncLoadingNextScene(string scenename)
    {
        //避免突兀，先转动0.5s
        yield return new WaitForSeconds(1.0f);
        async = Application.LoadLevelAsync(scenename);
        yield return async;
        Resources.UnloadUnusedAssets();
    }
}
