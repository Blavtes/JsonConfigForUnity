using UnityEngine;
using System.Collections;


/*
 * @brief       当前的职责是负责选关界面的按钮事件响应 以及体力值得计算 体力倒计时
 * @Author      king
 * @date        2014-10-9
 * @desc        体力值按钮 商店按钮 返回按钮的点击事件响应
 */

public class LevelPanelBtn : MonoBehaviour
{


    public GameObject shopPanel;

    public GameObject powerPanel;

    public GameObject startPanel;

    // 体力值label
    public GameObject powerNum;

    // 钻石label
    public GameObject daimondNum;
    // 计时器label
    public GameObject timeLabel;

    public GameObject addBtn;
    public GameObject shopBtn;
    public GameObject giftBtn;
    public GameObject returnBtn;


    #region   方法的overrload
    void Awake()
    {
        //GameObject addBtn = GameObject.Find("UI Root/StatePanel/Up Bar/AddPower");
        UIEventListener.Get(addBtn).onClick = AddBtnClick;

        //GameObject shopBtn = GameObject.Find("UI Root/StatePanel/Down Bar/ShopBtn");
        UIEventListener.Get(shopBtn).onClick = ShopBtnClick;

        //GameObject returnBtn = GameObject.Find("UI Root/StatePanel/Down Bar/Return");
        UIEventListener.Get(returnBtn).onClick = ReturnBtnClick;

        UIEventListener.Get(giftBtn).onClick = GiftBtnClick;
    }

    // 关于玩家体力的初始化
    void Start()
    {

        daimondNum.GetComponent<UILabel>().text = UserInstanse.GetInstance().coinNum.ToString();
        int currentTime = ResourceManager.GetUnixTimeStamp();

        // 测试专用
        //UserInstanse.GetInstance().powerNum = 1;
        //UserInstanse.GetInstance().timeStamp = currentTime;

        int heart = UserInstanse.GetInstance().powerNum;
        int timestamp = UserInstanse.GetInstance().timeStamp;

        // 第一次进入游戏 没有存储任何数据
        if (heart == 0 && timestamp == 0)
        {
            UserInstanse.GetInstance().powerNum = 5;
            UserInstanse.GetInstance().timeStamp = currentTime;

            //UserDataInstanse.SetHeartNum(5);
            //UserDataInstanse.SetTimeStamp(currentTime);

            powerNum.GetComponent<UILabel>().text = "5";
            timeLabel.GetComponent<UILabel>().text = "";

            Debug.Log("第一次进入游戏 剩余的体力");
            return;
        }

        // 当再次进入该界面时 体力值小于5时 
        if (heart < 5 && timestamp != 0)
        {
            // 当前全部体力值
            int addHeart = (currentTime - timestamp) / (60 * 30);
            int allHeart = addHeart + heart;

            if (allHeart >= 5)  // 表示已经恢复满了体力
            {
                Debug.Log("HeartData 当再次进入该界面时 恢复满体力时 " + allHeart + "\n");

                //UserDataInstanse.SetHeartNum(5);
                //UserDataInstanse.SetTimeStamp(currentTime);

                UserInstanse.GetInstance().powerNum = 5;
                UserInstanse.GetInstance().timeStamp = currentTime;


                powerNum.GetComponent<UILabel>().text = "5";
                timeLabel.GetComponent<UILabel>().text = "";
            }

            if (allHeart < 5) // 表示未恢复满体力
            {
                // UserDataInstanse.SetHeartNum(allHeart);
                // UserDataInstanse.SetTimeStamp(timestamp + addHeart * 60 * 30);

                UserInstanse.GetInstance().powerNum = allHeart;
                UserInstanse.GetInstance().timeStamp = timestamp + addHeart * 60 * 30;

                InvokeRepeating("UpdateLabelTime", 1, 1);

                powerNum.GetComponent<UILabel>().text = allHeart.ToString();
                timeLabel.GetComponent<UILabel>().text = "";
            }
        }
        // 购买过体力 体力值大于5
        if (heart > 5)
        {
            // 需要更新UI 体力值
            powerNum.GetComponent<UILabel>().text = heart.ToString();
            timeLabel.GetComponent<UILabel>().text = "";
        }


        //ResourceManager.SetUseInfo(ConstantValue.FirstTime_Player,1);
        // 刷新礼包
        if (ResourceManager.GetUserInfo(ConstantValue.FirstTime_Player) != 1)
        {
            giftBtn.GetComponent<UISprite>().spriteName = "libao";
        }

        InitStartPanel();
    }

    /// <summary>
    /// 是否开启下一关卡
    /// </summary>
    void InitStartPanel()
    {
        if (UserInstanse.GetInstance().isOpenNextLevel)
        {
            GameObject cur = Instantiate(startPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

            UserInstanse.GetInstance().isOpenNextLevel = false;
        }
    }
    // 更新钻石的数量
    void Update()
    {
        daimondNum.GetComponent<UILabel>().text = UserInstanse.GetInstance().coinNum.ToString();
    }

    /* 
     * @brief  更新体力值上面的label text   
     */
    void UpdateLabelTime()
    {

        //int timestamp = UserDataInstanse.GetTimeStamp();
        //int currentTime = UserDataInstanse.GetUnixTimeStamp();

        int currentTime = ResourceManager.GetUnixTimeStamp();
        int timestamp = UserInstanse.GetInstance().timeStamp;



        int minute = (60 * 30 - currentTime + timestamp) / 60;
        int second = (60 * 30 - currentTime + timestamp) % 60;

        string timeStr = string.Format("{0:D2}", minute) + ":" + string.Format("{0:D2}", second);

        timeLabel.GetComponent<UILabel>().text = timeStr;

        // 当倒计时为0时 
        if (minute == 0 && second == 0)
        {
            int heart = UserInstanse.GetInstance().powerNum;

            //UserDataInstanse.SetHeartNum(heart + 1);
            // UserDataInstanse.SetTimeStamp(currentTime);

            UserInstanse.GetInstance().powerNum = heart + 1;
            UserInstanse.GetInstance().timeStamp = currentTime;


            powerNum.GetComponent<UILabel>().text = (heart + 1).ToString();

            // 恢复满了体力 则取消定时器的调用
            if (heart + 1 == 5)
            {
                timeLabel.GetComponent<UILabel>().text = "";
                CancelInvoke("UpdateLabelTime");
            }
        }

    }
    #endregion   方法的overrload

    #region 四个按钮的事件相应

    public GameObject buyTipPanel;
    void GiftBtnClick(GameObject button)
    {

        GameObject cur = Instantiate(buyTipPanel) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        cur.gameObject.transform.parent = root.gameObject.transform;
        cur.transform.localScale = new Vector3(1, 1, 1);

        BuyTipPanel panel = cur.GetComponent<BuyTipPanel>();
        panel.buyTip_sureDelegate = makeSureDelegate;

        if (ResourceManager.GetUserInfo(ConstantValue.FirstTime_Player) == 1)
            panel.InitTipType(BuyTipPanel.PropType.PropType_Gift);
        else
            panel.InitTipType(BuyTipPanel.PropType.PropType_Seven);
    }

    void makeSureDelegate()
    {
        Debug.Log("makeSureDelegate-----");
        string cur = "";
        if (ResourceManager.GetUserInfo(ConstantValue.FirstTime_Player) == 1)
        {
            cur = ConstantString.LevelBottomTitles[0] as string;
            ResourceManager.SetUseInfo(ConstantValue.FirstTime_Player,2);
            giftBtn.GetComponent<UISprite>().spriteName = "libao";
        }
        else
            cur = ConstantString.LevelBottomTitles[1] as string;
        object[] titles = cur.Split(',');
        showGoodsTips(titles);
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
    // 购买体力按钮的点击事件响应
    void AddBtnClick(GameObject button)
    {
        Debug.Log("点击加号按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();

        GameObject cur = Instantiate(powerPanel) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        cur.gameObject.transform.parent = root.gameObject.transform;
        cur.transform.localScale = new Vector3(1, 1, 1);
        cur.transform.localPosition = new Vector3(0, 0, 0);
    }

    // 商店按钮的点击事件响应
    void ShopBtnClick(GameObject button)
    {
        Debug.Log("点击商店按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();

        GameObject cur = Instantiate(shopPanel) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        cur.gameObject.transform.parent = root.gameObject.transform;
        cur.transform.localScale = new Vector3(1, 1, 1);

    }
    // 返回按钮的点击事件响应
    void ReturnBtnClick(GameObject button)
    {
        Debug.Log("点击返回按钮 -- 返回按钮的名称 :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        Application.LoadLevel("StartScene");
    }
    #endregion 三个按钮的事件相应

    #region  开启与关闭 代理方法 以及代理方法的实现

    void OnEnable()
    {
        Debug.Log("BuyTipPanel ----- onEnable");
        BuyPowerTip.doBuyPowerSuccess += doBuyPowerSuccessDelgate;
    }

    void OnDisable()
    {
        Debug.Log("BuyTipPanel ----- OnDisable");
        BuyPowerTip.doBuyPowerSuccess -= doBuyPowerSuccessDelgate;
    }
    //回调方法的实现
    void doBuyPowerSuccessDelgate()
    {
        Debug.Log(" 购买体力成功 回调函数 ---- doBuyPowerSuccessDelgate :");

        powerNum.GetComponent<UILabel>().text = "" + UserInstanse.GetInstance().powerNum;
        timeLabel.GetComponent<UILabel>().text = "";
        CancelInvoke("UpdateLabelTime");
    }
    #endregion
}
