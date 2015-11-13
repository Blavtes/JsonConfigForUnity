using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责购买体力界面的布局 以及事件相应
 * @Author      king
 * @date        2014-10-13
 * @desc        关于体力界面的两个按钮布局以及事件相应
 */

public class BuyPowerTip : MonoBehaviour
{
    // 购买钻石panel
    public GameObject costDiamondPanel;

    public GameObject OkBtn;
    public GameObject DeleteBtn;

    public bool isLevelScene = false;

    // 关闭购买体力界面的回调
    public delegate void BuyPowerFail();
    public static event BuyPowerFail buyPowerFail = null;

    // 购买体力成功的回调
    public delegate void BuyPowerSuccess();
    public static event  BuyPowerSuccess doBuyPowerSuccess = null;


    void Awake()
    {
        UIEventListener.Get(OkBtn).onClick = okBtnClick;
        UIEventListener.Get(DeleteBtn).onClick = deleteBtnClick;
    }

    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        Debug.Log("点击关闭按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        GameObject.Destroy(gameObject);
    }

    /// <summary>
    ///  补充体力
    /// </summary>
    void AddPower()
    {
        UserInstanse.GetInstance().coinNum -= 300;

        //修改为+5 没有上限
        UserInstanse.GetInstance().powerNum += 5;
        UserInstanse.GetInstance().timeStamp = ResourceManager.GetUnixTimeStamp();

        if (doBuyPowerSuccess != null)
        {
            doBuyPowerSuccess();
        }

        GameObject.Destroy(gameObject);
    }

    // 确认按钮的点击事件响应
    void okBtnClick(GameObject button)
    {
        Debug.Log("点击确认按钮----  是否是选关场景:" + isLevelScene);
        SoundManager.Instance.PlayButtonTouchSound();
        if (UserInstanse.GetInstance().coinNum > 300)
        {
            AddPower();
        }
        else
        {
            Debug.Log("初始化钻石购买界面");
            // 初始化钻石购买界面
            GameObject cur = Instantiate(costDiamondPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

            cur.GetComponent<CostDaimondPanel>().preObject = gameObject;

            CostDaimondPanel panel = cur.GetComponent<CostDaimondPanel>();
            panel.InitTipType(RechargeInstanse.PayType.PayType_Five);
            panel.costDaimod_MakeSuerDelegate = makeSuerDelegate;
            panel.costDaimod_deleteDelegate = deleteDelegate;

            gameObject.active = false;
        }
    }
    /// <summary>
    /// 购买金币界面的回调
    /// </summary>
    void makeSuerDelegate()
    {
        Debug.Log("PowerPanel ------ makeSuerDelegate");
        AddPower();
    }

    void deleteDelegate()
    {
        Debug.Log("PowerPanel ------ deleteDelegate");
        GameObject.Destroy(gameObject);
    }
}
