using UnityEngine;
using System.Collections;

/*
 * @brief       负责创建第一个泡泡，以及第二个发射泡泡等
 * @desc        在界面上创建两个标记点（第一个泡泡坐标点，第二个泡泡坐标点）
 * @desc        第二个泡泡用当前界面上存在的泡泡种类去创建，第一个泡泡用第二个泡泡创建，然后再创建一次第二个泡泡
 * @desc        界面上有个StikeObject即发射器，他会在第一个发射点位置 
 */

public class StrikeManager : MonoBehaviour
{

    #region Properties
    private static StrikeManager m_Instance = null;
    public static StrikeManager Instance { get { return m_Instance; } private set { m_Instance = value; } }


    /*
     * strikeEmitter：发射器StikeObject 内部有markShootPubbleObject 保存发射的泡泡
     * strikeScript :从发射器内获取脚本组建，便于读取脚本内的状态,执行发射器内的函数
     */
    GameObject strikeEmitter;
    StrikeObject strikeScript;


    /*
     * 前三个是定位坐标的gameobject，后三个是初始化出来的泡泡
     * firstPositionObject:第一个发射点（即正在发射）的gameobject,通过FirstPositionObject获得
     * secondPositionObject:第二个预备泡泡的坐标gameboject，通过SecondPositionObject获得
     * firstShootPubble:第一个预备发射泡泡
     * secondShootPubble：第二个预备发射泡泡
     */
    GameObject firstPositionObject;
    GameObject secondPositionObject;

    GameObject firstShootPubble;
    GameObject secondShootPubble;

    #endregion Properties

    #region Member Function

    void Awake()
    {
        m_Instance = this;
        //获取发射器以及其脚本
        strikeEmitter = GameObject.Find("StrikeObject");
        strikeScript = strikeEmitter.GetComponent<StrikeObject>();
        //获取两个泡泡位置的gameobject
        firstPositionObject = GameObject.Find("FirstPositionObject");
        secondPositionObject = GameObject.Find("SecondPositionObject");
    }

    void Start()
    {
        //初始化第二个泡泡，再初始化第一个泡泡，因为第一个泡泡需用用第二个泡泡来初始化
        StartCoroutine(InitSecondShootPubble()); 
        //延时是 给第二个泡泡的缩放预留时间
        Invoke("ExeFunctionInitStrikeBySecond", 0.3f);
	}

    //只是做封装而已
    void ExeFunctionInitStrikeBySecond()
    {
        StartCoroutine(InitStrikeEmitterBySecondPubble());
    }

    /*
     * @brief       用第二个发射泡泡初始化发射器
     * @desc        下面的return 是为移动预留时间
     */
    public IEnumerator InitStrikeEmitterBySecondPubble()
    {
        //将第二个泡泡移动到第一个位置
        iTween.MoveTo(secondShootPubble.gameObject, firstPositionObject.transform.position, 0.1f);
        yield return new WaitForSeconds(0.1f);
        secondShootPubble.gameObject.transform.position = firstPositionObject.transform.position;

        //替换上面的内容
        AssignObjectToFirstShootObject(secondShootPubble);
        //初始化第二个发射泡泡
        StartCoroutine(InitSecondShootPubble());
        //标记完成了初始化,此时第二个泡泡初始化完成了，可以进行交换了
        isInitFinish = true;
    }

    /*
     * @brief       初始化第二个泡泡
     * @desc        第二个泡泡的种类由当前存在的泡泡决定
     * @desc        延时0.5s 为创建泡泡做准备
     */
    IEnumerator InitSecondShootPubble()
    { 
        //获取预备出现的泡泡的数组
        ArrayList remainObjects = PlayLogic.Instance.GetRemainObjectPerfabs();
        if (remainObjects != null && remainObjects.Count == 0)
        {
            //此处等待0.2s 是因为，游戏一开始，场景里面的泡泡没有创建完成，存在一个时差问题，创建之后这个问题就不存在了，也不再执行此处
            yield return new WaitForSeconds(0.2f);
            remainObjects = PlayLogic.Instance.GetRemainObjectPerfabs();
        }
        GameObject tmpSecondObject = null;
        if (remainObjects != null && remainObjects.Count > 0)
        {
            //随机索引
            int randomIndex = Random.Range(0, remainObjects.Count);
            //初始化第二个发射泡泡
            tmpSecondObject = Instantiate(remainObjects[randomIndex] as GameObject) as GameObject;
        }
        else
        { 
            //如果数组为空，则从 所有泡泡预设数组中随机出
            int randomIndex = Random.Range(0, PlayLogic.normalPubbleNumber);
            //初始化数组
            tmpSecondObject = (GameObject)Instantiate(PlayLogic.Instance.pubbleKindPrefabs[randomIndex]);
        }
        //判断当前是否存在新手指引，存在则根据新手指引的内容来
        GameObject tuitionManager = GameObject.Find(ConstantValue.TuitionManagerName);
        if (tuitionManager != null)
        {
            PubbleColorType getType = TuitionManager.Instance.GetCurrentColorType();
            if (getType != PubbleColorType.PUBBLE_EMPTY_TYPE)
            {
                DestroyImmediate(tmpSecondObject);
                tmpSecondObject = (GameObject)Instantiate(PlayLogic.Instance.pubbleKindPrefabs[(int)getType - 1]);
            }
        }
        AssignObjectToSecondShootObject(tmpSecondObject);
    }

    /*
     * @brief       当点击屏幕的时候，进行发射泡泡
     * @param       touchPos        点击的坐标
     */
    internal void ShootPubble(Vector3 touchPos)
    {
        //如果发射器上 有泡泡在移动，则不准发射,同时第二个泡泡也初始化完成了才可以发射,没有交换完 不准发射
        if (strikeScript.isShooting || !isInitFinish || !isFinishExChange)
            return;
        //获取发射方向:将发射点的坐标转化为屏幕坐标
        Vector3 firstPosition = firstPositionObject.transform.position;
        Vector3 _ssfirstPos = Camera.main.WorldToScreenPoint(firstPosition);
        Vector3 dir = (touchPos - _ssfirstPos).normalized;
        //Vector3 dir = touchPos;
        //发射器执行射击
        strikeScript.shootPubble(dir);
        //发射了之后，设置当前已经没有  道具泡泡，可以交换了
        markCanExChange = true;
        //发射之后，不准许交换位置，直到 第二个泡泡也初始化完成之后
        isInitFinish = false;
    }


    public float speedMove = 0.4f;
    //从第二个位置到达第一个位置
    IEnumerator LerpShootPubbleToStrike()
    {
        bool isContinue = true;
        while (isContinue)
        {
            Vector3 firstPosition = firstPositionObject.gameObject.transform.position;
            Vector3 secondPosition = secondShootPubble.gameObject.transform.position;
            secondShootPubble.gameObject.transform.position = Vector3.Lerp(secondPosition, firstPosition, speedMove * Time.deltaTime);
            secondPosition = secondShootPubble.gameObject.transform.position;
            if (Mathf.Abs(firstPosition.x - secondPosition.x) < 0.01 || Mathf.Abs(firstPosition.y - secondPosition.y) < 0.01f)
            {
                //手动赋值
                secondShootPubble.gameObject.transform.position = firstPositionObject.gameObject.transform.position;
                isContinue = false;
            }
            yield return null;
        }
        yield return null;
    }

    //是否完成了交换：如果点击交换按钮太频繁，容易引起混乱
    bool isFinishExChange = true;
    //是否初始化完成，没有初始化完成，不准许交换，不允许发射
    //当前问题:发射第一个之后，迅速点击交换，容易出问题
    //在发射的时候设置为false ，发射完成之后完成初始化 完第二个，设置为true
    bool isInitFinish = false;

    /*
     * @brief       交换第一个第二个泡泡
     * @desc        交换按钮的点击事件
     * @desc        1.第一个泡泡发射完，而且第二个泡泡 已经初始化给第一个泡泡，第二个泡泡也已经创建出来
     */
    public void ExChangeTwoPubbles()
    {
        if (markCanExChange)
        {
            if (isFinishExChange && isInitFinish)
            {
                if (isBuyExChangeProp())
                {
                    StartCoroutine(TransformPreparePubble());
                }
            }
        }
        else
        {
            //弹出一个提示框：道具泡泡不可交换
        }
    }

    /*
     * @brief       两个预备泡泡交换
     * @desc        两个泡泡分辨移动到对方位置
     * @desc        两个泡泡互相交换他们的值
     */
    private IEnumerator TransformPreparePubble()
    {
        isFinishExChange = false;
        iTween.MoveTo(firstShootPubble.gameObject, secondPositionObject.transform.position, 0.1f);
        iTween.MoveTo(secondShootPubble.gameObject, firstPositionObject.transform.position, 0.1f);

        yield return new WaitForSeconds(0.1f);

        GameObject tmpFirstObject = firstShootPubble;
        AssignObjectToFirstShootObject(secondShootPubble);
        AssignObjectToSecondShootObject(tmpFirstObject,false);
        isFinishExChange = true;
    }
    #endregion Member Function 



    //这个参数确定当前泡泡能否交换
    //规则：使用了道具泡泡之后，不准许跟第二个预备泡泡交换，但是可以跟其他道具泡泡交换
    bool markCanExChange = true;
    /*
     * @brief       点击了下面的道具泡泡的时候，将发射泡泡修改成道具泡泡
     * @prama       colorType       传入点击的道具泡泡类型
     * @desc        成为道具泡泡之后，不可再替换，这个随策划吧 
     */
    public void ExecuteEmissionPropPubble(PubbleColorType colorType)
    {
        //判断当前是否在发射之中，发射之中不准许 添加道具泡泡
        if (GetShootState()) return;
        //同时判断当前 两个预备泡泡 有没有初始化完
        if (!isInitFinish) return;
        //初始化道具泡泡，同时赋值给第一个预备泡泡
        GameObject propObject = null;
        if ((int)colorType > (int)PubbleColorType.PUBBLE_RAINBOW_TYPE && (int)colorType <= (int)PubbleColorType.PUBBLE_CRUSH_TYPE)
        {
            propObject = GameObject.Instantiate(PlayLogic.Instance.pubbleKindPrefabs[(int)colorType - 1]) as GameObject;
        }
        if (propObject != null)
        { 
            //如果存在这个道具泡泡
            //标记为不可交换
            markCanExChange = false;
            //销毁当前这个泡泡
            DestroyObject(firstShootPubble);
            AssignObjectToFirstShootObject(propObject,false);
        }
    }

    /*
     * @brief       给第一个发射泡泡赋值
     * @prama       tmpObject   要发射的泡泡
     * @prama       restoreDepth还原深度
     * @desc        分为两种情况：发射普通泡泡需要还原深度值，发射道具泡泡不需要还原
     */
    private void AssignObjectToFirstShootObject(GameObject tmpObject,bool restoreDepth = true)
    {
        //设置发射器的位置,到初始化坐标上
        strikeEmitter.transform.position = firstPositionObject.transform.position;
        //赋值
        firstShootPubble = tmpObject;
        //设置第一个发射泡泡的父节点 到发射器上
        firstShootPubble.transform.parent = strikeEmitter.transform;
        //设置相对坐标
        firstShootPubble.transform.localPosition = new Vector3(0, 0, 0);
        //设置相对缩放比例
        firstShootPubble.transform.localScale = new Vector3(1,1,1);
        //设置tag值，普通泡泡赋值已经设置过了，但是可能会是道具泡泡，再次赋值
        firstShootPubble.tag = ConstantValue.OthersTag;
        firstShootPubble.GetComponent<SphereCollider>().enabled = false;
        if (restoreDepth)
        {
            //普通泡泡，需要修改depth，还原depth
            firstShootPubble.GetComponent<ColorPubbleObject>().SetOrginPubbleDepth();
        }
        else
        {
            //道具泡泡，执行缩放动画
            iTween.PunchScale(firstShootPubble, new Vector3(0.2f, 0.2f, 0.2f), 1f);
        }
        //上面已经将 数据信息设置完，现在赋值给 stikeobject内的markshootobject，便于一起移动
        strikeScript.markShootPubleObject = firstShootPubble;        
    }

    /*
     * @brief       给第二个泡泡赋值
     * @prama       tmpObject   第二个泡泡即将的值
     * @prama       initialize  是否是初始化出来的
     * @desc        分为两种情况:true 初始化出来的泡泡执行缩放动画，false 交换的泡泡，不执行动画
     */
    private void AssignObjectToSecondShootObject(GameObject tmpObject, bool initialize = true)
    {
        secondShootPubble = tmpObject;
        secondShootPubble.transform.parent = secondPositionObject.transform;
        secondShootPubble.transform.localPosition = secondPositionObject.transform.position;
        secondShootPubble.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
        //设置预备泡泡的tag，防止在检测的时候被统计到
        secondShootPubble.tag = ConstantValue.OthersTag;
        secondShootPubble.GetComponent<SphereCollider>().enabled = false;
        //修改预备泡泡的depth便于显示
        secondShootPubble.GetComponent<ColorPubbleObject>().SetShowPubbleDepth();
        if (initialize)
        {
            //这是预备泡泡，出现的时候执行缩放动画
            iTween.PunchScale(secondShootPubble, new Vector3(0.2f, 0.2f, 0.2f), 1f);
        }
    }

    #region 添加转换控制:每场开局3个免费，之后提示购买
    const int limitExChangeTimes = 3;
    int countExChangeTimes = 0;
    public GameObject buyTipPanel;
    bool isBuyExChangeProp()
    {
        //return true;
        //先从本地数据里面读取，查看是否已经购买过永久使用
        if (UserInstanse.GetInstance().hasBuySteps == 1)
        {
            return true; 
        }

        if (limitExChangeTimes > countExChangeTimes)
        {
            countExChangeTimes++;
            return true;
        }
        else
        {

            SlidePlayPanel.Instance.PauseSlide();

            //弹出购买框
            GameObject cur = Instantiate(buyTipPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

            BuyTipPanel panel = cur.GetComponent<BuyTipPanel>();
            panel.InitTipType((BuyTipPanel.PropType)8);
            panel.buyTip_sureDelegate = makeSureDelegate;
            panel.buyTip_deleteDelegate = CloseDelegate;
        }
        return false;
    }

    void CloseDelegate()
    {
        SlidePlayPanel.Instance.RestoreSlideType();
    }

    public GameObject goodsTip;
    void makeSureDelegate()
    {
        SlidePlayPanel.Instance.RestoreSlideType();
        UserInstanse.GetInstance().hasBuySteps = 1;

        GameObject tip = Instantiate(goodsTip) as GameObject;
        GameObject root = GameObject.Find("UI Root");
        tip.gameObject.transform.parent = root.gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);
        tip.GetComponent<GoodsTipManager>().setTipTitle("成功开启");
    }
    #endregion 添加转换控制

    #region 章鱼转动的时候，将发射器位置定位到新坐标

    public bool GetShootState()
    {
        return strikeScript.isShooting;
    }

    public void ResetStrikePosition()
    {
        strikeEmitter.transform.position = firstPositionObject.transform.position;
    }
    #endregion 章鱼转动的时候，将发射器位置定位到新坐标
}
