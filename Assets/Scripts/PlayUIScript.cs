using UnityEngine;
using System.Collections;


///添加底部按钮的点击声音
///添加转换按钮声音
///添加暂停的声音
/*
 * @brief       绑定在PlayUI上
 * @desc        负责各个按钮的事件
 */
public class PlayUIScript : MonoBehaviour {


    private static PlayUIScript instance = null;
    public static PlayUIScript Instance { get { return instance; } }
    //根节点
    public GameObject uiRootGameObject = null;
	// 游戏暂停界面
	public GameObject gamePausePanel;

    void Awake()
    {
        originPosition = originObject.transform.position;
    }

    void Start()
    {
        instance = this;
        //获取数据
        crossNumber = UserInstanse.GetInstance().stonePubble_Num;
        fireNumber = UserInstanse.GetInstance().firePubble_Num;
        snowNumber = UserInstanse.GetInstance().snowPubble_Num;
        crushNumber = UserInstanse.GetInstance().stockPubble_Num;

        moneyNumber = UserInstanse.GetInstance().coinNum;
        customNumber = UserInstanse.GetInstance().chooseLevel_id + 1;
        ShowBottomLabelText();
        ShowTopLabelText();
        GetOriginValue();
    }
    void Update()
    {
        //获取数据
        crossNumber = UserInstanse.GetInstance().stonePubble_Num;
        fireNumber = UserInstanse.GetInstance().firePubble_Num;
        snowNumber = UserInstanse.GetInstance().snowPubble_Num;
        crushNumber = UserInstanse.GetInstance().stockPubble_Num;

        crossNumber = UserInstanse.GetInstance().stonePubble_Num;
        crossNumLabel.text = "" + crossNumber;
        fireNumber = UserInstanse.GetInstance().firePubble_Num;
        fireNumLabel.text = "" + fireNumber;
        snowNumber = UserInstanse.GetInstance().snowPubble_Num;
        snowNumLabel.text = "" + snowNumber;
        crushNumber = UserInstanse.GetInstance().stockPubble_Num;
        crushNumLabel.text = "" + crushNumber;

        moneyNumber = UserInstanse.GetInstance().coinNum;
        moneyLabel.text = "" + moneyNumber;
    }

    /*
     * @brief       暂停事件
     * @desc        :泡泡不再滑动
     * @desc        :每个泡泡执行的动作暂停:离子特效，掉落等
     * @desc        :发射泡泡停止运动
     */
    public void PauseButtonAction()
    {
        SoundManager.Instance.PlayButtonTouchSound();
        //滑动的泡泡停止
        SlidePlayPanel.Instance.PauseSlide();
        //弹出一个暂停框，其他的均不能点击
		// 2. 下一界面的初始化
		GameObject cur = Instantiate(gamePausePanel) as GameObject;
        cur.gameObject.transform.parent = uiRootGameObject.transform;
		cur.transform.localScale = new Vector3(1.0f, 1f, 1f);
    }

    #region 负责底下道具泡泡的发射以及数量统计
    /*
     * @brief       发射道具的按钮事件
     * @desc        绑定在Skillbutton上，根据不同的按钮，调用StrikeManager内的函数，将发射泡泡替换成道具泡泡
     */
    int lackType = -1;
    public void ShootPropPubbleFunction(GameObject propButton)
    {
        string buttomName = propButton.name;
        PubbleColorType colorType = PubbleColorType.PUBBLE_EMPTY_TYPE;
        lackType = -1;
        switch (buttomName)
        {
            case ConstantValue.Skill1Button:
                if (crossNumber > 0)
                    colorType = PubbleColorType.PUBBLE_CROSS_TYPE;
                else
                    lackType = 5;
                break;
            case ConstantValue.Skill2Button:
                if (fireNumber > 0)
                    colorType = PubbleColorType.PUBBLE_FIRE_TYPE;
                else
                    lackType = 2;
                break;
            case ConstantValue.Skill3Button:
                if (snowNumber > 0)
                    colorType = PubbleColorType.PUBBLE_SNOW_TYPE;
                else
                    lackType = 3;
                break;
            case ConstantValue.Skill4Button:
                if (crushNumber > 0)
                    colorType = PubbleColorType.PUBBLE_CRUSH_TYPE;
                else
                    lackType = 6;
                break;
            default:
                break;
        }
        SoundManager.Instance.PlayButtonTouchSound();
        if (colorType != PubbleColorType.PUBBLE_EMPTY_TYPE)
        {
            StrikeManager.Instance.ExecuteEmissionPropPubble(colorType);
        }
        else if (lackType > 1)
        { 
            //如果存在缺少的道具，弹出购买按钮
            PopBuyPropDlg();
        }
    }

    public GameObject buyPropDlg = null;
    void PopBuyPropDlg()
    {
        SlidePlayPanel.Instance.PauseSlide();
        if (buyPropDlg != null)
        {
            GameObject cur = Instantiate(buyPropDlg) as GameObject;
            cur.gameObject.transform.parent = uiRootGameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

            BuyTipPanel panel = cur.GetComponent<BuyTipPanel>();
            panel.InitTipType((BuyTipPanel.PropType)lackType);
            panel.buyTip_sureDelegate = BuyFinishProp;
            panel.buyTip_deleteDelegate = BuyFailProp;
        }
    }

    void BuyFailProp()
    {
        SlidePlayPanel.Instance.RestoreSlideType();
    }
    void BuyFinishProp()
    {
        SlidePlayPanel.Instance.RestoreSlideType();
        crossNumber = UserInstanse.GetInstance().stonePubble_Num;
        crossNumLabel.text = "" + crossNumber;
        fireNumber = UserInstanse.GetInstance().firePubble_Num;
        fireNumLabel.text = "" + fireNumber;
        snowNumber = UserInstanse.GetInstance().snowPubble_Num;
        snowNumLabel.text = "" + snowNumber;
        crushNumber = UserInstanse.GetInstance().stockPubble_Num;
        crushNumLabel.text = "" + crushNumber;
    }

    //四个道具泡泡的label
    public UILabel crossNumLabel;
    public UILabel fireNumLabel;
    public UILabel snowNumLabel;
    public UILabel crushNumLabel;
    public int crossNumber;
    public int fireNumber;
    public int snowNumber;
    public int crushNumber;

    void ShowBottomLabelText()
    { 
        crossNumLabel.text = "" + crossNumber;
        fireNumLabel.text = "" + fireNumber;
        snowNumLabel.text = "" + snowNumber;
        crushNumLabel.text = "" + crushNumber;
    }

    public void ChangePropLabelNum(PubbleColorType colorType)
    {
        switch (colorType)
        { 
            case PubbleColorType.PUBBLE_CROSS_TYPE:
                crossNumber--;
                UserInstanse.GetInstance().stonePubble_Num = crossNumber;
                crossNumLabel.text = "" + crossNumber;
                break;
            case PubbleColorType.PUBBLE_FIRE_TYPE:
                fireNumber--;
                UserInstanse.GetInstance().firePubble_Num = fireNumber;
                fireNumLabel.text = "" + fireNumber;
                break;
            case PubbleColorType.PUBBLE_SNOW_TYPE:
                snowNumber--;
                UserInstanse.GetInstance().snowPubble_Num = snowNumber;
                snowNumLabel.text = "" + snowNumber;
                break;
            case PubbleColorType.PUBBLE_CRUSH_TYPE:
                crushNumber--;
                UserInstanse.GetInstance().stockPubble_Num = crushNumber;
                crushNumLabel.text = "" + crushNumber;
                break;
            default:
                break;
        }
    }

    /*
     * @brief       交换按钮的事件
     */
    public void ExChangePreparePubble()
    {
        SoundManager.Instance.PlayButtonTouchSound();
        StrikeManager.Instance.ExChangeTwoPubbles();
    }
    #endregion 负责底下道具泡泡的发射以及数量统计

    #region 负责顶部几个label的显示
    public UILabel scoreLabel;
    public UILabel barrLabel;
    public UILabel moneyLabel;
    int scoreNumber = 0;
    int moneyNumber;
    int customNumber;

    void ShowTopLabelText()
    {
        ChangeBarr(customNumber);
        ChangeMoney();
        ChangeScore();
    }

    public void ChangeScore()
    {
        scoreLabel.text = "" + scoreNumber;
    }

    public void ChangeBarr(int barr)
    {
        barrLabel.text = "" + barr;
    }

    public void ChangeMoney()
    {
        moneyLabel.text = "" + moneyNumber;
        UserInstanse.GetInstance().coinNum = moneyNumber;
    }

    //动态显示分数
    public void LerpShowScore(int num )
    {
        scoreNumber += num;
        ChangeScore();
        //StartCoroutine(ChangeMyScore());
    }
    IEnumerator ChangeMyScore()
    {
        yield return null;
    }

    //调用一次 + 1
    public void ChangeDiamondNumber()
    {
        moneyNumber += 1;
        ChangeMoney();
    }

    public void SaveInfoToUserInstance()
    {
        UserInstanse.GetInstance().SuccessSaveCurrentLevel(PlayModelLogic.Instance.GetCommentStarNum(), scoreNumber, moneyNumber);
        UserInstanse.GetInstance().savePubbleNum(crossNumber, fireNumber, crushNumber, snowNumber);
    }
    public void SaveLoseInfoToUserInstance()
    {
        UserInstanse.GetInstance().FialSaveCurrentLevel(scoreNumber, moneyNumber);
        UserInstanse.GetInstance().savePubbleNum(crossNumber, fireNumber, crushNumber, snowNumber);
    }

    public void SaveInfoWhenLose()
    {
        UserInstanse.GetInstance().savePubbleNum(crossNumber, fireNumber, crushNumber, snowNumber);
    }

    #endregion 负责顶部几个label的显示

    #region 负责章鱼的转动
    public GameObject fishGameObject;
    //原点gameObject
    public GameObject originObject;
    public Vector3 originPosition;

    float minusLimit = -23.73f;
    float uprightLimit = 23.73f;

    //transform 
    Vector3 fishOriginPos;
    Quaternion fishOriginQuate;

    void GetOriginValue()
    {
        fishOriginPos = fishGameObject.transform.position;
        fishOriginQuate = fishGameObject.transform.rotation;
    }
    /*
     * @brief       章鱼旋转
     * @param       touchPoint 点击的世界坐标点
     * @param       shoot 当为true 的时候发射泡泡，默认为false        
     */
    public IEnumerator RotateFishGameObject(Vector3 inputPos,bool shoot = false)
    {
        if (!StrikeManager.Instance.GetShootState())
        {
            //旋转方案，回复到原点，之后再偏移
            Vector3 upDir = Vector3.up;
            Vector3 screenPosition = Camera.main.ScreenToWorldPoint(inputPos);

            Vector3 touchDir = screenPosition - originPosition;
            Vector3 touchNormal = touchDir.normalized;

            float cosValue = Vector3.Dot(upDir, touchNormal);
            //得到弧度
            float surpDeg = Mathf.Acos(cosValue) * Mathf.Rad2Deg;
            //回复原位置
            fishGameObject.transform.position = fishOriginPos;
            fishGameObject.transform.rotation = fishOriginQuate;
            //旋转
            if (touchNormal.x > 0)
                surpDeg = -surpDeg;

            if (surpDeg < minusLimit)
                surpDeg = minusLimit;
            else if (surpDeg > uprightLimit)
                surpDeg = uprightLimit;


            if (!float.IsNaN(surpDeg))
            {
                fishGameObject.transform.RotateAround(originPosition, new Vector3(0, 0, 1), surpDeg);
            }
            //将发射泡泡器的位置定位到 新坐标
            StrikeManager.Instance.ResetStrikePosition();

            yield return new WaitForSeconds(0.1f);
            if (shoot)
            {
                StrikeManager.Instance.ShootPubble(inputPos);
            }
        }
    }

    #endregion 负责章鱼的转动

    #region 负责战斗胜利后的道具奖励
    public GameObject goodsTip;
    
    // 用于多个道具奖励
    public void showGoodsTips(object [] titles)
    {
        GameObject tip = Instantiate(goodsTip) as GameObject;
        tip.gameObject.transform.parent = gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);
        tip.GetComponent<GoodsTipManager>().setTipTitles(titles);

    }
    //用于一个道具的购买
    public void showGoodsTip(string title)
    {
        GameObject tip = Instantiate(goodsTip) as GameObject;
        tip.gameObject.transform.parent = gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);
        tip.GetComponent<GoodsTipManager>().setTipTitle(title);

    }

    #endregion
}
