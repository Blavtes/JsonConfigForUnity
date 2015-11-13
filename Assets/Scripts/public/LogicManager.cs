using UnityEngine;
using System.Collections;

/*
 * @brief       负责控制游戏的简单逻辑
 * @desc        切入后台
 * @desc        back事件
 */
public class LogicManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(transform.gameObject);
	}

    #region 负责游戏的后台跟切入 或者退出游戏时
    void OnApplicationPause(bool pauseStatus)
    {
        UserInstanse.GetInstance().WriteLevelInfoToPersistent();
        Debug.Log("负责游戏的后台跟切入-------OnApplicationPause");
    }
    void OnApplicationQuit()
    {
        Debug.Log("程序退出前 调用该方法-------OnApplicationQuit");
        UserInstanse.GetInstance().WriteLevelInfoToPersistent();
    }
    #endregion 负责游戏的后台跟切入

}
