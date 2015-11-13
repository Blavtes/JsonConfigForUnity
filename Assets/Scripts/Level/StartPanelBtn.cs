using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责点击某一关卡弹出的提示框
 * @Author      king
 * @date        2014-10-9
 * @desc        关于弹出提示框上面的界面布局的操作 
 */

public class StartPanelBtn : MonoBehaviour
{

    // 当没有体力时 弹出体力弹框
    public GameObject powerPanel;

    public GameObject TipPanel;

    public GameObject deleteBtn;
    public GameObject propBtn1;
    public GameObject propBtn2;
    public GameObject startBtn;

    public GameObject light1;
    public GameObject light2;

    public GameObject scoreNum;

    public GameObject title1;
    public GameObject title2;
    public GameObject title4;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public GameObject colorPubbleNum;
    public GameObject lightPubbleNum;

    public GameObject propDes;


    void Awake()
    {
        UIEventListener.Get(deleteBtn).onClick = deleteBtnClick;
        UIEventListener.Get(propBtn1).onClick = propBtn1Click;
        UIEventListener.Get(propBtn2).onClick = propBtn2Click;
        UIEventListener.Get(startBtn).onClick = startBtnClick;

        int level = UserInstanse.GetInstance().chooseLevel_id;
        title2.GetComponent<UILabel>().text = (level +1).ToString();
        LevelInfo info = UserInstanse.GetInstance().LevelData[level];

        lightPubbleNum.GetComponent<UILabel>().text = UserInstanse.GetInstance().lightPubble_Num.ToString();
        colorPubbleNum.GetComponent<UILabel>().text = UserInstanse.GetInstance().colorPubble_Num.ToString();

        Debug.Log("闪电泡泡：" + UserInstanse.GetInstance().lightPubble_Num.ToString() + "颜色泡泡：" + UserInstanse.GetInstance().colorPubble_Num.ToString());

        Debug.Log("关卡的信息" + (info.level_id+1)+ "星数：" + info.star + "关卡的分数：" + info.score);

        if (info.star > 0)
        {
            star1.SetActive(true);
        }
        if (info.star > 1)
        {
            star2.SetActive(true);
        }
        if (info.star > 2)
        {
            star3.SetActive(true);
        }
        scoreNum.GetComponent<UILabel>().text = info.score.ToString();

        light2.SetActive(false);
        light1.SetActive(false);
    }
    /*
     * @brief   第三种方式获取按钮的点击事件
     */

    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        Debug.Log("点击关闭按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        GameObject.Destroy(gameObject);
    }

    private bool isLight = false;

    void showPorpTipPanel(int type)
    {
        // 2. 下一界面的初始化
        GameObject tip = Instantiate(TipPanel) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        tip.gameObject.transform.parent = root.gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);

        BuyTipPanel panel = tip.GetComponent<BuyTipPanel>();

        panel.InitTipType((BuyTipPanel.PropType)type);
        panel.buyTip_sureDelegate = buyPropSuccess;
    }

    /// 购买道具回调函数
    void buyPropSuccess()
    {
        Debug.Log("开始游戏界面-------购买道具回调函数");
        if (isLight)
        {
            light2.active = true;
            propDes.GetComponent<UILabel>().text = ConstantString.LevelStartLightDes;
            lightPubbleNum.GetComponent<UILabel>().text = UserInstanse.GetInstance().lightPubble_Num.ToString();
            showGoodsTip(ConstantString.LevelStartAllTitles[1]);
        }
        else
        {
            light1.active = true;
            propDes.GetComponent<UILabel>().text = ConstantString.LevelStartColorDes;
            colorPubbleNum.GetComponent<UILabel>().text = UserInstanse.GetInstance().colorPubble_Num.ToString();
            showGoodsTip(ConstantString.LevelStartAllTitles[0]);
        }
    }
    public GameObject goodsTip;
    // 用于一个道具奖励
    public void showGoodsTip(string title)
    {
        GameObject tip = Instantiate(goodsTip) as GameObject;
        tip.gameObject.transform.parent = gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);
        tip.GetComponent<GoodsTipManager>().setTipTitle(title);
    }

    // 道具---变色泡泡按钮的点击事件响应
    void propBtn1Click(GameObject button)
    {
        Debug.Log("点击道具1按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        int num = UserInstanse.GetInstance().colorPubble_Num;
        if (num > 0)
        {
            if (light1.active) light1.active = false;
            else light1.active = true;
            propDes.GetComponent<UILabel>().text = ConstantString.LevelStartColorDes;
        }
        else
        {
            isLight = false;
            showPorpTipPanel(1);
        }
    }

    // 道具---闪电泡泡按钮的点击事件响应
    void propBtn2Click(GameObject button)
    {
        Debug.Log("点击道具2按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        int num = UserInstanse.GetInstance().lightPubble_Num;
        if (num > 0)
        {
            if (light2.active) light2.active = false;
            else light2.active = true;

            propDes.GetComponent<UILabel>().text = ConstantString.LevelStartLightDes;
        }
        else
        {
            isLight = true;
            showPorpTipPanel(4);
        }
    }

    // 开始游戏按钮按钮的点击事件响应
    void startBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();
        int heartNum = UserInstanse.GetInstance().powerNum;
        Debug.Log("开始按钮点击 取得的体力数" + heartNum);
        if (heartNum > 0)
        {
            //开始游戏
            startGame();
        }
        else // 没有体力时 弹出体力框
        {
            //  1. 体力界面的初始化
            GameObject cur = Instantiate(powerPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

            // 设置回调
            BuyPowerTip.doBuyPowerSuccess += buyPowerSuccess;
        }
    }
    /// 开始游戏 函数 会计算体力 技能泡泡 的数量
    public GameObject LoadingPanel;
    void startGame()
    {
        int heartNum = UserInstanse.GetInstance().powerNum;
        // 1. 设置体力值 与倒计时时间戳
        if (heartNum == 5)
        {
            UserInstanse.GetInstance().timeStamp = ResourceManager.GetUnixTimeStamp();
        }
        UserInstanse.GetInstance().powerNum -= 1;

        if (light2.active)
        {
            UserInstanse.GetInstance().lightPubble_Num -= 1;
        }

        if (light1.active)
        {
            UserInstanse.GetInstance().colorPubble_Num -= 1;
        }

        // 2. 刷新界面的体力值
        GameObject powerNum = GameObject.Find("UI Root/StatePanel/Up Bar/PowerNum");
        powerNum.GetComponent<UILabel>().text = (heartNum - 1).ToString();

        Debug.Log("startBtnClick -------- -进入下个场景");

        // 3. set up this level infomation
        UserInstanse.GetInstance().ReadBarrInfo(light1.active, light2.active);

        GameObject cur = Instantiate(LoadingPanel) as GameObject;
        cur.GetComponent<LoadingScene>().LoadingNextScene("PlayScene");
        
    }
    /// <summary>
    /// 执行购买体力成功的回调函数
    /// </summary>
    void buyPowerSuccess()
    {
        Debug.Log("执行BuyPowerTip回调函数---- 并开始游戏");
        startGame();
        BuyPowerTip.doBuyPowerSuccess -= buyPowerSuccess;
    }
   
}
