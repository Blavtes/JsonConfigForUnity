using UnityEngine;
using System.Collections;



//新手教学枚举类型
public enum TuitionType
{ 
    TUITION_NONE_TYPE = 0,              //没有新手指引


    TUITION_FIRST_TOUCH_GREEN_TYPE,     //第一关，点击发射绿色
    TUITION_FIRST_TOUCH_BLUE_TYPE,      //第一关，点击发射蓝色
    TUITION_FIRST_TOUCH_EXCHANGE_TYPE,  //第一关，点击交换


    TUITION_SECOND_TOUCH_THUNDER_TYPE,  //第二关，强制点击闪电泡泡
    TUITION_THIRD_TOUCH_PROP_TYPE,      //第三关，强制使用道具，如果没有该指引算完成


    TUTION_FINISH_TYPE,                 //完成新手指引
};


//指引提示
public enum TuitionTipType
{ 
    TUITION_NONE_TIP_TYPE,              //没有提示
    TUITION_TIP_DROP_TYPE,              //提示道具

    TUITION_TIP_STEP_TYPE,              //步数关卡提示
    TUITION_TIP_REQUIRE_TYPE,           //步数要求

    TUITION_TIP_LIMIT_TYPE,             //描述极限关卡


    TUITION_TIP_PROP_PER_TYPE,          //死亡线提示用道具


};


/*
 * @brief       管理新手教学
 */
public class TuitionManager : MonoBehaviour {

    private static TuitionManager m_Instance = null;
    public static TuitionManager Instance { get { return m_Instance; } }


    //当前教学类型，默认没有，在start函数内获取
    TuitionType currentTuitionType = TuitionType.TUITION_NONE_TYPE;
    //当前提示类型，默认没有，在start函数内获取
    TuitionTipType currentTipType = TuitionTipType.TUITION_NONE_TIP_TYPE;
    //当前小手
    GameObject tuitionHandeObject = null;
    //当前遮挡曾
    BoxCollider tuitionBoxCollider = null;

    //
    const string tuitionKey = "TuitionKey";

    void Awake()
    {
        m_Instance = this;
        tuitionBoxCollider = GameObject.Find("BoxCollider").GetComponent<BoxCollider>();
        tuitionHandeObject = GameObject.Find("AllParent");
        DontDestroyOnLoad(transform.gameObject);
    }

	void Start () {
        currentTuitionType = (TuitionType)PlayerPrefs.GetInt(tuitionKey);
        markLimitTip = PlayerPrefs.GetInt(limitKey);
        markShowChanllageTip = PlayerPrefs.GetInt(showChanllangeKey);
        SetTuitionDisable();
    }

    void SetTuitionActive()
    {
        //放在前面的这个会优先 接收点击事件，妈的长知识了
        tuitionHandeObject.SetActive(true);
        tuitionBoxCollider.enabled = true;
    }

    void SetTuitionDisable()
    {
        tuitionBoxCollider.enabled = false;
        tuitionHandeObject.SetActive(false);
    }

    #region 控制小手的出现

    /*
    //当前是否正在显示指引 ，显示则 不再 做检测
    bool showIngTuition = false;

    bool showIngTip = false;
    void Update()
    { 
        //判断当前的指引

        //判断当前的提示

    }
    */

    /*
     * @brief       显示当前的指引
     * @desc        当前场景为levelscene
     * @desc        当前指引没有 ，则从头显示
     * 
     * @desc        当前场景为playscene
     * @desc        当前指引为start 则，指点点击
     * 
     * @desc        当前为playscene
     * @desc        上一指引为 TUITION_SECOND_TOUCH_START_TYPE ，则指引使用穿刺，如果没有，则算完成
     * 
     * @desc        
     */
    /*
    void CheckCurrentTuition()
    { 
        //获取当前场景名称
        string sceneName = Application.loadedLevelName;
        if (!showIngTuition && sceneName == ConstantValue.LevelSceneName && currentTuitionType == TuitionType.TUITION_NONE_TYPE)
        { 
            //显示第一个新手指引
            currentTuitionType = TuitionType.TUITION_FIRST_TOUCH_ONE_TYPE;
        }
        else if()


    }
     */

    #endregion 控制小手的出现

    #region 各个小手的位置
    const string first_one = "Item0";
    const string enterGameButton = "StartBtn";
    const string touchGreenShoot = "TuitionLeftTouch";
    const string touchBlueShoot = "TuitionRightTouch";
    const string exChangeButton = "ChangeButton";
    const string forceNextButton = "nextBtn";


    const string selectPropButton = "Prop2Btn";


    const string crossPropButton = "Skill1Button";


    #endregion 各个小手的位置


    #region 对外接口

    public void CreateStartTuition()
    {
        if (currentTuitionType == TuitionType.TUITION_SECOND_TOUCH_THUNDER_TYPE)
        {
            getPositionObject = false;
            positionObject = null;
            SetTuitionActive();
            StartCoroutine(PrepareCurrentTuition());
        }
    }

    public void CreateTuition()
    {
        //判断当前是否可以进行新手指引
        if ((int)currentTuitionType < (int)TuitionType.TUTION_FINISH_TYPE && currentTuitionType != TuitionType.TUITION_SECOND_TOUCH_THUNDER_TYPE)
        {
            getPositionObject = false;
            positionObject = null;
            SetTuitionActive();
            StartCoroutine(PrepareCurrentTuition());
        }
    }
    #endregion 对外接口

    #region 处理显示tuition
    //是否获得到了位置gameobject
    bool getPositionObject = false;
    GameObject positionObject = null;
    IEnumerator PrepareCurrentTuition()
    {
        //如果没有获得到则一直循环获得
        while (!getPositionObject)
        {
            switch (currentTuitionType)
            {
                case TuitionType.TUITION_NONE_TYPE:
                    currentTuitionType = TuitionType.TUITION_FIRST_TOUCH_GREEN_TYPE;
                    break;
                case TuitionType.TUITION_FIRST_TOUCH_GREEN_TYPE:
                    //
                    positionObject = GameObject.Find(touchGreenShoot);
                    break;
                case TuitionType.TUITION_FIRST_TOUCH_BLUE_TYPE:
                    //
                    positionObject = GameObject.Find(touchBlueShoot);
                    break;
                case TuitionType.TUITION_FIRST_TOUCH_EXCHANGE_TYPE:
                    //
                    positionObject = GameObject.Find(exChangeButton);
                    break;

                case TuitionType.TUITION_SECOND_TOUCH_THUNDER_TYPE:
                    //
                    yield return new WaitForSeconds(0.5f);
                    positionObject = GameObject.Find(selectPropButton);
                    break;
                case TuitionType.TUITION_THIRD_TOUCH_PROP_TYPE:
                    //
                    positionObject = GameObject.Find(crossPropButton);
                    break;
                default:
                    break;
            }
            
            if (positionObject != null)
            {
                //如果得到了，则不再循环
                getPositionObject = true;
            }
        }
        ShowCurrentTuition();
        yield return null;
    }

    void ShowCurrentTuition()
    {
        //设置小手位置
        tuitionHandeObject.transform.position = positionObject.transform.position;
    }

    /*
     * @brief       指引的点击事件
     * @desc        通过当前的position 来确定 点击事件
     */
    public void TouchTuitionButtonAction()
    {
        switch (currentTuitionType)
        {
            case TuitionType.TUITION_FIRST_TOUCH_GREEN_TYPE:
                //发射绿色泡泡
                if (!TouchManager.Instance.CanPlayShoot())
                {
                    return;
                }
                break;

            case TuitionType.TUITION_FIRST_TOUCH_BLUE_TYPE:
                //发射蓝色泡泡
                if (!TouchManager.Instance.CanPlayShoot()) return;
                break;

            case TuitionType.TUITION_FIRST_TOUCH_EXCHANGE_TYPE:
                //交换泡泡
                StrikeManager.Instance.ExChangeTwoPubbles();
                break;

            case TuitionType.TUITION_SECOND_TOUCH_THUNDER_TYPE:
                {
                    UIEventListener eventListener = positionObject.GetComponent<UIEventListener>();
                    eventListener.onClick(positionObject);
                }
                break;

            case TuitionType.TUITION_THIRD_TOUCH_PROP_TYPE:
                //
                PlayUIScript.Instance.ShootPropPubbleFunction(positionObject);
                break;

            default:
                break;
        }
        SetTuitionDisable();
        ChangeNextType();
    }

    void ChangeNextType()
    {
        int nextType = (int)currentTuitionType + 1;
        currentTuitionType = (TuitionType)nextType;
        //保存当前的状态

        //执行下一个
        if (currentTuitionType == TuitionType.TUITION_FIRST_TOUCH_GREEN_TYPE ||         //点击消除绿色泡泡
            currentTuitionType == TuitionType.TUITION_SECOND_TOUCH_THUNDER_TYPE ||      //下一关，之前会消除所有泡泡
            currentTuitionType == TuitionType.TUITION_THIRD_TOUCH_PROP_TYPE)
        {
            //此处需要切换场景,不显示内容
            SetTuitionDisable();
        }
        else
        {
            //显示下一步
            CreateTuition();
        }
        //保存一下
        SaveTuitionProgress();
        if (currentTuitionType == TuitionType.TUTION_FINISH_TYPE)
        {
            SetTuitionDisable();
        }
    }

    void SaveTuitionProgress()
    { 
        //当为 选择闪电，或者 穿刺，或者完成的时候保存
        if (currentTuitionType == TuitionType.TUITION_SECOND_TOUCH_THUNDER_TYPE ||
            currentTuitionType == TuitionType.TUITION_THIRD_TOUCH_PROP_TYPE ||
            currentTuitionType == TuitionType.TUTION_FINISH_TYPE)
        {
            PlayerPrefs.SetInt(tuitionKey,(int)currentTuitionType);
        }
    }
    #endregion 


    #region 负责相关的出现泡泡
    //第一次出现绿色,其次蓝色，蓝色，红色
    int countTime = 0;
    public PubbleColorType GetCurrentColorType()
    {
        PubbleColorType pubbleType = PubbleColorType.PUBBLE_EMPTY_TYPE;
        if (currentTuitionType < TuitionType.TUITION_SECOND_TOUCH_THUNDER_TYPE)
        {
            countTime++;
            switch (countTime)
            {
                case 1:
                    pubbleType = PubbleColorType.PUBBLE_GREEN_TYPE;
                    break;
                case 2:
                    pubbleType = PubbleColorType.PUBBLE_BLUE_TYPE;
                    break;
                case 3:
                    pubbleType = PubbleColorType.PUBBLE_BLUE_TYPE;
                    break;
                case 4:
                    pubbleType = PubbleColorType.PUBBLE_RED_TYPE;
                    break;
                default:
                    break;
            }
        }
        return pubbleType;
    }


    #endregion 负责相关的出现泡泡


    #region 显示提示

    public GameObject tipChanllageObject = null;
    public UILabel tipChangleLabel = null;
    int markShowChanllageTip = 0;
    const string showChanllangeKey = "ChanllageKey";
    int markLimitTip = 0;
    const string limitKey = "LimitKey";
    public GameObject tipHandObject = null;

    enum ShowType
    { 
        NONE_TYPE = 0,
        CHANLLANGE_TYPE = 1,
        LIMIT_TYPE = 2,
    };

    ShowType showCurrentType = ShowType.NONE_TYPE;
    public void ShowChanllangeTip()
    {
        if (markShowChanllageTip == 0)
        {
            markShowChanllageTip = 1;
            showCurrentType = ShowType.CHANLLANGE_TYPE;
            tipChanllageObject.SetActive(true);
            StartCoroutine(ShowThoseTip(ConstantString.TUITION_TIP_CHALLENGE_TXT));
        }
    }

    public void ShowLimitTip()
    {
        if (markLimitTip == 0)
        {
            markLimitTip = 1;
            showCurrentType = ShowType.LIMIT_TYPE;
            tipChanllageObject.SetActive(true);
            tipHandObject.SetActive(false);
            StartCoroutine(ShowThoseTip(ConstantString.TUITION_TIP_LIMIT_TXT));
        }
    }

    IEnumerator ShowThoseTip(string text)
    {
        tipChangleLabel.text = text;
        yield return new WaitForSeconds(5f);
        //
        HiddenTip();
    }

    void HiddenTip()
    {
        tipChanllageObject.SetActive(false);
        if (showCurrentType == ShowType.CHANLLANGE_TYPE)
        {
            //存储
            PlayerPrefs.SetInt(showChanllangeKey, 1);
        }
        else
        {
            PlayerPrefs.SetInt(limitKey, 1);
        }
        
    }

    #endregion 显示提示




}
