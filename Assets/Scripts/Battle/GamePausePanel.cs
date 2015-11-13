using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
/*
 * @brief       当前负责游戏暂停界面
 * @Author      king
 * @date        2014-10-14
 * @desc        关于弹出暂停界面上的界面布局以及按钮的事件相应
 */


public class GamePausePanel : MonoBehaviour
{

    public GameObject costDaimondPanel;

    public GameObject continueBtn;
    public GameObject againBtn;
    public GameObject homeBtn;
    public GameObject musicBtn;
    public GameObject voiceBtn;
    public GameObject getBtn;

    public GameObject prop;
    public GameObject propDes;

    private int  propIndex = 0;

    void Awake()
    {
        UIEventListener.Get(continueBtn).onClick = continueBtnClick;
        UIEventListener.Get(againBtn).onClick = againBtnClick;
        UIEventListener.Get(homeBtn).onClick = homeBtnClick;
        UIEventListener.Get(musicBtn).onClick = musicBtnClick;
        UIEventListener.Get(voiceBtn).onClick = voiceBtnClick;
        UIEventListener.Get(getBtn).onClick = getBtnClick;

    }
    // Use this for initialization
    void Start()
    {
        randomProp();

        if (!UserInstanse.GetInstance().soundSet)
        {
            voiceBtn.GetComponent<UISprite>().spriteName = "yinliang_off";
            voiceBtn.GetComponent<UIButton>().normalSprite = "yinliang_off";
            voiceBtn.GetComponent<UIButton>().pressedSprite = "yinliang_off_an";
        }
        if (!UserInstanse.GetInstance().audioSet)
        {
            musicBtn.GetComponent<UISprite>().spriteName = "yinyue_off";
            musicBtn.GetComponent<UIButton>().normalSprite = "yinyue_off";
            musicBtn.GetComponent<UIButton>().pressedSprite = "yinyue_off_an";
        }
    }
    /*
     * @brief   随机游戏暂停界面上的道具 
     */
    void randomProp()
    {
        propIndex = Random.Range(1, 7);
        prop.GetComponent<UISprite>().spriteName = "bag" + propIndex.ToString();
        propDes.GetComponent<UILabel>().text = ConstantString.LevelPropTitles[propIndex-1];

        Debug.Log("随机生成的数字：------bag" + propIndex + "\n 介绍：" + ConstantString.LevelPropTitles[propIndex - 1]);
        if (propIndex > 1)
        {
            prop.GetComponent<UISprite>().height = 114;
        }
        else
        {
            prop.GetComponent<UISprite>().height = 134;
        }
    }

    /*
     * @brief   第三种方式获取按钮的点击事件
     */

    // 继续按钮的点击事件响应
    void continueBtnClick(GameObject button)
    {
        Debug.Log("点击继续按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();

		//滑动的泡泡停止
        SlidePlayPanel.Instance.RestoreSlideType();
		//gamePausePanel.SetActive (false);
        Destroy(gameObject);
    }

    // 重来按钮的点击事件响应
    public GameObject LoadingPanel;
    void againBtnClick(GameObject button)
    {
        Debug.Log("点击重来按钮按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();

        GameObject cur = Instantiate(LoadingPanel) as GameObject;
        cur.GetComponent<LoadingScene>().LoadingNextScene("PlayScene");

    }
    // j首页按钮的点击事件响应
    void homeBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();

        Debug.Log("点击首页按钮----  button name :" + button.name);

        if ((float)Screen.height / Screen.width <= 1.5f)
        {
            Application.LoadLevel("Level2Scene");
        }
        else
        {
            Application.LoadLevel("LevelScene");
        }
    }

    // 音效按钮的点击事件响应
    void musicBtnClick(GameObject button)
    {
        Debug.Log("点击开始游戏按钮----  button name :" + button.name);
        string name = button.GetComponent<UIButton>().normalSprite; ;
        Debug.Log("按钮的精灵名称" + name);
        
        if (name == "yinyue")
        {
            button.GetComponent<UISprite>().spriteName = "yinyue_off";
            button.GetComponent<UIButton>().normalSprite = "yinyue_off";
            button.GetComponent<UIButton>().pressedSprite = "yinyue_off_an";
            UserInstanse.GetInstance().audioSet = false;
        }
        else
        {
            button.GetComponent<UISprite>().spriteName = "yinyue";
            button.GetComponent<UIButton>().normalSprite = "yinyue";
            button.GetComponent<UIButton>().pressedSprite = "yinyue_an";
            UserInstanse.GetInstance().audioSet = true;
        }
    }
    // 背景音乐按钮的点击事件响应
    void voiceBtnClick(GameObject button)
    {
        Debug.Log("点击音量按钮----  button name :" + button.name);
        string name = button.GetComponent<UIButton>().normalSprite; ;
        Debug.Log("按钮的精灵名称" + name);
        if (name == "yinliang")
        {
            button.GetComponent<UISprite>().spriteName = "yinliang_off";
            button.GetComponent<UIButton>().normalSprite = "yinliang_off";
            button.GetComponent<UIButton>().pressedSprite = "yinliang_off_an";
            SoundManager.Instance.CloseBackGroundMusic();
        }
        else
        {
            button.GetComponent<UISprite>().spriteName = "yinliang";
            button.GetComponent<UIButton>().normalSprite = "yinliang";
            button.GetComponent<UIButton>().pressedSprite = "yinliang_an";
            SoundManager.Instance.PlayBackGroundMusic();
        }
    }

    // 获取按钮的点击事件响应
    public GameObject BuyTipPanel;
    void getBtnClick(GameObject button)
    {
        Debug.Log("点击获取游戏按钮----  button name :" + button.name);

        // 2. 购买提示界面的初始化
        GameObject cur = Instantiate(BuyTipPanel) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        cur.gameObject.transform.parent = root.gameObject.transform;
        cur.transform.localScale = new Vector3(1, 1, 1);

        BuyTipPanel panel = cur.GetComponent<BuyTipPanel>();
        panel.InitTipType((BuyTipPanel.PropType)propIndex - 1);
        panel.buyTip_sureDelegate = makeSureDelegate;
    }

    void makeSureDelegate()
    {
        Debug.Log("makeSureDelegate-----");

        string cur = ConstantString.LevelPropTips[propIndex-1] as string;
        object[] titles = cur.Split(',');
        showGoodsTips(titles);

        randomProp();
    }

    public GameObject goodsTip;
    // 用于多个道具奖励
    public void showGoodsTips(object[] titles)
    {
        GameObject tip = Instantiate(goodsTip) as GameObject;
        tip.gameObject.transform.parent = gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);
        tip.GetComponent<GoodsTipManager>().setTipTitles(titles);

    }
}
