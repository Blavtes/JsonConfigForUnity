using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * @brief       玩法模式
 * @desc        普通模式直接在PlayLogic内实现了，此处管理极限跟挑战
 */
public enum PlayModelStyle 
{
    PLAY_NORMAL_STYLE = 1,      //普通模式
    PLAY_CHANLLENGE_STYLE = 2,  //挑战模式： 限制步数
    PLAY_LIMIT_STYLE = 3,       //极限模式： 每八步降落两行
};

public class PlayModelLogic : MonoBehaviour
{
    //单例便于获取
    private static PlayModelLogic m_Instance;
    public static PlayModelLogic Instance { get { return m_Instance; } }

    //记录模式:默认普通
    public PlayModelStyle playModeType = PlayModelStyle.PLAY_NORMAL_STYLE;

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    { 
        //获取当前关卡的信息
        LevelInfo leveInfo = UserInstanse.GetInstance().LevelData[UserInstanse.GetInstance().chooseLevel_id];
        //关卡类型
        playModeType = (PlayModelStyle)leveInfo.type;
        //获取步数
        countCurrentSurpStep = leveInfo.step;
        //三星
        threeStarStandard = leveInfo.star3;
        twoStarStandard = leveInfo.star2;
        oneStarStandard = leveInfo.star1;
        //挑战模式：显示步数
        if (playModeType == PlayModelStyle.PLAY_CHANLLENGE_STYLE)
        { 
            //显示步数
            labelGdSprite.SetActive(true);
            labelScript = stepNumLabel.GetComponent<UILabel>() as UILabel;
            labelScript.text = "" + countCurrentSurpStep;
        }
    }

    /*
     * @brief       处理不同的模式:调用时机 ---每次发射就会调用(strike内),道具泡泡也会算在 步数内
     * @desc        如果当前为Normal，则不做操作，就是现在的操作
     * @desc        Limit:则需要修改步数显示，同时查看步数是否到了
     * @desc        Chanllenge:则需要修改步数显示，同时不断生成新的泡泡
     */
    public void ManageKindModelLogic()
    {
        switch (PlayModelLogic.Instance.playModeType)
        {
            case PlayModelStyle.PLAY_NORMAL_STYLE:
                //不做操作
                break;
            case PlayModelStyle.PLAY_LIMIT_STYLE:
                //极限模式
                LimitPlayFunction();
                break;
            case PlayModelStyle.PLAY_CHANLLENGE_STYLE:
                //此处负责：修改步数，同时检测步数
                //挑战模式的生成新泡泡，在一个协程内实现,不在此处调用
                ChanllangePlayFunction();
                break;
            default:
                break;
        }
    }
	
    //记录当前剩余的步数
    public int countCurrentSurpStep = 0;
    //记录当前发射的步数
    public int countCurrentShootStep = 0;
    //警告限制步数
    const int warningStepNum = 3;
    //承载Label的背景sprite，当前是直接隐藏
    public GameObject labelGdSprite = null; 
    //显示步数的Label
    public GameObject stepNumLabel = null;
    //UILabel脚本
    private UILabel labelScript = null;



    #region 挑战模式的玩法
    /*
     * @brief       挑战模式
     * @desc        步数 - 1，然后检测是否达到了 3 步 ，达到了给警告
     * @desc        修改界面上的步数显示
     */
    void ChanllangePlayFunction()
    {
        if (countCurrentSurpStep > 0)
        {
            countCurrentSurpStep--;
            countCurrentShootStep++;
        }
        //如果剩余泡泡到达了界限值，则弹出警告
        if (countCurrentSurpStep == warningStepNum)
            PopWarningTip();
        //检测步数
        CheckGameStepNumInChallenge();
        //修改当前剩余的泡泡步数
        ShowCurrentStepNum();
    }

    /*
     * @brief       当步数为3的时候弹出警告
     * @desc        挑战模式下才会出现
     */
    void PopWarningTip()
    {
        //现在不实现
    }

    /*
     * @brief       修改界面上的步数显示
     * @desc        挑战模式下才会出现
     */
    void ShowCurrentStepNum()
    {
        if (labelGdSprite != null)
        {
            labelScript.text = "" + countCurrentSurpStep;
        }
    }
    #endregion 挑战模式的玩法

    #region 极限模式的玩法
    /*
     * @brief       极限模式
     * @desc        不断生成新泡泡，没八步生成 两行
     */
    void LimitPlayFunction()
    {
        countCurrentShootStep++;
        CreateNewPubbles();
    }

    /*
     * @brief       协程控制新生成的泡泡
     * @desc        在脚本启用时调用
     * @desc        只有在挑战模式下才会生成新泡泡
     * @desc        由一个专门的类去重新生成 这两行泡泡，此处只负责创建
     */
    const int perStepNum = 13;
    void CreateNewPubbles()
    {
        //每当发射8次，则降落一次 ，一行
        if (countCurrentShootStep % perStepNum == 0)
        {
            //获取要添加的两行数据
            SlidePlayPanel.Instance.StartCreateNewPubble(UserInstanse.GetRandomInfo());
        }
    }
    #endregion 极限模式的玩法

    #region 负责检测挑战模式下 步数是否没有了，以及购买等
    /*
     * @brief       负责检测挑战模式下是否游戏失败
     * @desc        没有之后，则弹出购买步数对话框
     */
    void CheckGameStepNumInChallenge()
    {
        if (playModeType == PlayModelStyle.PLAY_CHANLLENGE_STYLE)
        {
            if (countCurrentSurpStep <= 0)
            {
                //当前没有步数了，停止滑动
                SlidePlayPanel.Instance.ChangeSlideStypeToStop();
                //弹出购买框
                StartCoroutine(GameManager.Instance.PopBuyStepDialog());
            }
        }
    }

    /*
     * @brief       购买成功
     */
    public void SucceeBuyStep()
    {
        //修改回玩的状态
        GameManager.Instance.SetStateToPlaying();
        countCurrentSurpStep = 10;
        SlidePlayPanel.Instance.StarEndBuyStepMove();
        ShowCurrentStepNum();
    }
    #endregion 负责检测挑战模式下 步数是否没有了，以及购买等


    #region 负责检测玩家当前关卡的评星
    //三星标准
    int threeStarStandard;
    // 两星标准
    int twoStarStandard;
    //一星标准
    int oneStarStandard;
    //记录游戏开始之后总共用的时间
    float countTime = 0f;
    void LateUpdate()
    {
        //如果当前是
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentStateIsPlayingState())
            {
                countTime += Time.deltaTime;
            }
        }
    }

    //根据当前评论星星
    public int GetCommentStarNum()
    {
        switch (playModeType)
        { 
            case PlayModelStyle.PLAY_NORMAL_STYLE:
            case PlayModelStyle.PLAY_LIMIT_STYLE:
                CommentTimeStandard();
                break;
            case PlayModelStyle.PLAY_CHANLLENGE_STYLE:
                CommentStepNumStandard();
                break;
            default:
                break;
        }
        return currenStarNum;
    }

    private int currenStarNum = 0;
    //用时间衡量的标准
    void CommentTimeStandard()
    {
        currenStarNum = 1;
        if (countTime < threeStarStandard)
        {
            currenStarNum = 3;
        }
        else if (countTime < twoStarStandard)
        {
            currenStarNum = 2;
        }
    }


    void CommentStepNumStandard()
    {
        currenStarNum = 1;
        if (countCurrentSurpStep > threeStarStandard)
        {
            currenStarNum = 3;
        }
        else if(countCurrentSurpStep > twoStarStandard)
        {
            currenStarNum = 2;
        }
    }

    #endregion 负责检测玩家当前关卡的评星
}
