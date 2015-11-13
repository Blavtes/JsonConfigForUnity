using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责挑战胜利弹窗
 * @Author      king
 * @date        2014-10-14
 * @desc        关于胜利弹窗上的界面布局以及按钮事件相应
 */

public class WinPanel : MonoBehaviour
{

    // 体力界面
    public GameObject powerPanel;

    // 通关提示界面
    public GameObject successPanel;

    public GameObject agianBtn;
    public GameObject levelBtn;
    public GameObject nextBtn;

    public GameObject star0;
    public GameObject star1;
    public GameObject star2;

    public GameObject title1;
    public GameObject levelNum;
    public GameObject title4;

    public GameObject heightScore;
    public GameObject currentScore;

    LevelInfo currentLevelInfo = null;
    bool isNextTag = false;
    string[] AllReWard = { "体力 * 1", "钻石* 50", "变色泡泡 *2", "火球泡泡 *2", "冰冻泡泡 *2", "闪电泡泡 *2", "穿透泡泡 *2", "冲击泡泡 *2" };
    void Awake()
    {
        UIEventListener.Get(agianBtn).onClick = AgianBtnClick;
        UIEventListener.Get(levelBtn).onClick = LevelBtnClick;
        UIEventListener.Get(nextBtn).onClick = NextBtnClick;

        SoundManager.Instance.PlayWinSound();

       int level = UserInstanse.GetInstance().chooseLevel_id;
       levelNum.GetComponent<UILabel>().text = (level + 1).ToString();

       LevelInfo currentLevelInfo = UserInstanse.GetInstance().ReturnLevelInfo();

       int star = UserInstanse.GetInstance().currentBarrStarNum;
       int currenScore = UserInstanse.GetInstance().currentBarrScoreNum;
       int highScore = currentLevelInfo.score > currenScore ? currentLevelInfo.score : currenScore;

       Debug.Log("本次关卡获得的星数：" + star + "本次关卡获得的分数：" + currenScore + "以及最高分数：" + highScore);
       currentScore.GetComponent<UILabel>().text = currenScore.ToString();
       heightScore.GetComponent<UILabel>().text = highScore.ToString();

       ArrayList list = new ArrayList();
       if (star > 0)
       {
           star0.SetActive(true);
           if (currentLevelInfo.reward1 != 0)
           {
               list.Add(AllReWard[currentLevelInfo.reward1 - 1]);
           }
       }
       if (star > 1)
       {
           star1.SetActive(true);
           if (currentLevelInfo.reward2 != 0)
           {
               list.Add(AllReWard[currentLevelInfo.reward2 - 1]);
           }
       }
       if (star > 2)
       {
           star2.SetActive(true);
           if (currentLevelInfo.reward3 != 0)
           {
               list.Add(AllReWard[currentLevelInfo.reward3 - 1]);
           }
       }

       GameObject PlayUI = GameObject.Find("UI Root/Panel/PlayUI");

       //第一关 新手引导的奖励另算
       if (UserInstanse.GetInstance().chooseLevel_id == 0 && currentLevelInfo.getReward == 0)
       {
           list.Clear();
           list.Add("体力 * 1");
           list.Add("钻石 * 100");
           list.Add("闪电泡泡 *2");
           list.Add("彩虹泡泡 *2");
           list.Add("穿透泡泡 *2");
           list.Add("火球泡泡 *2");
           PlayUI.GetComponent<PlayUIScript>().showGoodsTips(list.ToArray());
           UserInstanse.GetInstance().ReturnLevelInfo().getReward = 1;

           UserInstanse.GetInstance().powerNum += 1;
           UserInstanse.GetInstance().coinNum += 100;
           UserInstanse.GetInstance().colorPubble_Num += 2;
           UserInstanse.GetInstance().lightPubble_Num += 2;
           UserInstanse.GetInstance().stonePubble_Num += 2;
           UserInstanse.GetInstance().firePubble_Num  += 2;

       }
       else if (currentLevelInfo.getReward == 0 && star > 2)
       {

           GetReward(currentLevelInfo.reward1 - 1);
           GetReward(currentLevelInfo.reward2 - 1);
           GetReward(currentLevelInfo.reward3 - 1);

           PlayUI.GetComponent<PlayUIScript>().showGoodsTips(list.ToArray());
           UserInstanse.GetInstance().ReturnLevelInfo().getReward = 1;
       }
       // 如果第60关打通  则弹通关提示
       if (UserInstanse.GetInstance().chooseLevel_id == 59)
       {
           GameObject cur = Instantiate(successPanel) as GameObject;
           GameObject root = GameObject.Find("UI Root");
           cur.gameObject.transform.parent = root.gameObject.transform;
           cur.transform.localScale = new Vector3(1, 1, 1);
       }
    }

    void GetReward(int tag)
    {
        switch (tag)
        {
            case 0:
                {
                    Debug.Log("奖励体力 1点");
                    UserInstanse.GetInstance().powerNum += 1;
                }
                break;
            case 1:
                {
                    Debug.Log("奖励钻石 50个");
                    UserInstanse.GetInstance().coinNum += 50;

                }
                break;
            case 2:
                {
                   Debug.Log("奖励变色泡泡 2个");
                   UserInstanse.GetInstance().colorPubble_Num += 2;

                }
                break;
            case 3:
                {
                    Debug.Log("奖励火球泡泡 2个");
                    UserInstanse.GetInstance().firePubble_Num += 2;
                }
                break;
            case 4:
                {
                    Debug.Log("奖励冰冻泡泡 2个");
                    UserInstanse.GetInstance().snowPubble_Num += 2;
                }
                break;
            case 5:
                {
                    Debug.Log("奖励闪电泡泡 2个");
                    UserInstanse.GetInstance().lightPubble_Num += 2;
                }
                break;
            case 6:
                {
                    Debug.Log("奖励穿透泡泡 +2");
                    UserInstanse.GetInstance().stonePubble_Num += 2;
                }
                break;
            case 7:
                {

                    Debug.Log("奖励冲击泡泡 2个");
                    UserInstanse.GetInstance().stockPubble_Num += 2;
                }
                break;
            default:
                break;
        }
    }

    /*
     * @brief   第三种方式获取按钮的点击事件
     */

    // 重新开始按钮的点击事件响应
    void AgianBtnClick(GameObject button)
    {
        //
        SoundManager.Instance.BlowUpVolume();
        SoundManager.Instance.PlayButtonTouchSound();

        int heartNum = UserInstanse.GetInstance().powerNum;
        Debug.Log("点击下一关卡  ------ 开始按钮点击 取得的体力数" + heartNum);
        if (heartNum > 0)
        {
            againGame();

        }
        else // 没有体力时 弹出体力框
        {
            GameObject cur = Instantiate(powerPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);
            isNextTag = false;
        }
    }

    // 选关按钮的点击事件响应
    void LevelBtnClick(GameObject button)
    {
        SoundManager.Instance.BlowUpVolume();
        SoundManager.Instance.PlayButtonTouchSound();

        if ((float)Screen.height / Screen.width <= 1.5f)
        {
            Application.LoadLevel("Level2Scene");
        }
        else
        {
            Application.LoadLevel("LevelScene");
        }

    }
    // 下一关卡按钮的点击事件响应
    void NextBtnClick(GameObject button)
    {
        SoundManager.Instance.BlowUpVolume();
        SoundManager.Instance.PlayButtonTouchSound();

        if (UserInstanse.GetInstance().chooseLevel_id != 59)
        {
            UserInstanse.GetInstance().isOpenNextLevel = true;
            UserInstanse.GetInstance().chooseLevel_id += 1;
        }

        if ((float)Screen.height / Screen.width <= 1.5f)
        {
            Application.LoadLevel("Level2Scene");
        }
        else
        {
            Application.LoadLevel("LevelScene");
        }

        return;


        int heartNum = UserInstanse.GetInstance().powerNum;
        Debug.Log("点击下一关卡  ------ 开始按钮点击 取得的体力数" + heartNum);
        if (heartNum > 0)
        {
            OpenNextLevel();

        }
        else // 没有体力时 弹出体力框
        {
            GameObject cur = Instantiate(powerPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

            isNextTag = true;
        }
    }
    public GameObject LoadingPanel;
    void OpenNextLevel()
    {
        // 1. 设置体力值 与倒计时时间戳
        if (UserInstanse.GetInstance().powerNum == 5)
        {
            UserInstanse.GetInstance().timeStamp = ResourceManager.GetUnixTimeStamp();
        }
        UserInstanse.GetInstance().powerNum -= 1;

        // 2. set up this level infomation
        UserInstanse.GetInstance().chooseLevel_id += 1;
        UserInstanse.GetInstance().ReadBarrInfo(false, false);

        GameObject cur = Instantiate(LoadingPanel) as GameObject;
        cur.GetComponent<LoadingScene>().LoadingNextScene("PlayScene");
    }

    void againGame()
    {
        // 1. 设置体力值 与倒计时时间戳
        if (UserInstanse.GetInstance().powerNum == 5)
        {
            UserInstanse.GetInstance().timeStamp = ResourceManager.GetUnixTimeStamp();
        }
        UserInstanse.GetInstance().powerNum -= 1;

        // 2. 加载本关卡
        GameObject cur = Instantiate(LoadingPanel) as GameObject;
        cur.GetComponent<LoadingScene>().LoadingNextScene("PlayScene");
    }

    #region  开启与关闭 代理方法 以及代理方法的实现

    void OnEnable()
    {
        Debug.Log("LosePanel ----- onEnable");
        BuyPowerTip.doBuyPowerSuccess += doBuyPowerSuccessDelgate;
    }

    void OnDisable()
    {
        Debug.Log("LosePanel ----- OnDisable");
        BuyPowerTip.doBuyPowerSuccess -= doBuyPowerSuccessDelgate;
    }
    //回调方法的实现
    void doBuyPowerSuccessDelgate()
    {
        Debug.Log(" 购买体力成功 回调函数 ---- doBuyPowerSuccessDelgate :");
        // 重新开始本关游戏
        if (isNextTag)
        {
            OpenNextLevel();
        }
        else
        {
            againGame();
        }
    }
    #endregion
}
