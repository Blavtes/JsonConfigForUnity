using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
/*
 * @brief       当前负责购买七折礼包 五折礼包 以及6中道具泡泡
 * @Author      king
 * @date        2014-11-11
 * @desc        当购买道具金币不足时 会弹出购买钻石的界面 并实现其回调 
 *  重构 只用于道具的购买
 */

public class BuyTipPanel : MonoBehaviour
{
    // 花费金币购买钻石panel
    public GameObject costDaimondPanel;

    public GameObject OkBtn;
    public GameObject DeleteBtn;

    public GameObject TipText;

    PropType chooseType = PropType.PropType_No;

    public delegate void MakeSureButtonDelegate();
    public  MakeSureButtonDelegate buyTip_sureDelegate = null;

    public delegate void DelegateButtonDelegate();
    public  DelegateButtonDelegate buyTip_deleteDelegate = null;

    void Awake()
    {
        UIEventListener.Get(OkBtn).onClick = okBtnClick;
        UIEventListener.Get(DeleteBtn).onClick = deleteBtnClick;
    }

    /*
     * @brief      购买道具的类型
     */
    public enum PropType
    {
        PropType_No = -1,      //没有泡泡,即策划给的空值
        PropType_Seven = 0,    //七折优惠
        PropType_Color = 1,    //变色泡泡
        PropType_Fire = 2,     //火球泡泡
        PropType_Snow = 3,     //冰冻泡泡
        PropType_Light = 4,    //闪电泡泡
        PropType_Stone = 5,    //穿透泡泡
        PropType_Shock = 6,    //冲击泡泡
        PropType_Gift = 7,     //礼包
        PropType_Exchange = 8,   //交换功能
    };

    // 初始化弹窗类型
    public void InitTipType(PropType pType)
    {
       chooseType = pType;
       TipText.GetComponent<UILabel>().text = ConstantString.LevelBuyTipContent[(int)pType];
    }

    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        Debug.Log("点击关闭按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        if (buyTip_deleteDelegate != null)
            buyTip_deleteDelegate();

        GameObject.Destroy(gameObject);
    }

    // 增加UserDataInstanse 中存放的技能泡泡的数量
    void AddSkillPubbleNum(int num)
    {
        UserInstanse.GetInstance().colorPubble_Num += num;
        UserInstanse.GetInstance().lightPubble_Num += num;
        UserInstanse.GetInstance().firePubble_Num  += num;
        UserInstanse.GetInstance().stonePubble_Num += num;
        UserInstanse.GetInstance().stockPubble_Num += num;
        UserInstanse.GetInstance().snowPubble_Num  += num;
    }

    // 确认按钮的点击事件响应
    void okBtnClick(GameObject button)
    {
        SoundManager.Instance.PlayButtonTouchSound();

        buyProp();//购买道具 
    }

    // 购买道具 并存入userInstanse中
    void buyProp()
    {
        Debug.Log("购买道具 并存入instanse 中---- " + chooseType);
        int userCoinNum = UserInstanse.GetInstance().coinNum;
        switch (chooseType)
        {
            case PropType.PropType_Gift: 
                {
                    Debug.Log("花费1元----购买福利礼包");
                    CallRechargeFunction(RechargeInstanse.PayType.PayType_One);
                }
                break;
            case PropType.PropType_Seven:
                {
                    Debug.Log("----12元 促销大礼包 ----------");
                    CallRechargeFunction(RechargeInstanse.PayType.PayType_Two);
                }
                break;
            case PropType.PropType_Color:
                {
                    if (userCoinNum >= 300)
                    {
                        UserInstanse.GetInstance().coinNum -= 300;
                        UserInstanse.GetInstance().colorPubble_Num += 5;
                        closeTipPanelOk();
                    }
                    else
                    {
                        ShowCoinCostPanel();
                    }
                }
                break;
            case PropType.PropType_Fire:
                {
                    if (userCoinNum >= 300)
                    {
                        UserInstanse.GetInstance().coinNum -= 300;
                        UserInstanse.GetInstance().firePubble_Num += 5;
                        closeTipPanelOk();
                    }
                    else
                    {
                        ShowCoinCostPanel();
                    }
                }
                break;
            case PropType.PropType_Snow:
                {
                    if (userCoinNum >= 300)
                    {
                        UserInstanse.GetInstance().coinNum -= 300;
                        UserInstanse.GetInstance().snowPubble_Num += 5;
                        closeTipPanelOk();
                    }
                    else
                    {
                        ShowCoinCostPanel();
                    }
                }
                break;
            case PropType.PropType_Light:
                {
                    if (userCoinNum >= 500)
                    {
                        UserInstanse.GetInstance().coinNum -= 500;
                        UserInstanse.GetInstance().lightPubble_Num += 5;
                        closeTipPanelOk();
                    }
                    else
                    {
                        ShowCoinCostPanel();
                    }
                }
                break;
            case PropType.PropType_Stone:
                {
                    if (userCoinNum >= 500)
                    {
                        UserInstanse.GetInstance().coinNum -= 500;
                        UserInstanse.GetInstance().stonePubble_Num += 5;
                        closeTipPanelOk();
                    }
                    else
                    {
                        ShowCoinCostPanel();
                    }
                }
                break;
            case PropType.PropType_Shock:
                {
                    if (userCoinNum >= 500)
                    {
                        UserInstanse.GetInstance().coinNum -= 500;
                        UserInstanse.GetInstance().stockPubble_Num += 5;
                        closeTipPanelOk();
                    }
                    else
                    {
                        ShowCoinCostPanel();
                    }
                }
                break;
            case PropType.PropType_Exchange:
                {
                    CallRechargeFunction(RechargeInstanse.PayType.PayType_Six);
                }
                break;
            default:
                break;
        }
    }

    // 点击确定 关闭购买提示界面
    void closeTipPanelOk()
    {
        if (buyTip_sureDelegate != null)
            buyTip_sureDelegate();

        GameObject.Destroy(gameObject);
    }

    /// 点击关闭按钮 关闭购买提示界面
    void closeTipPanelCancle()
    {
        if (buyTip_deleteDelegate != null)
            buyTip_deleteDelegate();
        GameObject.Destroy(gameObject);
    }

    /// 弹出购买金币界面 
    void ShowCoinCostPanel()
    { 
            GameObject cur = Instantiate(costDaimondPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

            cur.GetComponent<CostDaimondPanel>().preObject = gameObject;

            CostDaimondPanel panel = cur.GetComponent<CostDaimondPanel>();
            panel.InitTipType(RechargeInstanse.PayType.PayType_Three);
            panel.costDaimod_MakeSuerDelegate = makeSuerDelegate;
            panel.costDaimod_deleteDelegate = deleteDelegate;

            gameObject.active = false;
    }

    void CallRechargeFunction(RechargeInstanse.PayType type)
    {
        RechargeInstanse.Instance.rechargeSuccessDelegate = makeSuerDelegate;
        RechargeInstanse.Instance.rechargeFailDelegate = deleteDelegate;
        RechargeInstanse.Instance.RechargeMoneyFunction(type);
    }

    /// 购买金币界面的购买回调
    void makeSuerDelegate()
    {
        Debug.Log("BuyTipPanel ------ makeSuerDelegate");
        BuySuccessFunc();
    }

    /// 购买金币界面的取消回调
    void deleteDelegate()
    {
        Debug.Log("BuyTipPanel ------ deleteDelegate");
        closeTipPanelCancle();
    }

    void BuySuccessFunc()
    {
        switch (chooseType)
        {
            case PropType.PropType_Gift:
                {
                    Debug.Log("--------花费1元----购买超值大礼包");
                    UserInstanse.GetInstance().coinNum += 300;
                    AddSkillPubbleNum(3);
                    closeTipPanelOk();
                }
                break;
            case PropType.PropType_Seven:
                {
                    Debug.Log("----优惠大礼包----------12元----");
                    UserInstanse.GetInstance().coinNum += 6000;
                    AddSkillPubbleNum(15);
                    closeTipPanelOk();
                }
                break;
            case PropType.PropType_Color:
                {
                        Debug.Log("-------------300金币--购买颜色泡泡5个");
                        UserInstanse.GetInstance().coinNum -= 300;
                        UserInstanse.GetInstance().colorPubble_Num += 5;
                        closeTipPanelOk();
                }
                break;
            case PropType.PropType_Fire:
                {
                    Debug.Log("-------------300金币--购买火焰泡泡5个");
                        UserInstanse.GetInstance().coinNum -= 300;
                        UserInstanse.GetInstance().firePubble_Num += 5;
                        closeTipPanelOk(); 
                }
                break;
            case PropType.PropType_Snow:
                {
                    Debug.Log("--------------300金币--购买冰冻泡泡5个");
                    UserInstanse.GetInstance().coinNum -= 300;
                    UserInstanse.GetInstance().snowPubble_Num += 5;
                    closeTipPanelOk();
                }
                break;
            case PropType.PropType_Light:
                {
                    Debug.Log("------------花费500金币购买5个闪电泡泡");
                    UserInstanse.GetInstance().coinNum -= 500;
                    UserInstanse.GetInstance().lightPubble_Num += 5;
                    closeTipPanelOk();
                }
                break;
            case PropType.PropType_Stone:
                {
                    Debug.Log("-----------花费500金币购买5个穿透泡泡");
                    UserInstanse.GetInstance().coinNum -= 500;
                    UserInstanse.GetInstance().stonePubble_Num += 5;
                    closeTipPanelOk();
                }
                break;
            case PropType.PropType_Shock:
                {

                    Debug.Log("-----------花费500金币购买5个冲击泡泡");
                    UserInstanse.GetInstance().coinNum -= 500;
                    UserInstanse.GetInstance().stockPubble_Num += 5;
                    closeTipPanelOk();
                }
                break;
            case PropType.PropType_Exchange:
                {

                    Debug.Log("-----------花费3元购买永久更换");
                    closeTipPanelOk();
                }
                break;
            default:
                break;
        }
    }
}