using UnityEngine;
using System.Collections;

public class PropScrollView : MonoBehaviour {


    public GameObject costDaimondPanel;

    public GameObject buyBtn0;
    public GameObject buyBtn1;
    public GameObject buyBtn2;
    public GameObject buyBtn3;
    public GameObject buyBtn4;
    public GameObject buyBtn5;
    public GameObject buyBtn6;

    public GameObject[] PropDesLabels;
    public GameObject buyTipPanel;

    void Awake()
    {
        UIEventListener.Get(buyBtn0).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn1).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn2).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn3).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn4).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn5).onClick = buyBtnClick;
        UIEventListener.Get(buyBtn6).onClick = buyBtnClick;
        for (int i = 0; i < PropDesLabels.Length; i++)
        {
            GameObject obj = PropDesLabels[i] as GameObject;
            obj.GetComponent<UILabel>().text = ConstantString.LevelPropTitles[i];
        }
    }

    int cout = 0; // 标记点击的按钮标记
    // 道具滚动视图的购买道具按钮
    void buyBtnClick(GameObject button)
    {
        Debug.Log("点击购买道具按钮0----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        // 1. 扣除buyBtn 6个字符 剩下的为本道具的id
        cout = int.Parse(button.name.Substring(6));
        Debug.Log(" 扣除buyBtn 6个字符 剩下的为本道具的id----" + cout);

        // 2. 购买提示界面的初始化
        GameObject cur = Instantiate(buyTipPanel) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        cur.gameObject.transform.parent = root.gameObject.transform;
        cur.transform.localScale = new Vector3(1, 1, 1);

        BuyTipPanel panel = cur.GetComponent<BuyTipPanel>();
        panel.InitTipType((BuyTipPanel.PropType)cout);
        panel.buyTip_sureDelegate = makeSureDelegate;


   
    }
    void makeSureDelegate()
    {
        Debug.Log("makeSureDelegate-----");
        string cur = ConstantString.LevelPropTips[cout] as string;

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
}
