using UnityEngine;
using System.Collections;

public class CoinScrollView : MonoBehaviour
{

    public GameObject buyBtn0;
    public GameObject buyBtn1;
    public GameObject buyBtn2;

    public GameObject costDaimondPanel;
    public GameObject[] CoinDesLabels;

    int cout = 0; // 标记点击的按钮标记

    void Awake()
    {
        UIEventListener.Get(buyBtn0).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn1).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn2).onClick = buyBtnClick;

        for (int i = 0; i < CoinDesLabels.Length; i++)
        {
            GameObject obj = CoinDesLabels[i] as GameObject;
            obj.GetComponent<UILabel>().text = ConstantString.LevelCoinTitles[i];
        }


    }
    // 道具滚动视图的购买金币按钮
    void buyBtnClick(GameObject button)
    {
        Debug.Log("点击购买金币按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        // 1. 扣除buyBtn 6个字符 剩下的为本道具的id
        cout = int.Parse(button.name.Substring(6));
        Debug.Log(" 扣除buyBtn 6个字符 剩下的为本道具的id----" + cout);

        // 2. 购买提示界面的初始化
        GameObject cur = Instantiate(costDaimondPanel) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        cur.gameObject.transform.parent = root.gameObject.transform;
        cur.transform.localScale = new Vector3(1, 1, 1);

        CostDaimondPanel panel = cur.GetComponent<CostDaimondPanel>();
        panel.InitTipType((RechargeInstanse.PayType)cout);
        panel.costDaimod_MakeSuerDelegate = makeSureDelegate;
    }
    void makeSureDelegate()
    {
        Debug.Log("makeSureDelegate-----");
        string cur = ConstantString.LevelCoinTips[cout - 3] as string;
        showGoodsTip(cur);
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
}
