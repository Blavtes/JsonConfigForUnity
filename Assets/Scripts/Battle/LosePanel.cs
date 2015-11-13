using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责挑战失败弹窗
 * @Author      king
 * @date        2014-10-14
 * @desc        关于失败弹窗上的界面布局以及按钮事件相应
 */

public class LosePanel : MonoBehaviour
{

    public GameObject powerPanel;

    public GameObject agianBtn;
    public GameObject levelBtn;

    public GameObject heightScore;
    public GameObject thisScore;

    void Awake()
    {
        UIEventListener.Get(agianBtn).onClick = AgianBtnClick;
        UIEventListener.Get(levelBtn).onClick = LevelBtnClick;

        SoundManager.Instance.PlayLoseSound();

        LevelInfo currentLevelInfo = UserInstanse.GetInstance().ReturnLevelInfo();
        int currenScore = UserInstanse.GetInstance().currentBarrScoreNum;
        int highScore = currentLevelInfo.score > currenScore ? currentLevelInfo.score : currenScore;

        Debug.Log("本次关卡获得的分数：" + currenScore + "以及最高分数：" + highScore);
        thisScore.GetComponent<UILabel>().text = currenScore.ToString();
        heightScore.GetComponent<UILabel>().text = highScore.ToString();
    }
    /*
     * @brief   第三种方式获取按钮的点击事件
     */

    // 重新开始按钮的点击事件响应
    void AgianBtnClick(GameObject button)
    {
        SoundManager.Instance.BlowUpVolume();
        SoundManager.Instance.PlayButtonTouchSound();
        int heartNum = UserInstanse.GetInstance().powerNum;
        Debug.Log("点击下一关卡  ------ 开始按钮点击 取得的体力数" + heartNum);
        if (heartNum > 0)
        {
            // 重新开始本关游戏
            againGame();
        }
        else // 没有体力时 弹出体力框
        {
            GameObject cur = Instantiate(powerPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);
        }

    }

    // 选关按钮的点击事件响应
    void LevelBtnClick(GameObject button)
    {
        SoundManager.Instance.BlowUpVolume();
        Debug.Log("选关按钮的----  button name :" + button.name);
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

    public GameObject LoadingPanel;
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
        againGame();
    }
    #endregion
}
