using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责金钱购买钻石界面
 * @Author      king
 * @date        2014-10-27 
 *  重构 只用于金币的购买
 */

public class CostDaimondPanel : MonoBehaviour
{

    public GameObject OkBtn;
    public GameObject DeleteBtn;

    public delegate void CostDaimod_MakeSuerDelegate();
    public  CostDaimod_MakeSuerDelegate costDaimod_MakeSuerDelegate = null;

    public delegate void CostDaimod_DeleteDelegate();
    public CostDaimod_DeleteDelegate costDaimod_deleteDelegate = null;
   
    int[] coinCost = {800,3000,12000 };
    int[] coinNum = { 3,6,12};

    // 上一个出现的界面
    public GameObject preObject = null;

    // 当前选择的类型
    RechargeInstanse.PayType chooseType = RechargeInstanse.PayType.PayType_No;
    
    void Awake()
    {
        UIEventListener.Get(OkBtn).onClick = okBtnClick;
        UIEventListener.Get(DeleteBtn).onClick = deleteBtnClick;
    }
    public GameObject labelDes;
    public void InitTipType(RechargeInstanse.PayType type)
    {
        chooseType = type;
        labelDes.GetComponent<UILabel>().text = ConstantString.LevelCostTitle[(int)type -3];
    }

    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        Debug.Log("点击关闭按钮----  button name :" + button.name);

        SoundManager.Instance.PlayButtonTouchSound();

        if (costDaimod_deleteDelegate != null)
            costDaimod_deleteDelegate();

        GameObject.Destroy(gameObject);
    }

    // 确认按钮的点击事件响应
    void okBtnClick(GameObject button)
    {
        Debug.Log("点击确认按钮---- 需要花费金钱----- " + chooseType);
        SoundManager.Instance.PlayButtonTouchSound();

        RechargeInstanse.Instance.rechargeSuccessDelegate = rechargeSuccessFunc;
        RechargeInstanse.Instance.rechargeFailDelegate = rechargeFailFunc;
        RechargeInstanse.Instance.RechargeMoneyFunction((RechargeInstanse.PayType)chooseType);   
    }

    // 购买成功回调
    void rechargeSuccessFunc()
    {

        UserInstanse.GetInstance().coinNum += coinCost[(int)chooseType-3];

        if (preObject) { preObject.active = true; }

        if (costDaimod_MakeSuerDelegate != null)
            costDaimod_MakeSuerDelegate();

        GameObject.Destroy(gameObject);
    }
    // 购买失败回调
    void rechargeFailFunc()
    {
        Debug.Log("购买失败回调---------------");
        if (costDaimod_deleteDelegate != null)
            costDaimod_deleteDelegate();

        GameObject.Destroy(gameObject);
    }
}
