using UnityEngine;
using System.Collections;


/*
 * @brief       绑定到开始界面 初始化玩家的一些信息
 * @Author      King
 * @date        2014-10-13
 * @desc        数据单例的初始化
 */

public class StartPanel : MonoBehaviour {

    public GameObject root;

    public GameObject GoodsTipPanel;

    public GameObject bgmusicBtn;
    public GameObject videoBtn;

    void Awake()
    {
        // 单例的初始化放在开始界面
        UserInstanse.GetInstance();
    }

	// Use this for initialization
	void Start () 
    {
        if (!UserInstanse.GetInstance().audioSet)
        {
            videoBtn.GetComponent<UISprite>().spriteName = "music_off";
            videoBtn.GetComponent<UIButton>().normalSprite = "music_off";
            videoBtn.GetComponent<UIButton>().pressedSprite = "music_off_an";
        }

        if (!UserInstanse.GetInstance().soundSet)
        {
            bgmusicBtn.GetComponent<UISprite>().spriteName = "sound_off";
            bgmusicBtn.GetComponent<UIButton>().normalSprite = "sound_off";
            bgmusicBtn.GetComponent<UIButton>().pressedSprite = "sound_off_an";
        }
	
	}
    // 开始按钮的点击事件响应
    public void StartBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();
        Debug.Log("目标机器的 宽度：---" + Screen.width);
        Debug.Log("目标机器的 高度：---" + Screen.height);
        Debug.Log("目标机器的 高度：---" + (float)Screen.height / Screen.width);
        if ((float)Screen.height / Screen.width <= 1.5f)
        {
            Application.LoadLevel("Level2Scene");
        }
        else
        {
            Application.LoadLevel("LevelScene");
        }
        
        Resources.UnloadUnusedAssets();
    }

    // 关于我们的点击事件响应
    public void AboutBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();
       Debug.Log("点击按钮----  button name :" + button.name);
       Application.LoadLevel("AboutScene");
    }

    // 问题按钮的点击事件响应
    public void QuestionBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();
        Debug.Log("点击按钮----  button name :" + button.name);
        Application.LoadLevel("HelpScene");
    }

    // 音效按钮的点击事件响应
    public void MusicBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();
        string name = button.GetComponent<UIButton>().normalSprite;
        Debug.Log("点击按钮----按钮的精灵名称" + name);
        if (name == "music_on")
        {
            button.GetComponent<UISprite>().spriteName = "music_off";
            button.GetComponent<UIButton>().normalSprite = "music_off";
            button.GetComponent<UIButton>().pressedSprite = "music_off_an";
            UserInstanse.GetInstance().audioSet = false;
        }
        else
        {
            button.GetComponent<UISprite>().spriteName = "music_on";
            button.GetComponent<UIButton>().normalSprite = "music_on";
            button.GetComponent<UIButton>().pressedSprite = "music_on_an";
            UserInstanse.GetInstance().audioSet = true;
        }
    }
    // 背景音乐按钮的点击事件响应
    public void SoundBtnClick(GameObject button)
    {
        Debug.Log("点击关闭按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        string name = button.GetComponent<UIButton>().normalSprite;
        if (name == "sound_on")
        {
            button.GetComponent<UISprite>().spriteName = "sound_off";
            button.GetComponent<UIButton>().normalSprite = "sound_off";
            button.GetComponent<UIButton>().pressedSprite = "sound_off_an";
            SoundManager.Instance.CloseBackGroundMusic();
        }
        else
        {
            button.GetComponent<UISprite>().spriteName = "sound_on";
            button.GetComponent<UIButton>().normalSprite = "sound_on";
            button.GetComponent<UIButton>().pressedSprite = "sound_on_an";
            SoundManager.Instance.PlayBackGroundMusic();
           
        }
    }
   public void showGoodsTip(string title)
    {
        GameObject tip = Instantiate(GoodsTipPanel) as GameObject;
        tip.gameObject.transform.parent = gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);
        tip.GetComponent<GoodsTipManager>().setTipTitle(title);

    }
}
