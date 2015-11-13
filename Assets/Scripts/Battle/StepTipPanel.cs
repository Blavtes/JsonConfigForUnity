using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责购买步数提示界面的布局 以及事件相应
 * @Author      king
 * @date        2014-10-22
 * @desc        关于买步数提示提示框
 */

public class StepTipPanel : MonoBehaviour
{
    public GameObject stepPanel;
    public GameObject OkBtn;
    public GameObject DeleteBtn;

    void Awake()
    {
        UIEventListener.Get(OkBtn).onClick = okBtnClick;
        UIEventListener.Get(DeleteBtn).onClick = deleteBtnClick;
    }

    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        //不进行购买，则提示游戏失败
        SlidePlayPanel.Instance.SlideGameOver();
        GameObject.Destroy(gameObject);
    }

    // 花费金币购买钻石panel
    public GameObject costDaimondPanel;

    // 确认按钮的点击事件响应
    void okBtnClick(GameObject button)
    {
        int userCoinNum = UserInstanse.GetInstance().coinNum;

        if (userCoinNum >= 200)
        {
            makeSuerDelegate();
        }
        else
        {
            GameObject cur = Instantiate(costDaimondPanel) as GameObject;
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

    /// 开启交换回调
    void makeSuerDelegate()
    {
        Debug.Log("StepTipPanel  ------ 开启交换------- 成功");
        UserInstanse.GetInstance().coinNum -= 200;
         //进行购买
        PlayModelLogic.Instance.SucceeBuyStep();
        GameObject.Destroy(gameObject);
    }

    /// 开启交换的失败回调
    void deleteDelegate()
    {
        Debug.Log("StepTipPanel  ------ 开启交换------- 失败 -  未作任何操作");
        //不进行购买，则提示游戏失败
        SlidePlayPanel.Instance.SlideGameOver();
        GameObject.Destroy(gameObject);
    }
}

