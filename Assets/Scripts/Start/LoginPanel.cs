using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责登陆界面 领取登陆奖励
 * @Author      king
 * @date        2014-10-24
 * @desc        关于登陆界面的逻辑实现
 */

public class LoginPanel : MonoBehaviour
{

    public GameObject UIRoot;
 
    public GameObject circleSprite;

    public GameObject loginPanel;

    public GameObject[] getBtns;

    public GameObject DeleteBtn;

    int[] logindayData;
    int loginDayTimestamp;

    int getBtnTag = -1;

    int[] posX = {-190,-65, 65,187,-144,0,144};
    int[] posY = { 142, 142, 142, 142, -85, -85, -85};

    void Awake()
    {
        foreach(GameObject ob in getBtns)
        {
            UIEventListener.Get(ob).onClick = okBtnClick;
        }

        UIEventListener.Get(DeleteBtn).onClick = deleteBtnClick;

        logindayData = UserInstanse.GetInstance().loginData;
        loginDayTimestamp = UserInstanse.GetInstance().daytimeStamp;
    }
    void  Start ()
    {
        Debug.Log("时间戳 为0时 判断是是第一次进入游戏 --- 时间戳是：" + loginDayTimestamp + "logindayData的长度是:" + logindayData.Length);
        if (0 == loginDayTimestamp)  // 第一次进入游戏
        {
            InitLoginData();
            UserInstanse.GetInstance().loginData = logindayData;
            UserInstanse.GetInstance().daytimeStamp = ResourceManager.GetUnixTimeStamp();
        }
        else
        {
            int time = ResourceManager.GetUnixTimeStamp() - loginDayTimestamp;
            int currentTime = ResourceManager.GetUnixTimeStamp();
            Debug.Log("----------------------------------现在的时间差额 time：" + time);

            //if (time / 10  == 1)  // test 使用
            if (time / (60 *60 *24)  == 1)  // 超过24个小时 又小于48个小时
            {
                for (int i = 0; i < logindayData.Length; ++i)
                {
                    if (logindayData[i] == 3 && i == 6) //第七天领取奖励
                    {
                        logindayData[i] = 1;
                        logindayData[0] = 2;
                        break;
                    }
                    else if(logindayData[i] == 3)
                    {
                        logindayData[i] = 1;
                        logindayData[i+1] = 2;
                        break;
                    }
                }
                UserInstanse.GetInstance().loginData = logindayData;
            }
            //else   if (time / 10  > 1)  // test 使用
            else if (time / (60 * 60 * 24) > 1) // 超过48小时 重新开始计算奖励
            {
                InitLoginData();
                UserInstanse.GetInstance().loginData = logindayData;
                UserInstanse.GetInstance().daytimeStamp = ResourceManager.GetUnixTimeStamp();
            }
        }
        RefreshBtns();
    }

    /// <summary>
    /// 初始化登陆奖励数据 使之第一天可以领取
    /// </summary>
    void InitLoginData()
    {
        for (int i = 0; i < logindayData.Length; ++i)
        {
            if (i == 0)
            {
                logindayData[i] = 2; // 标示第一天应该领取
            }
            else
            {
                logindayData[i] = 1; // 标示其余天不应该领取
            }
        }
    }

    /// <summary>
    /// 刷新一个button
    /// </summary>
    void RefreshBtn(GameObject btn, string normalName,string pressName)
    {
        btn.GetComponent<UISprite>().spriteName = normalName;
        btn.GetComponent<UIButton>().normalSprite = normalName;
        btn.GetComponent<UIButton>().pressedSprite = pressName;
    }

    /// <summary>
    /// 刷新所有的button
    /// </summary>
    void RefreshBtns()
    {
        for (int i = 0; i < 7; ++i)
        {
            GameObject obj = getBtns[i] as GameObject;
            int tag = logindayData[i];

            if (tag == 1) // 未领取
            {
                RefreshBtn(obj, "lingqu_02", "lingqu_02");
            }
            else if (tag == 2) // 应该领取
            {
                RefreshBtn(obj, "lingqu_01", "lingqu_01_an");

                GameObject cur = Instantiate(circleSprite) as GameObject;
                cur.gameObject.transform.parent = gameObject.transform;
                cur.transform.localScale = new Vector3(1, 1, 1);
                cur.transform.localPosition = new Vector3(posX[i], posY[i], 0);
            }
            else if (tag == 3) // 已领取
            {
                RefreshBtn(obj, "yiling_01", "yiling_01");
            }
        }
    }

    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();
        GameObject.Destroy(loginPanel);
    }

    // 确认按钮的点击事件响应
    void okBtnClick(GameObject button)
    {
        Debug.Log("登陆礼包 领取的天数-----" + button.name + button.GetComponent<UISprite>().spriteName);

        SoundManager.Instance.PlayButtonTouchSound();
        int btnNum = int.Parse(button.name.Substring(3)); // btn (1-7)
        string name = button.GetComponent<UISprite>().spriteName.Substring(0,9);
        if (name == "lingqu_01") // 可点击
        {
            switch (btnNum)
            {
                case 1:
                    {
                        UserInstanse.GetInstance().coinNum += 100;   // 获得100 金币
                        UIRoot.GetComponent<StartPanel>().showGoodsTip(ConstantString.LoginTip1);
                    }
                    break;
                case 2:
                    {
                        UserInstanse.GetInstance().firePubble_Num += 2; // 火焰泡泡
                        UIRoot.GetComponent<StartPanel>().showGoodsTip(ConstantString.LoginTip2);
                    }
                    
                    break;
                case 3:
                    {
                        UserInstanse.GetInstance().stockPubble_Num += 2; //冲击泡泡
                        UIRoot.GetComponent<StartPanel>().showGoodsTip(ConstantString.LoginTip3);
                    }
                    break;
                case 4:
                    {
                        UserInstanse.GetInstance().coinNum += 300;   // 获得300 金币
                        UIRoot.GetComponent<StartPanel>().showGoodsTip(ConstantString.LoginTip4);
                    }
                    break;
                case 5:
                    {
                        UserInstanse.GetInstance().snowPubble_Num += 2; //雪花泡泡
                        UIRoot.GetComponent<StartPanel>().showGoodsTip(ConstantString.LoginTip5);
                       }
                    break;
                case 6:
                    {
                        UserInstanse.GetInstance().stonePubble_Num += 2; //穿透泡泡
                        UIRoot.GetComponent<StartPanel>().showGoodsTip(ConstantString.LoginTip6);
                    }
                    break;
                case 7:
                    {
                        UserInstanse.GetInstance().coinNum += 500;   // 获得500 金币
                        UIRoot.GetComponent<StartPanel>().showGoodsTip(ConstantString.LoginTip7);
                    }
                    break;
                default:
                    break;
            }

            logindayData[btnNum - 1] = 3; //标示已经领取过了
            UserInstanse.GetInstance().loginData = logindayData;
            UserInstanse.GetInstance().daytimeStamp = ResourceManager.GetUnixTimeStamp();
            GameObject.Destroy(loginPanel);
        }
    }


}

