using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * @brief       负责playLogic的滑动
 * @desc        负责滑动:10.18做挑战模式跟极限模式进行代码重构
 * @desc        负责：检测游戏是否结束等
 * @date        10.18
 * 
 */
public class SlidePlayPanel : MonoBehaviour
{
    static private SlidePlayPanel mSlidePlayPanelInstance = null;
    static public SlidePlayPanel Instance { get { return mSlidePlayPanelInstance; } }

    void Awake()
    {
        mSlidePlayPanelInstance = this;
        //计算底部界限的Y坐标
        overLimitY = GameObject.Find(ConstantValue.OverLimitObject).transform.position.y + pubbleWorldMarginHeight/2f;
        thirdCutSpeedObject = GameObject.Find(ConstantValue.ThirdCutSpeedObject);
        thirdSpeedLimitY = thirdCutSpeedObject.transform.position.y;
    }
    void Start()
    {
        PrepareTargetPos();
    }

    
    //获取最低端的标记量
    //第一次在创建所有的泡泡的时候赋值，之后在统计所有数组脚本的时候（update内赋值），通过遍历获取最底下的泡泡
    public GameObject slideMostBottomObject = null;
    //滑落上端限制点
    const float limitWorldDis = 0.5f;
    //滑落下限制点
    const float stopLimitWordDis = 0.1f;
    //泡泡世界坐标下的两个之间的Y距离
    const float pubbleWorldMarginHeight = 0.1f;
    //快速滑动的速度
    float quickSpeed = 0.2f;
    //匀速滑动的速度
    public float constSpeed = 0.02f;
    //冰冻时间限制
    int limitSnowTime = 5;
    //记录最低端的界限Y坐标，负责检测游戏是否Over
    float overLimitY = 0;
    //获取顶部挡板的Object
    //GameObject topPanelParentObject = null;
    enum SlideType
    {
        SLIDE_SHOW_TYPE = 0,         //从下网上运动
        SLIDE_CONSTANT_TYPE ,        //匀速
        SLIDE_QUICK_TYPE ,           //快速
        SLIDE_SNOW_TYPE ,            //冰冻
        SLIDE_THIRD_CUT_SPEED_TYPE,  //第三减速带滑动
        SLIDE_CRUSH_TYPE ,           //冲击波：停止等待推板上来，然后一起运动
        SLIDE_CHALLANGE_TYPE ,       //挑战模式：生成两行泡泡，迅速下滑
        SLIDE_GOON_DISAPPEAR_TYPE,   //连消，退后半格
        SLIDE_HANDLE_STOP_TYPE,      //手动停止，当前是为购买步数做的停止操作
        SLIDE_PAUSE_STOP_TYPE,       //暂停滑动
        SLIDE_BUY_STEP_END_TYPE,     //购买步数后，向上滑动
        SLIDE_LOSE_STOP_TYPE ,       //游戏失败停止滑动
        SLIDE_WIN_STOP_TYPE ,        //游戏胜利停止滑动
        SLIDE_STOP_TYPE ,            //停止
    };
    SlideType pubbleSlideType = SlideType.SLIDE_SHOW_TYPE;
    //冰冻效果
    public GameObject snowEffectGameObject = null;
    public GameObject uiRootGameObject = null;
    public GameObject markShowSnowObject = null;
    //新的两行泡泡
    public Dictionary<int, PubbleRowInfo> newPubbleInfo;

    /*
     * @brief       创建完成后，所有的泡泡从下往上滑动展示左右泡泡
     * @desc        滑至limitWorldDis 处停止滑动，开始缓缓下落
     */
    public IEnumerator SlidePanelShowAllPubble()
    {
        //使用lerp方法
        float speedMove = 1.2f;
        //目标位置就是 至limitWorldDis 处
        Vector3 targetPos = new Vector3(transform.position.x, limitWorldDis, transform.position.z);
        //判断当前的 滑动类型:如果是展示，则展示
        while (pubbleSlideType == SlideType.SLIDE_SHOW_TYPE)
        {
            //使用Lerp方法
            transform.position = Vector3.Lerp(transform.position, targetPos, speedMove * Time.deltaTime);
            //如果快要接近目标位置的时候，修改游戏状态，使其可以发射泡泡了
            //当然后面可能会添加开始动画，那个时候需要再动画执行之后才修改 state
            if (GameManager.Instance.CurrentStateIsPrepare() && Mathf.Abs(targetPos.y - transform.position.y) < 0.3f)
            {
                GameManager.Instance.SetStateToPlaying();
            }
            //如果滑动小于 这个数值的时候，直接不再循环,修改状态到匀速滑动
            if (Mathf.Abs(targetPos.y - transform.position.y) < 0.02)
            {
                pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
            }
            yield return null;
        }
        //展示完成之后，开始进行匀速滑动
        StartCoroutine(ManageSlideAction());
        yield return null;
    }
    
    /*
     * @brief       管理滑动
     * @desc        起始，快速从底部上来，到limitdis 停止开始匀速下滑
     * @desc        最底下的泡泡，如果高于limitdis，则快速下落到stoplimitdis位置
     * @desc        
     */
    public IEnumerator ManageSlideAction()
    {
        while ((int)pubbleSlideType < (int)SlideType.SLIDE_STOP_TYPE)
        {
            //此处添加一个 检测最底端 的标记量 是否为空函数，为空则，重新赋值
            if (!GetBottomObjIsExit())
                CheckMostBottomTransform();
            if (slideMostBottomObject != null)
            {
                Transform bottomTransform = slideMostBottomObject.transform;
                switch (pubbleSlideType)
                {
                    case SlideType.SLIDE_SHOW_TYPE:
                        //展示状态：永远不会执行到这儿里，不做操作
                        SlideStartMoveType();
                        break;
                    case SlideType.SLIDE_QUICK_TYPE:
                        //快速滑动
                        SlideQuickType(bottomTransform);
                        break;
                    case SlideType.SLIDE_CONSTANT_TYPE:
                        //匀速滑动
                        SlideConstantType(bottomTransform);
                        break;
                    case SlideType.SLIDE_THIRD_CUT_SPEED_TYPE:
                        //第三减速带
                        SlideThirdCutSpeedType(bottomTransform);
                        break;
                    case SlideType.SLIDE_SNOW_TYPE:
                        //处于冰冻泡泡的功效之下
                        SlideSnowType(bottomTransform);
                        break;
                    case SlideType.SLIDE_CRUSH_TYPE:
                        //冲击波效果
                        SlideCrushType();
                        break;
                    case SlideType.SLIDE_CHALLANGE_TYPE:
                        //挑战模式，生成新的两行泡泡下滑
                        //滑板不断下滑，挡板不断上升
                        ExFunctionChanllenge();
                        break;
                    case SlideType.SLIDE_GOON_DISAPPEAR_TYPE:
                        SlideGoonDisappearType();
                        break;
                    //case SlideType.SLIDE_HANDLE_STOP_TYPE:
                        //购买步数页面弹出之后，不再滑动,此处不做操作
                        //break;
                    case SlideType.SLIDE_BUY_STEP_END_TYPE:
                        //购买步数之后滑动
                        SlideBuyStepType();
                        break;
                    case SlideType.SLIDE_PAUSE_STOP_TYPE:
                        break;
                    default:
                        break;
                }
            }
            //负责控制警戒声音
            ManageWarningSound();
            yield return null;
        }
        yield return null;
    }


    #region 初始泡泡上移规则
    Vector3 targetPos;
    float speedMove = 1.2f;
    void PrepareTargetPos()
    {
        //目标位置就是 至limitWorldDis 处
        targetPos = new Vector3(transform.position.x, limitWorldDis, transform.position.z);
    }
    //使用lerp方法
    void SlideStartMoveType()
    {
        //使用Lerp方法
        transform.position = Vector3.Lerp(transform.position, targetPos, speedMove * Time.deltaTime);
        //如果快要接近目标位置的时候，修改游戏状态，使其可以发射泡泡了
        //当然后面可能会添加开始动画，那个时候需要再动画执行之后才修改 state
        if (GameManager.Instance.CurrentStateIsPrepare() && Mathf.Abs(targetPos.y - transform.position.y) < 0.3f)
        {
            GameManager.Instance.SetStateToPlaying();
        }
        //如果滑动小于 这个数值的时候，直接不再循环,修改状态到匀速滑动
        if (Mathf.Abs(targetPos.y - transform.position.y) < 0.02)
        {
            pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
        }
    }


    #endregion 初始泡泡上移规则



    #region 负责管理最底端标记泡泡的赋值
    /*
     * @brief       判断最底端标记泡泡是否有值
     * @parma       bool 有效的时候返回true，无效的时候返回false
     */
    bool GetBottomObjIsExit()
    {
        if (slideMostBottomObject != null)
        {
            //如果不为空，则检测其脚本上的标记变量：是否毁灭，是否掉落
            PubbleObject script = slideMostBottomObject.GetComponent<PubbleObject>();
            if ((script.isBurst && PlayLogic.Instance.markNeedBurstAccount >= PlayLogic.limitBurstCount) || script.isDropIng)
                slideMostBottomObject = null;
        }
        if (slideMostBottomObject == null)
            return false;
        else
            return true;
    }

    /*
     * @brief       记录哪个是最底下的泡泡
     */
    public void CheckMostBottomTransform()
    {
        if (slideMostBottomObject != null)
        { 
            //如果不为空，则检测其脚本上的标记变量：是否掉落，是否有关联
            PubbleObject script = slideMostBottomObject.GetComponent<PubbleObject>();
            //此处添加 account > limitAccount 是因为，当发射之后会立刻标记为 burst = true ，只判断burst 有误差
            if ((script.isBurst && PlayLogic.Instance.markNeedBurstAccount >= PlayLogic.limitBurstCount) || script.isDropIng)
                slideMostBottomObject = null;
        }
        PubbleObject[] allScripts = PlayLogic.Instance.storageAllObjectScripts;
        for (int i = 0; i < allScripts.Length; i++)
        {
            if (allScripts[i] != null)
            {
                PubbleObject pubbleScript = allScripts[i];
                CheckWhoIsMostObject(pubbleScript.gameObject);
            }
        }
    }

    /*
     * @brief       判断哪个在底下
     */
    public void CheckWhoIsMostObject(GameObject gmob)
    {
        PubbleObject pubbleScript = gmob.GetComponent<PubbleObject>();

        if (pubbleScript != null && !pubbleScript.isBurst && !pubbleScript.isDropIng && (int)pubbleScript.pubbleType < (int)PubbleColorType.PUBBLE_RAINBOW_TYPE)
        {
            //如果这个泡泡，不为null 而且 未被标记为毁灭   而且 有关联
            GameObject newBottomObj = pubbleScript.gameObject;
            //这个脚本不为null，而且未被标记摧毁，未被标记 失联
            if (slideMostBottomObject == null)
            {
                slideMostBottomObject = newBottomObj;
            }
            else if (slideMostBottomObject.transform.localPosition.y > newBottomObj.transform.localPosition.y)
            {
                //如果 标记位置高于 newObj ,
                slideMostBottomObject = newBottomObj;
            }
        }
    }

    /*
     * @brief       给底部标记泡泡赋值
     * @desc        在创建所有泡泡的时候，进行赋值
     */
    public void AssignBottomPubble(GameObject obj)
    {
        if (slideMostBottomObject == null)
            slideMostBottomObject = obj;
    }

    /*
     * @brief       获取最底端的标记泡泡的transform
     * @desc        当获取当前预备有效泡泡数组的时候用到
     */
    public Transform GetBottomMarkTransform()
    {
        if (slideMostBottomObject != null)
            return slideMostBottomObject.transform;
        return null; ;
    }
    #endregion #region 负责管理最底端标记泡泡的赋值

    #region 负责快速滑动跟匀速滑动的处理
    /*
     * @brief       处理快速滑动
     * @param       tfm     最底端标记泡泡的transform
     */
    void SlideQuickType(Transform tfm)
    {
        //快速滑动，当最底端的 标记量 位置高于limitWorldDis 的时候，快速滑落至stopLimitWordDis
        Vector3 myPos = transform.position;
        Vector3 newPos = new Vector3(myPos.x, myPos.y - quickSpeed * Time.deltaTime, myPos.z);
        transform.position = newPos;
        //检测是不是到了stopLimitWordDis处,到了则修改当前状态为匀速
        if (tfm.position.y < stopLimitWordDis)
            pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
    }

    /*
     * @brief       处理匀速滑动
     * @param       tfm     最底端标记泡泡的transform
     */
    void SlideConstantType(Transform tfm)
    {
        //匀速运动
        Vector3 myPos = transform.position;
        Vector3 newPos = new Vector3(myPos.x, myPos.y - constSpeed * Time.deltaTime, myPos.z);
        transform.position = newPos;
        //判断当前是否到了  死亡界限
        if (tfm.position.y < thirdSpeedLimitY)
        {
            //到达第三减速带以下，则改为第三减速
            pubbleSlideType = SlideType.SLIDE_THIRD_CUT_SPEED_TYPE;
        }
        else if (tfm.position.y > limitWorldDis)
        {
            //判断是否到了高于 limitWordDis,高于则修改状态为quick。因为有时候会被穿刺泡泡 等后推几行
            pubbleSlideType = SlideType.SLIDE_QUICK_TYPE;
        }
    }

    void ManageWarningSound()
    {
        if (pubbleSlideType == SlideType.SLIDE_THIRD_CUT_SPEED_TYPE)
        {
            SoundManager.Instance.PlayWarningSound();
        }
        else
        {
            SoundManager.Instance.PauseWarningSound();
        }
    }
    #endregion 负责快速滑动跟匀速滑动的处理

    #region 第三减速带
    //第三减速带由一个gameobject确定,这个object父节点是章鱼，便于保证他的相对位置
    GameObject thirdCutSpeedObject = null;
    float thirdSpeedLimitY = 0;
    float thirdCutSpeed = 0.008f;
    /*
     * @brief       第三减速带滑动
     * @desc        检测游戏是否失败
     * @desc        检测是否回到匀速带，快速带等
     */
    void SlideThirdCutSpeedType(Transform tfm)
    {
        //匀速运动
        Vector3 myPos = transform.position;
        Vector3 newPos = new Vector3(myPos.x, myPos.y - thirdCutSpeed * Time.deltaTime, myPos.z);
        transform.position = newPos;
        //判断当前是否到了  死亡界限
        if (tfm.position.y < overLimitY)
        {
            SlideGameOver();
        }
        else if (tfm.position.y > thirdSpeedLimitY)
        {
            //判断是否到了高于 第三减速带界限，高于则修改状态为匀速。因为有时候会被穿刺泡泡 等后推几行
            pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
        }
    }
    #endregion 第三减速带

    #region 负责冰冻泡泡效果的显示以及毁灭
    //记录冰冻时间
    float countSnowTime = 0f;
    /*
     * @brief       执行冰冻效果的滑动
     * @parma       tfm     底端标记泡泡的transform
     */
    void SlideSnowType(Transform tfm)
    {
        countSnowTime += Time.deltaTime;
        if (countSnowTime > limitSnowTime)
        {
            //如果冰冻 时间到了：销毁冰冻效果 同时检测当前最底端的泡泡位置，以修改 滑动状态
            DestroySnowEffect();
            if (tfm.position.y > limitWorldDis)
                //如果最底下高于limitWorldDis，则quick 运动
                pubbleSlideType = SlideType.SLIDE_QUICK_TYPE;
            else
                //如果不高于limitWorldDis ,则匀速运动
                pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
        }
    }

    /*
     * @brief       播放冰冻效果
     */
    public void PlaySnowEffect()
    { 
        //修改状态为snow，播放动画
        pubbleSlideType = SlideType.SLIDE_SNOW_TYPE;
        //冰冻时间置0
        countSnowTime = 0.0f;
        //显示冰冻效果
        if (markShowSnowObject != null)
        {
            DestroySnowEffect();
        }
        markShowSnowObject = Instantiate(snowEffectGameObject) as GameObject;
        markShowSnowObject.transform.parent = uiRootGameObject.transform;
        markShowSnowObject.transform.localScale = new Vector3(1, 1, 1);
        SetDropButtonDisable();
    }

    /*
     * @brief       销毁冰冻效果
     */
    void DestroySnowEffect()
    { 
        //销毁冰冻效果
        Destroy(markShowSnowObject);
        markShowSnowObject = null;
        SetDropButtonActive();
    }
    #endregion 负责冰冻泡泡效果的显示以及毁灭


    #region 负责创建并滑动新生成的泡泡：挑战模式

    //负责控制新生成的泡泡
    bool hasCreateOneRow = false;
    //记录下落的量，到了0.1(一行)泡泡的高度，则创建新的一行
    float slideDis = 0.0f;
    //界限判断
    float limitMoveDis = 0;
    //极限模式下的出泡泡滑动速度
    public float limitStyleSpeed = 120f;
    /*
     * @brief       创建并滑动泡泡
     * @desc        设置当前game状态，为不可发射状态
     * @desc        挡板的碰撞器禁用
     * @desc        修改为 协程，因为当 消除了，后退半格的时候 有误差，因此等待这半格后退完成
     */
    public void StartCreateNewPubble(Dictionary<int, PubbleRowInfo> pubbles)
    {
        //计算一行需要下落的距离
        limitMoveDis = 0.115f * Mathf.Sqrt(3) / 2;
        slideDis = 0f;
        newPubbleInfo = null;
        newPubbleInfo = pubbles;
        hasCreateOneRow = false;
        pubbleSlideType = SlideType.SLIDE_CHALLANGE_TYPE;
        PlayLogic.Instance.SetTopBoxCollideState(false);
    }

    /*
     * @brief       改为整体泡泡创建一行后保持不动，挡板上移一行距离
     * @desc        
     */
    void ExFunctionChanllenge()
    {
        //如果不是玩的状态
        if (!GameManager.Instance.CurrentStateIsPlayingState()) return;
        //创建一行泡泡:quickSpeed
        if (!hasCreateOneRow)
        {
            PlayLogic.Instance.CreatePubbleOneRowLimitStyle(newPubbleInfo[0]);
            PlayLogic.Instance.GetPreParePosition();
            hasCreateOneRow = true;
        }

        //挡板上移
        float upMoveDis = limitStyleSpeed * Time.deltaTime;
        if (!PlayLogic.Instance.SetTopLimitPosition(upMoveDis))
        { 
            //如果不需要继续移动了，则
            pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
            PlayLogic.Instance.SetTopBoxCollideState(true);
        }
        /*
        //挡板保持原世界位置，整体下落
        Vector3 originPosition = PlayLogic.Instance.GetTopPanelWorldPosition();
        Vector3 myPos = transform.position;
        Vector3 newPos = new Vector3(myPos.x, myPos.y - limitStyleSpeed * Time.deltaTime, myPos.z);
        transform.position = newPos;
        PlayLogic.Instance.ResetTopPabelPosition(originPosition);
        slideDis += quickSpeed * Time.deltaTime;
        if (slideDis > limitMoveDis)
        {
            pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
            PlayLogic.Instance.SetTopBoxCollideState(true);
        }
         * */
    }
    #endregion 负责创建并滑动新生成的泡泡：挑战模式


    #region 冲击波泡泡的效果
    //冲击波特效object
    GameObject crushPanelObj = null;
    //冲击波挡板应该停止的位置
    float crushPanelStopPosY;
    //整个滑板应该达到的新位置
    float panelTransformNewPosY;
    //冲击波泡泡的速度
    float crushSpeed = 1.0f;
    public void ExeFunctionCrushEffect(GameObject panelObj)
    {
        crushPanelObj = panelObj;
        pubbleSlideType = SlideType.SLIDE_CRUSH_TYPE;
        //冲击五行
        panelTransformNewPosY = transform.position.y + 5 * pubbleWorldMarginHeight;
        //1.5 是因为 冲击波光效的 大小为 222 是 泡泡的三倍
        crushPanelStopPosY = slideMostBottomObject.transform.position.y - 1.5f * pubbleWorldMarginHeight;
        SetDropButtonDisable();
    }


    void SlideCrushType()
    {
        if (crushPanelObj.transform.position.y < crushPanelStopPosY)
        {
            Vector3 myPos = crushPanelObj.transform.position;
            Vector3 newPos = new Vector3(0, myPos.y + crushSpeed * Time.deltaTime, myPos.z);
            crushPanelObj.transform.position = newPos;
        }
        else
        {
            //如果达到了停步位置，则整体上滑 五步
            if (transform.position.y < panelTransformNewPosY)
            {
                Vector3 transPos = transform.position;
                Vector3 newTransPos = new Vector3(transPos.x, transPos.y + crushSpeed * Time.deltaTime, transPos.z);
                transform.position = newTransPos;
            }
            else
            {
                //滑动完了：销毁特效，改当前为 匀速模式
                Destroy(crushPanelObj);
                pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
                //
                SetDropButtonActive();
            }
        }
    }
    #endregion 冲击波泡泡的效果


    #region 连消后退效果
    //后退半格的速度
    float goonDisappearSpeed = 0.2f;
    //后退的之后的位置
    float moveAfterPosY;

    /*
     * @brief       连消退后半格
     * @desc        每次连消都退后半格
     * @desc        冰冻效果不考虑在内
     */
    public void ExeFunctionGoOnEffect()
    {
        if (pubbleSlideType != SlideType.SLIDE_SNOW_TYPE && pubbleSlideType != SlideType.SLIDE_HANDLE_STOP_TYPE && pubbleSlideType != SlideType.SLIDE_CHALLANGE_TYPE)
        { 
            //非冰雪情况下, 非 购买步数情况下 后退,如果当前是生成 新泡泡，则不后退
            //修改为 后退 状态
            pubbleSlideType = SlideType.SLIDE_GOON_DISAPPEAR_TYPE;
            //后退之后的位置
            moveAfterPosY = transform.position.y + pubbleWorldMarginHeight / 2f;
        }
    }

    void SlideGoonDisappearType()
    {
        if (transform.position.y < moveAfterPosY)
        {
            Vector3 transPos = transform.position;
            Vector3 newTransPos = new Vector3(transPos.x, transPos.y + goonDisappearSpeed * Time.deltaTime, transPos.z);
            transform.position = newTransPos;
        }
        else
        { 
            //如果达到了之后，则：修改状态
            pubbleSlideType = SlideType.SLIDE_CONSTANT_TYPE;
        }
    }
    #endregion 连消后退效果

    #region 负责挑战模式下，暂停与购买相关
    /*
     * @brief       修改为这个状态后，页面不在滑动,处于停止状态
     */
    //记录当前的状态
    SlideType orignCurrentType;
    public void ChangeSlideStypeToStop()
    {
        orignCurrentType = pubbleSlideType;
        pubbleSlideType = SlideType.SLIDE_HANDLE_STOP_TYPE;
    }

    /*
     * @brief       购买之后滑动
     * @desc        滑动到初始位置
     */
    float stepSlideSpeed = 0.4f;
    float stepMoveDisY = 0;

    public void StarEndBuyStepMove()
    {
        //目前统一定好 退 4格 ,看效果
        stepMoveDisY = pubbleWorldMarginHeight * 4;
        pubbleSlideType = SlideType.SLIDE_BUY_STEP_END_TYPE;
        //currentType
    }

    float countStepMoveY = 0;
    public void SlideBuyStepType()
    {
        if (countStepMoveY < stepMoveDisY)
        {
            Vector3 transPos = transform.position;
            countStepMoveY += stepSlideSpeed * Time.deltaTime;
            Vector3 newTransPos = new Vector3(transPos.x, transPos.y + stepSlideSpeed * Time.deltaTime, transPos.z);
            transform.position = newTransPos;
        }
        else
        {
            //如果达到了之后，则：修改状态 为原先的状态
            pubbleSlideType = orignCurrentType;
        }
    }



    #endregion 负责挑战模式下，暂停与购买相关

    #region 负责游戏失败
    public void SlideGameOver()
    {
        pubbleSlideType = SlideType.SLIDE_STOP_TYPE;
        //
        StartCoroutine(GameManager.Instance.GameOverFunction());
    }
    #endregion 负责游戏失败

    #region 为暂停做的操作
    //暂停只是停止挡板滑动以及不能发射泡泡
    //记录上次的滑动类型
    SlideType lastSlideType;
    public void PauseSlide()
    {
        lastSlideType = pubbleSlideType;
        //暂停滑动
        pubbleSlideType = SlideType.SLIDE_PAUSE_STOP_TYPE;
    }
    //当前存在问题的可能是 冰冻效果
    public void RestoreSlideType()
    {
        pubbleSlideType = lastSlideType;
    }

    public bool CurrentStateIsPause()
    {
        return pubbleSlideType == SlideType.SLIDE_PAUSE_STOP_TYPE;
    }

    #endregion 为暂停做的操作

    #region 负责切换冰冻跟冲击波
    //当处于任何两种效果下，这俩均 不可点击
    public GameObject snowButton = null;
    public GameObject crushButton = null;

    void SetDropButtonDisable()
    {
        if (snowButton != null)
        {
            UIButton buttonScript = snowButton.GetComponent<UIButton>();
            buttonScript.SetState(UIButton.State.Disabled, true);
            buttonScript.isEnabled = false;
        }

        if (crushButton != null)
        {
            UIButton buttonScript = crushButton.GetComponent<UIButton>();
            buttonScript.SetState(UIButton.State.Disabled, true);
            buttonScript.isEnabled = false;
        }
    }

    void SetDropButtonActive()
    {
        if (snowButton != null)
        {
            UIButton buttonScript = snowButton.GetComponent<UIButton>();
            buttonScript.SetState(UIButton.State.Normal, true);
            buttonScript.isEnabled = true;
        }

        if (crushButton != null)
        {
            UIButton buttonScript = crushButton.GetComponent<UIButton>();
            buttonScript.SetState(UIButton.State.Normal, true);
            buttonScript.isEnabled = true;
        }
    }

    #endregion 负责切换冰冻跟冲击波

    

    #region 对外接口
    /*
     * @brief       判断当前是否可以发射道具泡泡
     * @desc        
     */
    public bool IsCanShootDrop()
    {
        return pubbleSlideType == SlideType.SLIDE_QUICK_TYPE || pubbleSlideType == SlideType.SLIDE_CONSTANT_TYPE;
    }

    #endregion 对外接口
}
