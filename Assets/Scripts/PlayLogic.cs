using UnityEngine;
using System.Collections;

/*
 * @brief       负责页面上泡泡的显示等逻辑
 * @Author      Wolf
 * @date        2014-9-16
 * @desc        绑定在PlayLogic上，负责创建所有的泡泡逻辑
 */

public class PlayLogic : MonoBehaviour
{

    #region Properties
    //创建一个单例便于调用
    private static PlayLogic m_Instance = null;
    public static PlayLogic Instance { get { return m_Instance; } }


    //所有泡泡类型的预设数组:在Editor 内编辑赋值
    public GameObject[] pubbleKindPrefabs;
    //保存所有泡泡的特效
    public GameObject[] pubbleEffectPerfabs;



    //NGUI的坐标 为像素坐标系中心点平面坐标:(0,0)
    //当前设计分辨率为（768*1280），左右均有边界
    //一共9个泡泡，每个泡泡尺寸74 ，中间间隔为0 ，碰撞半径 36
    //写的这么乱是因为当前我的水平有限，先把流程走通，实现效果，后面能改则改

    //泡泡的尺寸
    public float pubbleWidth = 74.0f;
    //最左边坐标 - 384 ,左右边界均为51 ，因此从-333 开始
    public float startPointX = -333f;
    //左右边界，用于检测发射泡泡是不是到了左右边界，便于调试发射泡泡的位置
    public float leftLimitPosX;
    public float rightLimitPosX;
    //泡泡之间间隔 左右上下均为 2.0f
    public float marginEachPubble = 0.0f;
    //记录泡泡的X坐标
    float markCurrentPointX;
    //起始泡泡的Y坐标:PlayLogic为EmptyObject所以为一个点,所以第一行泡泡显示的时候，能看见一半
    float startPointY = 0f;//
    //记录当添加的泡泡的Y坐标
    float markCurrentPointY ;
    //每次Y坐标 移距离在 start 内赋值
    public float markMoveUnit;
    //一行9个泡泡:每行可能的坐标一共17个
    public static int rowPubbleNum = 9;

    //记录当前总共创建了几行,用于计算该行泡泡的起始坐标
    int markCreateRowIndex;

    


    //当发射泡泡遇到其他泡泡的时候，开始记录要毁坏的泡泡 数,到达要求就执行毁灭
    public int markNeedBurstAccount;
    public const int limitBurstCount = 3;
    //记录发射泡泡的类型:用于判断是否与发射泡泡颜色一致的泡泡
    public PubbleColorType markShootType = PubbleColorType.PUBBLE_EMPTY_TYPE;
    //记录被撞击泡泡的类型:特殊时候用于 变色跟闪电
    public PubbleColorType markCollidedPubbleType = PubbleColorType.PUBBLE_EMPTY_TYPE;
    //记录当前页面所有泡泡上绑定的脚本,便于获取脚本内的状态信息
    public PubbleObject[] storageAllObjectScripts;
    //记录顶部泡泡脚本,每当创建一行的时候，记录一次，便于后面从顶部泡泡开始遍历有没有关联泡泡
    public PubbleObject[] markTopRowPubble;
    //记录顶部9个泡泡的位置,便于发射泡泡撞击到了顶部的时候设置位置
    public Vector3[] markTopPubblePostion;




    
    //顶部界限的父节点，此时获取引用是便于创建泡泡的时候，确定位置
    GameObject topLimitParentObject = null;
    //底部界限的 世界坐标
    public float bottomLimitY = 0;
    //普通泡泡种类 一个7 个
    public const int normalPubbleNumber = 7;
    //记录一共有多少行泡泡，这个泡泡是创建的泡泡，不包括发射上来的
    public int countPubbleRowsInPanel = 0;

    #endregion Properties

    #region Unity OverLoad
    void Awake()
    {
        m_Instance = this;
        topLimitParentObject = GameObject.Find(ConstantValue.TopLimitParenObjectName);
    }

	void Start () {
        //计算底部界限的Y坐标:第一个发射泡泡位置 + 一个泡泡尺寸  0.1 为一个泡泡世界坐标尺寸
        bottomLimitY = GameObject.Find(ConstantValue.FirstPositonObjName).transform.position.y + 0.1f;
        markCreateRowIndex = 0;
        //上移单位  记得 + margin
        markMoveUnit = Mathf.Sqrt(3) * pubbleWidth / 2 + marginEachPubble;
        markCurrentPointY = startPointY;
        leftLimitPosX = startPointX + pubbleWidth / 2.0f;
        rightLimitPosX = leftLimitPosX + (rowPubbleNum - 1) * (pubbleWidth + marginEachPubble);
        markTopRowPubble = new PubbleObject[rowPubbleNum];
        markTopPubblePostion = new Vector3[rowPubbleNum];
        countPubbleRowsInPanel = UserInstanse.GetInstance().PubbleAllRowInfo.Count;
        CreateShowPubble();
    }
    #endregion Unity Overload


    #region 负责创建关卡内的所有泡泡
    /*
     * @brief       创建泡泡
     */
    void CreateShowPubble()
    {
        //创建所有行的泡泡
        for (int i = 0; i < countPubbleRowsInPanel; i++)
        {
            CreatePubbleOneRowStartGame(UserInstanse.GetInstance().PubbleAllRowInfo[i]);
        }
        ResetPanelTransform();
        RandomAllPubbleDiamond();
        //滑板从下往上移动，展现全部泡泡，过程
        StartCoroutine(SlidePlayPanel.Instance.ManageSlideAction());
    }

    /*
     * @brief       重新确定panel的位置
     */
    void ResetPanelTransform()
    {
        SetTopPabelPosition();
        //先确定自身位置与目标位置
        Vector3 originPos = new Vector3(transform.localPosition.x, -transform.localPosition.y - topLimitParentObject.transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = originPos;
    }

    /*
     * @brief       重新确定挡板位置
     * @desc        创建所有泡泡的时候，或者新生成泡泡的时候
     */
    void SetTopPabelPosition()
    {
        //确定顶部的挡板的位置
        UITexture texture = topLimitParentObject.GetComponent<UITexture>();
        float topPosY = markCurrentPointY - markMoveUnit + pubbleWidth / 2 + marginEachPubble + texture.localSize.y / 2 + marginEachPubble;
        topLimitParentObject.gameObject.transform.localPosition = new Vector3(topLimitParentObject.gameObject.transform.localPosition.x, topPosY, topLimitParentObject.gameObject.transform.localPosition.z);
    }


    //生成一行泡泡，之后期望的挡板位置
    Vector3 preparePosition;
    public void GetPreParePosition()
    {
        UITexture texture = topLimitParentObject.GetComponent<UITexture>();
        float topPosY = markCurrentPointY - markMoveUnit + pubbleWidth / 2 + marginEachPubble + texture.localSize.y / 2 + marginEachPubble;
        Vector3 originTopPos = topLimitParentObject.gameObject.transform.localPosition;
        preparePosition = new Vector3(originTopPos.x,topPosY,originTopPos.z);
    }

    public bool SetTopLimitPosition(float updis)
    {
        Vector3 originTopPos = topLimitParentObject.gameObject.transform.localPosition;
        topLimitParentObject.gameObject.transform.localPosition = new Vector3(originTopPos.x, originTopPos.y + updis, originTopPos.z);
        if (preparePosition.y > topLimitParentObject.transform.localPosition.y)
        {
            //如果没有达到预定点，则返回true ，继续上移
            return true;
        }
        else
        { 
            //如果达到了预定位置，设置为预定坐标
            topLimitParentObject.gameObject.transform.localPosition = preparePosition;
            return false;
        }
    }

    public Vector3 GetTopPanelWorldPosition()
    {
        return topLimitParentObject.transform.position;
    }

    public void ResetTopPabelPosition(Vector3 position)
    {
        topLimitParentObject.transform.position = position;
    }

    public void SetTopBoxCollideState(bool state)
    {
        topLimitParentObject.GetComponentInChildren<BoxCollider>().enabled = state;
    }

    #endregion 负责创建关卡内的所有泡泡


    #region 负责泡泡创建:首次创建全部，后期极限模式，单行创建

    /*
     * @brief       游戏开始创建全部的泡泡
     * @desc        此时泡泡位置遵循最顶部 九个位置
     */
    void CreatePubbleOneRowStartGame(PubbleRowInfo rowInfo)
    {
        //获取当前行起始的X 坐标:因为分单行双行(各自起始点不一样)
        if (1 == (countPubbleRowsInPanel - markCreateRowIndex) % 2)
        {
            //表示单行：缩进半个width
            markCurrentPointX = startPointX + pubbleWidth / 2;
        }
        else
        {
            //表示双行：缩进一个width
            markCurrentPointX = startPointX + pubbleWidth;
        }
        CreateOneRowPubble(rowInfo);
    }

    //记录极限模式下创建的泡泡行数
    int countLimitCreateNum = 0;
    /*
     * @brief       极限模式的时候创建单行泡泡
     * @desc        此时位置 要以 当前界面上的泡泡位置去算，不能保证顶行是九个位置
     */
    public void CreatePubbleOneRowLimitStyle(PubbleRowInfo rowInfo)
    {
        //记录极限的行数
        countLimitCreateNum++;
        //记录总的行数
        countPubbleRowsInPanel++;
        if (1 == countLimitCreateNum % 2)
        {
            //表示在顶行泡泡之上的奇数 ，此时一行总共是8个泡泡,缩进一个泡泡位置
            markCurrentPointX = startPointX + pubbleWidth;
        }
        else
        {
            //表示双行：缩进半个width
            markCurrentPointX = startPointX + pubbleWidth/2;
        }
        CreateOneRowPubble(rowInfo);
    }

    /*
     * @brief       创建单行泡泡
     * @parma       rowInfo     单行泡泡信息
     * @desc        移动的时候考虑下移的多少
     */
    void CreateOneRowPubble(PubbleRowInfo rowInfo)
    {
        //单行9个泡泡
        for (int i = 0; i < rowPubbleNum; i++)
        {
            markTopRowPubble[i] = null;
            //获取当前泡泡的信息
            PubbleInfo pubbleInfo = rowInfo.PubbleRowDic[i];
            //如果不是空泡泡类型
            if (pubbleInfo.ColorType != PubbleColorType.PUBBLE_EMPTY_TYPE)
            {
                //创建当前泡泡的坐标
                Vector3 pubblePosition = new Vector3(markCurrentPointX, markCurrentPointY, 0);
                Vector3 pubbleScale = new Vector3(1, 1, 1);
                //创建泡泡
                GameObject pubbleObject = (GameObject)Instantiate(pubbleKindPrefabs[(int)pubbleInfo.ColorType - 1]);
                pubbleObject.transform.parent = transform;
                pubbleObject.transform.localPosition = pubblePosition;
                pubbleObject.transform.localScale = pubbleScale;
                //计算该泡泡的周边泡泡
                pubbleObject.GetComponent<PubbleObject>().CalculateAdjacentObjects();
                //如果是到了创建最后一行泡泡，则开始保存这一行泡泡，作为顶部泡泡
                if (countPubbleRowsInPanel == markCreateRowIndex + 1)
                {
                    //记录顶行泡泡的脚本
                    markTopRowPubble[i] = pubbleObject.GetComponent<PubbleObject>();
                }
                //创建的第一个泡泡，将会是最底下的，记录一下,便于后面判断
                SlidePlayPanel.Instance.AssignBottomPubble(pubbleObject);
                //记录当前有效的预备泡泡类型
                CalculatePrefabPubbleType(pubbleInfo.ColorType);
            }

            //如果是到了创建最后一行泡泡，则开始记录该行9个泡泡位置
            if (countPubbleRowsInPanel == markCreateRowIndex + 1)
            {
                markTopPubblePostion[i] = new Vector3(markCurrentPointX, markCurrentPointY, 0);
            }
            //计算X坐标:不断往右平移一个单位坐标,跟一个间隔
            markCurrentPointX += pubbleWidth;
            markCurrentPointX += marginEachPubble;
        }
        //计算Y坐标:不断上移
        markCurrentPointY += markMoveUnit;
        //记录当前创建了多少行泡泡
        markCreateRowIndex++;
    }

    /*
     * @brief       随机钻石
     * @desc        当创建完成所有的泡泡的时候，要随机几个可以得到钻石
     */
    void RandomAllPubbleDiamond()
    {
        UpdatePlayObjectsScriptsList();
        int diamondNum = Random.Range(5, 9);
        ArrayList indexNumArr = new ArrayList();
        for (int i = 0; i < diamondNum; i++)
        {
            int randomIndex = Random.Range(0, storageAllObjectScripts.Length);
            int indexOfArr = indexNumArr.IndexOf(randomIndex);
            while (indexOfArr >= 0)
            {
                //表示存在，则继续随机
                randomIndex = Random.Range(0, storageAllObjectScripts.Length);
                indexOfArr = indexNumArr.IndexOf(randomIndex);
            }
            if (randomIndex < storageAllObjectScripts.Length)
            {
                storageAllObjectScripts[randomIndex].ChangeDiamongMark();
            }
        }
    }
    #endregion 负责泡泡创建:首次创建全部，后期极限模式，单行创建
    
    

    #region 功能函数
    /*
     * @brief       在PlayLogic内检测所有泡泡的毁灭  以及  掉落
     */
    public void CheckAllPubbleFallOrBurstInLogic()
    {
        if (GetIsCanBurstState())
        {
            //更新存储的脚本数组:保证数据最新
            UpdatePlayObjectsScriptsList();
            //销毁了，则判断是否需要后退
            CheckBurstTimesToPush();
            //达到毁灭条件：执行摧毁
            BurstMarkedPubbleObject();
        }
        //掉落没有关联的泡泡
        CheckFallDownOnly();
    }

    /*
     * @brief       负责检测 有没有达到毁灭要求
     * @brief       bool 如果达到了毁灭则，返回ture，未达到则返回false
     * @desc        如果达不到，则将 毁灭波数 置为0 ，同时还原所有的脚本
     */
    bool GetIsCanBurstState()
    {
        //检测统计数,如果达不到界限 而且 不能强制毁灭
        if (markNeedBurstAccount < limitBurstCount && !GetIsCanForceBurstState())
        {
            //此处置为 0 ，是因为检测是否有需要毁灭，只在此处检测置0
            ResetBurstTimes();
            //如果达不到毁灭条件:重置原先的数据
            ResetAllObjects();
            return false;
        }
        return true;
    }


    /*
     * @brief       检测当前是否需要强力毁灭
     * @desc        如果发射泡泡是爆炸泡泡 或者 被撞击者 为 闪电泡泡，则需要强力毁灭
     */
    bool GetIsCanForceBurstState()
    {
        if (markShootType == PubbleColorType.PUBBLE_FIRE_TYPE || markCollidedPubbleType == PubbleColorType.PUBBLE_THUNDER_TYPE)
        {
            //如果发射泡泡为爆炸，或者被撞击泡泡为闪电，则强力毁灭
            return true;
        }
        return false;
    }

    /*
     * @brief       还原数据
     * @desc        当发射完泡泡，进行检测的时候，达不到条件，则还原数据
     */
    public void ResetAllObjects()
    {
        //统计数值   置0
        markNeedBurstAccount = 0;
        //还原发射泡泡的类型
        markShootType = PubbleColorType.PUBBLE_EMPTY_TYPE;
        //重新update  页面上所有的泡泡 以及泡泡  绑定的脚本
        UpdatePlayObjectsScriptsList();
        //重置所有脚本
        for (int i = 0; i < storageAllObjectScripts.Length; i++)
        {
            storageAllObjectScripts[i].ResetPubbleState();
        }
    }

    /*
     * @brief       只还原，毁灭标记
     * @desc        当击中的是掉落道具泡泡的时候执行，不执行原有的毁灭，只执行道具泡泡的功能
     */
    internal void ResetAllObjectsBurstMark()
    {
        //统计数值   置0
        markNeedBurstAccount = 0;
        //重新update  页面上所有的泡泡 以及泡泡  绑定的脚本
        UpdatePlayObjectsScriptsList();
        //重置所有脚本
        for (int i = 0; i < storageAllObjectScripts.Length; i++)
        {
            storageAllObjectScripts[i].ResetPubbleState();
        }
    }

    /*
     * @brief       更新脚本存储列表，便于获取最新的脚本状态
     * @desc        使用时机：(1)要摧毁被标记的泡泡，获取该数组，挨个遍历哪个需要被摧毁
     * @desc                  (2)达不到摧毁条件，获取该数组，挨个遍历还原设置
     * @desc                  (3)游戏初始，保有一个数组
     */
    internal void UpdatePlayObjectsScriptsList()
    {
        //根据tag值获取列表
        GameObject[] currentObjects = GameObject.FindGameObjectsWithTag(ConstantValue.PlayObjectTag);
        //所有泡泡绑定的脚本数组
        storageAllObjectScripts = null;
        storageAllObjectScripts = new PubbleObject[currentObjects.Length];
        for (int i = 0; i < currentObjects.Length; i++)
        {
            //保存所有脚本
            PubbleObject pubbleScript = currentObjects[i].GetComponent<PubbleObject>();
            storageAllObjectScripts[i] = pubbleScript;
        }
    }

    /*
     * @brief       摧毁被标记的泡泡
     * @desc        以发射泡泡为中心，开始向周围消除
     */
    public GameObject shootGameObject = null;
    void BurstMarkedPubbleObject()
    {
        ArrayList burstPubbles = new ArrayList();
        //
        PubbleObject shootScript = shootGameObject.GetComponent<PubbleObject>();
        for (int i = 0; i < storageAllObjectScripts.Length; i++)
        {
            if (storageAllObjectScripts[i].isBurst)
            {
                if (shootScript != storageAllObjectScripts[i])
                { 
                    //如果与发射泡泡 不一致，则保存
                    burstPubbles.Add(storageAllObjectScripts[i]);
                }
            }
        }

        //将发射泡泡脚步放入数组
        ArrayList myArr = new ArrayList();
        myArr.Add(shootScript);
        //存放后最后的 距离数组
        ArrayList lastArr = new ArrayList();
        lastArr.Add(myArr);

        //先找到最小的距离
        Transform shootTransform = shootGameObject.transform;        
        while (burstPubbles.Count > 0)
        {
            //求出最小的 距离
            float minDis = 0f;
            int myIndex = 0;
            while (myIndex < burstPubbles.Count)
            {
                PubbleObject pubbleScript = burstPubbles[myIndex] as PubbleObject;
                float marginDis = Vector3.Distance(shootTransform.position, pubbleScript.GetPubbleObjectTransform().position);
                if (minDis == 0) minDis = marginDis;
                minDis = minDis < marginDis ? minDis : marginDis;
                myIndex++;
            }

            //开始将符合最小距离的脚本存储起来
            ArrayList minDisPubbles = new ArrayList();
            myIndex = 0;
            while (myIndex < burstPubbles.Count)
            {
                PubbleObject pubbleScript = burstPubbles[myIndex] as PubbleObject;
                float marginDis = Vector3.Distance(shootTransform.position, pubbleScript.GetPubbleObjectTransform().position);
                if (Mathf.Abs(marginDis - minDis) < 0.02f)
                {
                    //如果符合最小距离的条件:保存，并移除
                    minDisPubbles.Add(pubbleScript);
                    burstPubbles.Remove(pubbleScript);
                }
                else
                    //如果不符合该距离则 ++
                    myIndex++;
            }
            if (minDisPubbles.Count > 0)
                lastArr.Add(minDisPubbles);
        }
        StartCoroutine(OrderBurstPubbles(lastArr));
    }

    IEnumerator OrderBurstPubbles(ArrayList arr)
    {
        //开始销毁泡泡
        while (arr.Count > 0)
        {
            ArrayList oneArr = arr[0] as ArrayList;
            for (int i = 0; i < oneArr.Count; i++)
            { 
                PubbleObject script = oneArr[i] as PubbleObject;
                script.BurstMySelf(false);
            }
            arr.Remove(oneArr);
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

   

    /*
     * @brief       只检测是否有可以掉落的泡泡并掉落
     * @desc        在穿刺泡泡结束的时候，会单独调用一下
     */
    public void CheckFallDownOnly()
    {
        for (int i = 0; i < storageAllObjectScripts.Length; i++)
        {
            //全部置为没有关联，后面从顶部开始检测的时候，有关联的置为true，最后掉落 false
            storageAllObjectScripts[i].isConnected = false;
        }
        //顶部的泡泡是一定有关联的
        for (int i = 0; i < rowPubbleNum; i++)
        {
            if (markTopRowPubble[i])
            {
                markTopRowPubble[i].TracePubbleIsConnection();
            }
        }
        //开始将没有关联的泡泡进行掉落
        for (int i = 0; i < storageAllObjectScripts.Length; i++)
        {
            if (storageAllObjectScripts[i])
            {
                if (storageAllObjectScripts[i].isConnected == false && !storageAllObjectScripts[i].isBurst)
                {
                    //如果该泡泡没有关联 ,而且 不是毁灭状态，则执行掉落
                    storageAllObjectScripts[i].BurstMySelf(true);
                }
            }
        }
        //检测哪个是最底的标记
        SlidePlayPanel.Instance.CheckMostBottomTransform();
        //检测是否胜利了:这个时候比较准确
        CheckGameWin();
        //全部掉落完之后，将所有泡泡的标记变量，重新还原
        Invoke("ResetAllObjects", 0.2f);
    }

    


    /*
     * @brief       检测是否胜利了
     * @desc        在还剩20个泡泡之内才计算
     * @desc        检测数组之中除了毁灭的 与掉落的是否还有其他的,没有了则胜利
     */
    public void CheckGameWin()
    {
        bool isWin = true;
        if (storageAllObjectScripts.Length < 20)
        {
            foreach (PubbleObject script in storageAllObjectScripts)
            {
                if (script.isBurst == false && script.isConnected == true)
                {
                    //存在没有掉落的泡泡，未胜利
                    isWin = false;
                    break;
                }
            }
            if (isWin)
            {
                //胜利了
                StartCoroutine(GameManager.Instance.GameWinFunction());
            }
        }
    }

    /*
     * @brief       所有泡泡自由落体
     * @desc        所有泡泡掉落
     * @desc        滑板不再下落
     */
    public void FallDownAllPubble()
    {
        foreach (PubbleObject script in storageAllObjectScripts)
        {
            if (script != null)
                script.FallDownMyPubbleObject();
        }
    }

    #endregion 功能函数

    #region 负责准备可用于发射的泡泡数组
    /*
     * @brief       获取当前界面存在的泡泡 可以发射的泡泡预设数组
     * 规则:游戏一开始，所有的泡泡还没有创建出来，此时脚本数组是空的，则返回null，strike可以选择所有的常用泡泡
     * 如果有泡泡了，则从当前数组之中遍历找到可以发射的类型
     */
    public ArrayList GetRemainObjectPerfabs()
    {
        //当前没有泡泡，则返回null ，随意创建
        if (storageAllObjectScripts.Length == 0)
        {
            return GetPerparePerfabsArr();
        }

        //获取当前界面上所有的泡泡
        GameObject[] allPubbleObjects = GameObject.FindGameObjectsWithTag(ConstantValue.PlayObjectTag);
        //保存有效的当前泡泡名字,所有的不重复name
        ArrayList currentAvailableNames = new ArrayList();
        //保存有效的泡泡预设
        ArrayList currentVailabelPerfabs = new ArrayList();
        foreach (GameObject pleObject in allPubbleObjects)
        {
            //获取该泡泡的名字：-7 是将后面的(Clone) 去掉
            string tmpName = pleObject.name;
            string tempPubbleName = tmpName.Substring(0, tmpName.Length - 7);
            //有效名字内不存在该名字
            if (!currentAvailableNames.Contains(tempPubbleName))
            {
                currentAvailableNames.Add(tempPubbleName);
                //对应去找预设:只有常规的泡泡 预设放在该数组，气泡，石头等不放在这里
                for (int j = 0; j < normalPubbleNumber; j++)
                {
                    if (pubbleKindPrefabs[j].name == tempPubbleName)
                    {
                        currentVailabelPerfabs.Add(pubbleKindPrefabs[j]);
                    }
                }
            }
            //如果这些常用泡泡都存在里面了，跳出循环，防止继续消耗内存
            if (currentVailabelPerfabs.Count == normalPubbleNumber) break;
        }
        return currentVailabelPerfabs.Count > 0 ? currentVailabelPerfabs : null;
    }


    //创建泡泡的时候统计可以出现的泡泡种类,当上面函数获取不到内容的时候，采用该数组
    ArrayList originPubbleArr = new ArrayList();

    /*
     * @brief       统计当前可用的预备泡泡种类
     * @parma       type    传入类型，检测可用性以及是否存在，不存在且可用保存
     */
    void CalculatePrefabPubbleType(PubbleColorType pubbleType)
    {
        if (pubbleType < PubbleColorType.PUBBLE_AIR_TYPE && pubbleType > PubbleColorType.PUBBLE_EMPTY_TYPE)
        {
            //如果该类型符合预备标准，则判断当前数组是否存在
            if (originPubbleArr.IndexOf(pubbleType) < 0)
            {
                //表示当前数组不存在该值
                originPubbleArr.Add(pubbleType);
            }
        }
    }

    /*
     * @brief       当目前没有找到合适的预备泡泡，则
     */
    ArrayList GetPerparePerfabsArr()
    {
        ArrayList perfabsArr = new ArrayList();
        foreach (PubbleColorType colorType in originPubbleArr)
        {
            perfabsArr.Add(pubbleKindPrefabs[(int)colorType - 1]);
        }
        return perfabsArr;
    }

    #endregion 负责准备可用于发射的泡泡数组

    #region 负责处理连消:只要是连消则退半格，中断之后则重新计数
    //计数，负责记录总的连消次数，暂时只是用来检测 达到第二次连消以上才会退半格
    int markBurstTimes = 0;
    /*
     * @brief       检测是不是连消
     */
    void CheckBurstTimesToPush()
    {
        if (markBurstTimes > 0)
        {
            //退半格
            SlidePlayPanel.Instance.ExeFunctionGoOnEffect();
        }
        //当普通泡泡发射，而且未消除的情况下，置为0
        markBurstTimes++;
        PlayAwardEffect();
    }

    const int coolLimitNum = 3;
    const int greatLimitNum = 6;
    const int prefectLimitNum = 9;
    //负责检测当前弹出什么样子的提示效果
    public GameObject coolGameObject = null;
    public GameObject greatGameObject = null;
    public GameObject perfectGameObject = null;
    public GameObject uiRootGameObject = null;

    void PlayAwardEffect()
    {
        GameObject effectObject = null;
        if (markBurstTimes == coolLimitNum)
        {
            effectObject = (GameObject)Instantiate(coolGameObject);
        }
        else if (markBurstTimes == greatLimitNum)
        {
            effectObject = (GameObject)Instantiate(greatGameObject);
        }
        else if (markBurstTimes >= prefectLimitNum)
        {
            effectObject = (GameObject)Instantiate(perfectGameObject);
        }

        if (effectObject != null)
        {
            SoundManager.Instance.PlayCheerClip();
            effectObject.transform.parent = uiRootGameObject.transform;
            effectObject.transform.localScale = Vector3.one;
        }
    }

    public int GetBurstTimes()
    {
        return markBurstTimes;
    }

    public void AddBurstTimes()
    {
        markBurstTimes++;
    }

    void ResetBurstTimes()
    {
        markBurstTimes = 0;
    }

    #endregion 负责处理连消:只要是连消则退半格，中断之后则重新计数


}
