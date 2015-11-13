using UnityEngine;
using System.Collections;

public class TipBackDlg : MonoBehaviour {

	// Use this for initialization
    string currentSceneName = null;
	void Start () {
        currentSceneName = Application.loadedLevelName;
        if (currentSceneName == ConstantValue.PlaySceneName)
        { 
            //弹出退出游戏面板，则暂停游戏
            SlidePlayPanel.Instance.PauseSlide();
        }
	}

    public void MakeSureButton()
    {
        //判断当前是否在游戏场景内
        
        SoundManager.Instance.PlayButtonTouchSound();
        Application.Quit();
    }

    public void CloseDlgButton()
    {
        if (currentSceneName == ConstantValue.PlaySceneName)
            //弹出退出游戏面板，则暂停游戏
            SlidePlayPanel.Instance.RestoreSlideType();
        SoundManager.Instance.PlayButtonTouchSound();
        Destroy(gameObject);
    }
}
