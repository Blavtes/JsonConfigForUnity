using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
 /// 单例模式的实现

/*
 * @brief       负责玩家充值信息
 * @Author      King
 * @date        2014-11- 11
 * @desc        关于玩家的充值信息
 */
public class RechargeInstanse : MonoBehaviour
{
    private static RechargeInstanse mInstance = null;
    public static RechargeInstanse Instance { get { return mInstance; } }


	public  GameObject contentPanel;



    #region  awake方法 start方法
    void Awake()
    {
        mInstance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        mInstance = this;
    }
    #endregion

    /*
     * @brief      花钱支付类型
     */
    public enum PayType
    {
        PayType_No = 0,      //没有泡泡,即策划给的空值
        PayType_One = 1,      //福利礼包
        PayType_Two = 2,      //促销大礼包
        PayType_Three = 3,    //800钻石
        PayType_Four = 4,     //3000钻石
        PayType_Five = 5,     //12000钻石
        PayType_Six = 6,      //开启交换
    };
    // 当前选择的充值类型
    private int CurrentType ;

    #region 方法回调
    // 充值成功回调
    public delegate void RechargeSuccessDelegate();
    public RechargeSuccessDelegate rechargeSuccessDelegate = null;

    // 充值失败回调
    public delegate void RechargeFailDelegate();
    public RechargeFailDelegate rechargeFailDelegate = null;

    #endregion
	private GameObject purcharsePanel;
	bool IsShow = false;
    public void RechargeMoneyFunction(PayType type)
    {

        Debug.Log("RechargeMoneyFunction--------  充值类型------------" + (int)type);

        CurrentType =  (int)type;

		purcharsePanel = Instantiate(contentPanel) as GameObject;
		GameObject root = GameObject.Find("UI Root");
		purcharsePanel.gameObject.transform.parent = root.gameObject.transform;
		purcharsePanel.transform.localScale = new Vector3(1, 1, 1);

      	SDK.CallFunction_iOS_PayGods(CurrentType);
    }


    string payResult = null;
    public void PayResultSuccessMethod(string resultType)
    {
        payResult = resultType;
        Debug.Log("RechargeInstanse-----RechargeSuccess--------充值成功回调");
        if (rechargeSuccessDelegate != null)
        {
            rechargeSuccessDelegate();
        }
    }
	public void SetPanelActive(string resultType)
    {
		GameObject.Destroy (purcharsePanel);
		Debug.Log("RechargeInstanse-----SetPanelActive-------- GameObject.Destroy (purcharsePanel);");
    }

}
