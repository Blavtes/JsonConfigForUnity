using UnityEngine;
using System.Collections;

/*
 * @brief       发射器，可以算作，绑定在
 * @desc        在屏幕上定好两个坐标点的gameobject，striker 放置在第一个发射点。
 * @desc        striker内部有个Gameobject（用于保存要发射的泡泡）
 */

public class StrikeObject : MonoBehaviour
{
    #region Properties

    //标记发射速度:可能后面关卡会用到
    float markShootSpeed = 3.0f;
    //移动方向
    Vector3 markMoveDirection = Vector3.zero;
    //是否有泡泡在发射，有则不准许再发射
    internal bool isShooting = false;
    //记录当前发射的泡泡:在初始化stiker 的时候赋值
    internal GameObject markShootPubleObject;
    //记录发射泡泡类型:用于做  穿刺泡泡检测
    PubbleColorType shootType;
    #endregion Properties

    #region Member Function

    void Start()
    {
        AssignMarginValue();
        AssignTransLateSpeend();
    }

    /*
     * @brief       发射泡泡
     * @param       dir     发射的方向
     * @desc        当点击了屏幕的时候，会调用该函数，在FixedUpdate执行发射
     */
    public void shootPubble(Vector3 dir)
    {
        shootType = markShootPubleObject.GetComponent<PubbleObject>().pubbleType;
        //做道具泡泡发射的检测
        PlayUIScript.Instance.ChangePropLabelNum(shootType);
        //如果不是穿刺泡泡，则关闭运动学，便于检测
        if (shootType != PubbleColorType.PUBBLE_CROSS_TYPE)
        {
            rigidbody.isKinematic = false;
        }
        else
        {
            //所有泡泡为运动学 碰撞器  与 运动学触发器  能够触发 触发器函数
            GetComponent<SphereCollider>().isTrigger = true;
        }
        markMoveDirection = dir;
        isShooting = true;
    }

    /*
     * @brief       检测并移动自身
     * @desc        跟update 的性质差不多，不过间隔可以由开发者在build setting内设置
     */
    void FixedUpdate()
    {
        //如果当前没有发射泡泡
        if (!isShooting)
            return;
        //自身沿着markMoveDirection方向以markShootSpeed的速度移动
        transform.Translate(markMoveDirection * markShootSpeed * Time.deltaTime);
    }


    Vector3 transLateSpeed;
    float transDis = 0f;
    void AssignTransLateSpeend()
    {
        transLateSpeed = markMoveDirection * markShootSpeed * Time.deltaTime;
        transDis = Vector3.Distance(Vector3.zero, transLateSpeed);
    }

    //每一帧检测碰撞物体
    //所有泡泡所在的层
    public LayerMask pubbleLayer;
    //两个边界的层
    public LayerMask boundLayer;

    void CheckCollider()
    {
        RaycastHit hit;
        //检测到泡泡的距离
        if (Physics.Raycast(transform.position, markMoveDirection, out hit, transDis, pubbleLayer))
        {
            //根据tag值找:记得设置tag值
            if (hit.collider.gameObject.tag == ConstantValue.PlayObjectTag)
            {
                //如果检测到了前方是泡泡，则return

            }
        }
        else if (Physics.Raycast(transform.position, markMoveDirection, out hit, transDis, boundLayer))
        {
            //如果检测到的是 边界

        
        }
    }
    #endregion Member Function



    #region 负责检测碰撞与触发
    public GameObject leftPositionObject = null;
    public GameObject rightPositonObject = null;
    Vector3 leftPositionLimit;
    Vector3 rightPositionLimit;
    bool hasAssignValue = false;
    void AssignMarginValue()
    {
        if (leftPositionObject != null && rightPositonObject != null)
        {
            hasAssignValue = true;
            leftPositionLimit = leftPositionObject.transform.position;
            rightPositionLimit = rightPositonObject.transform.position;
        }
    }

    /*
     * @brief       当自身碰撞到了 碰撞器，执行该函数（继承自父类）
     * @param       other       被碰撞到的物体
     */
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("collision is x :" + other.contacts[0].point.x + "   y  :" + other.contacts[0].point.y + "  z  :" + other.contacts[0].point.z);
        Debug.Log("OnCollisionEnter  1 :" + other.gameObject.name);
        //如果当前没有在playing状态下不做任何的检测碰撞
        if (!GameManager.Instance.CurrentStateIsPlayingState()) return;
        //由于所有的节点都以UIRoot为根结点，其上绑定了一个rigibody，因此有时候会检测到他，所以判断一下
        if (rigidbody.isKinematic && other.gameObject.name != ConstantValue.UIRootName)
            return;
        Debug.Log("OnCollisionEnter  2 :" + other.gameObject.name);
        if (other.gameObject.name == ConstantValue.LeftMarginName || other.gameObject.name == ConstantValue.RightMarginName)
        {
            HandleCollisionMargin(other);
            Debug.Log("OnCollisionEnter  3 :" + other.gameObject.name);
        }
        else if (other.gameObject.tag == ConstantValue.PlayObjectTag && isShooting)
        {
            //如果碰到了其他泡泡
            HandleCollisionOtherPubble(other);
            //道具泡泡不扣除步数：放在此处的原因 是 ----- 防止最后一个步 不能好好的处理
            if ((int)shootType < (int)PubbleColorType.PUBBLE_AIR_TYPE)
            {
                //修改步数
                PlayModelLogic.Instance.ManageKindModelLogic();
            }
        }
    }


    /*
     * @brief       处理泡泡碰撞到边界
     * @desc        下面分这些情况是为了，将碰撞点的X 坐标使用其真实的边界坐标:为泡泡超出边界的问题而做（边界范围调大了x）
     */
    void HandleCollisionMargin(Collision other)
    {
        //如果碰撞了左右边界,则修改当前的移动方向
        //播放撞墙声音
        //重新改变射线方向(沿着法线反射向量)
        //第一个参数：要被  发射的向量
        //第二个参数: 要反射用的法线   :垂直的叫法线
        //Collision.contacts   表示碰撞点数组
        SoundManager.Instance.PlayCollisionSound();
        markMoveDirection = Vector3.Reflect(markMoveDirection, other.contacts[0].normal).normalized;
        /*
        if (other.gameObject.name == ConstantValue.LeftMarginName && hasAssignValue)
        {
            Vector3 contactPos = new Vector3(-leftPositionLimit.x, other.contacts[0].point.y, other.contacts[0].point.z);
            markMoveDirection = Vector3.Reflect(markMoveDirection, contactPos.normalized).normalized;
        }
        else if (other.gameObject.name == ConstantValue.RightMarginName && hasAssignValue)
        {
            Vector3 contactPos = new Vector3(-rightPositionLimit.x, other.contacts[0].point.y, other.contacts[0].point.z);
            markMoveDirection = Vector3.Reflect(markMoveDirection, contactPos.normalized).normalized;
        }
        else
        {
            markMoveDirection = Vector3.Reflect(markMoveDirection, other.contacts[0].normal).normalized;
        }
         * */
    }

    /*
     * @brief       泡泡遇到其他泡泡的处理函数
     */
    void HandleCollisionOtherPubble(Collision other)
    {
        if (markShootPubleObject.GetComponent<PubbleObject>().pubbleType != PubbleColorType.PUBBLE_CROSS_TYPE)
        {
            //非穿刺泡泡，他们的行为一致
            SoundManager.Instance.PlayCollisionSound();
            StrikeOtherPubbleObject(other.gameObject);
        }
    }
    #endregion 负责检测碰撞与触发
    


    /*
     * @brief       如果碰撞到了顶部的警戒线，则该泡泡停留在此处
     * @parama      other 被碰撞的物体
     */
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter   : " + other.gameObject.name);
        //如果不在Playing状态下不做任何的触发
        if (!GameManager.Instance.CurrentStateIsPlayingState()) return;
        if (other.gameObject.name == ConstantValue.TopLimitName && markShootPubleObject.GetComponent<PubbleObject>().pubbleType != PubbleColorType.PUBBLE_CROSS_TYPE)
        {
            //判断当前的触发器是不是顶部的警戒线,同时不是穿刺泡泡的情况下，执行重新设置位置
            SoundManager.Instance.PlayCollisionSound();
            HandleCollideTopLimit(other);
            //道具泡泡不扣除步数：放在此处的原因 是 ----- 当打击挡板的时候不减少步数
            if ((int)shootType < (int)PubbleColorType.PUBBLE_AIR_TYPE)
            {
                //修改步数
                PlayModelLogic.Instance.ManageKindModelLogic();
            }
        }
        else if (other.gameObject.name == ConstantValue.TopLimitName && markShootPubleObject.GetComponent<PubbleObject>().pubbleType == PubbleColorType.PUBBLE_CROSS_TYPE)
        { 
            //如果是穿刺泡泡，撞击到了顶部，直接销毁
            BurstCrossPubble();
        }
        else if (other.gameObject.name == ConstantValue.AirConeName)
        {
            //如果碰撞到了气泡，则气泡爆裂
            HandleAirPubbleBurst(other);
        }
        else if (other.gameObject.name == ConstantValue.LeftMarginName || other.gameObject.name == ConstantValue.RightMarginName)
        {
            //此时是 穿刺泡泡碰撞到了左右边界，播放声音
            SoundManager.Instance.PlayCollisionSound();
            //如果碰撞了左右边界,则修改当前的移动方向
            //播放撞墙声音
            //重新改变射线方向(沿着法线反射向量)
            //第一个参数：要被  发射的向量
            //第二个参数: 要反射用的法线   :垂直的叫法线
            //Collision.contacts   表示碰撞点数组
            //markMoveDirection = Vector3.Reflect(markMoveDirection, other.contacts[0].normal).normalized;
            if (other.gameObject.name == ConstantValue.LeftMarginName)
                markMoveDirection = Vector3.Reflect(markMoveDirection, new Vector3(1, 0, 0)).normalized;
            else
                markMoveDirection = Vector3.Reflect(markMoveDirection, new Vector3(-1, 0, 0)).normalized;
        }
        else if (other.gameObject.tag == ConstantValue.PlayObjectTag && isShooting && shootType == PubbleColorType.PUBBLE_CROSS_TYPE)
        {
            if (markCrossBurstCount < limitCrossBurstCount)
            {
                //计数
                CountBurstTime();
                //销毁这个被碰撞到的泡泡
                other.gameObject.GetComponent<PubbleObject>().BurstMySelf(false);
                markCrossBurstCount++;
            }
            else
            {
                //已经到达界限了：销毁穿刺泡泡，同时还原个数，检测是否有掉落的泡泡
                BurstCrossPubble();
            }
        }
    }

    #region 负责泡泡碰撞的函数

    /*
     * @brief       碰撞到了其他的泡泡
     * @param       otherObject     被撞击的泡泡
     */
    private void StrikeOtherPubbleObject(GameObject otherObject)
    {
        rigidbody.isKinematic = true;
        //激活球体碰撞器
        markShootPubleObject.GetComponent<SphereCollider>().enabled = true;
        //发射泡泡要与其他泡泡有相同的父节点，便于一样处理
        markShootPubleObject.transform.parent = PlayLogic.Instance.transform;
        //设置同样的tag
        markShootPubleObject.tag = ConstantValue.PlayObjectTag;
        //发射泡泡执行 撞击函数
        markShootPubleObject.GetComponent<PubbleObject>().MyObjectCollidedOtherObject(otherObject);
        //标记没有移动的泡泡了，可以继续发射了
        isShooting = false;
        //创建第一个发射泡泡
        StartCoroutine(StrikeManager.Instance.InitStrikeEmitterBySecondPubble());
    }

    //记录穿刺泡泡毁灭的泡泡数
    int markCrossBurstCount = 0;
    //用于限制穿刺个数
    const int limitCrossBurstCount = 8;
    /*
     * @brief       销毁穿刺泡泡
     * @desc        当穿刺个数到达8个或者碰撞到顶部界限的时候，毁灭穿刺泡泡，同时检测是否掉落
     */
    void BurstCrossPubble()
    {
        //发射器关闭触发器,因为在发射的时候，判断是否为穿刺泡泡时候修改了
        GetComponent<SphereCollider>().isTrigger = false;
       //发射器启用运动学 
        rigidbody.isKinematic = true;
        //销毁穿刺泡泡
        markShootPubleObject.GetComponent<PubbleObject>().BurstMySelf(false);
        //标记没有移动的泡泡了，可以继续发射了
        isShooting = false;
        //还原穿刺数
        markCrossBurstCount = 0;
        //创建第一个发射泡泡
        StartCoroutine(StrikeManager.Instance.InitStrikeEmitterBySecondPubble());
        //更新所有的脚本数组，保证最新
        PlayLogic.Instance.UpdatePlayObjectsScriptsList();
        //检测是否有掉落的
        PlayLogic.Instance.CheckFallDownOnly();
        //还原
        ResetMarkBurstTime();
    }

    /*
     * @brief       碰撞到顶部警戒线做的处理函数
     */
    void HandleCollideTopLimit(Collider other)
    {
        //启用运动学
        rigidbody.isKinematic = true;
        //激活泡泡的球体碰撞器
        markShootPubleObject.GetComponent<SphereCollider>().enabled = true;
        //发射泡泡要与其他泡泡有相同的父节点，便于一样处理
        markShootPubleObject.transform.parent = PlayLogic.Instance.transform;
        //设置同样的tag
        markShootPubleObject.tag = ConstantValue.PlayObjectTag;
        //发射泡泡执行 撞击函数
        markShootPubleObject.GetComponent<PubbleObject>().MyObjectCollidedTopLimit(other.gameObject);
        //标记没有移动的泡泡了，可以继续发射了
        isShooting = false;
        //发射器关闭触发器,因为在发射的时候，判断是否为穿刺泡泡时候修改了
        GetComponent<SphereCollider>().isTrigger = false;
        //创建第一个发射泡泡
        StartCoroutine(StrikeManager.Instance.InitStrikeEmitterBySecondPubble());
    }

    /*
     * @brief       触发的时候如果是遇到了气泡，则气泡爆裂
     */
    void HandleAirPubbleBurst(Collider other)
    {
        other.gameObject.GetComponent<PubbleObject>().BurstMySelf(false);
    }

    #endregion 负责泡泡碰撞的函数


    #region 负责记录铁球的毁灭
    //是否记录了铁球的毁灭
    bool hadMarkBurstTime = false;
    /*
     * @brief       当穿刺泡泡碰撞其他泡泡的时候调用
     */
    void CountBurstTime()
    {
        if (!hadMarkBurstTime)
        {
            hadMarkBurstTime = true;
            //
            PlayLogic.Instance.AddBurstTimes();
        }
    }

    /*
     * @brief       还原标记变量
     * @desc        在穿刺泡泡毁灭的时候调用
     */
    void ResetMarkBurstTime()
    {
        hadMarkBurstTime = false;
    }
    #endregion 负责记录铁球的毁灭
}
