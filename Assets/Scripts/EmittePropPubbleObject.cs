using UnityEngine;
using System.Collections;

public class EmittePropPubbleObject : PubbleObject
{
    public GameObject FireEffectObject = null;
    public GameObject CrushEffectObject = null;


    /*
     * @brief       播放泡泡特效
     * @desc        穿刺泡泡，执行自己的特效，父类特效
     * @desc        炸弹泡泡，以自己为中心播放
     * @desc        冰冻：冻住整个屏幕
     * @desc        冲击波：最后执行
     */
    override public void PlayMyParticlesEffects()
    {
        if (pubbleType == PubbleColorType.PUBBLE_CROSS_TYPE)
        { 
            //穿刺没有效果,当前。可能会给他自己加一个 毁灭效果
        }
        else if (pubbleType == PubbleColorType.PUBBLE_SNOW_TYPE)
        {
            //显示冰冻效果，同时执行冰冻 技能
            SlidePlayPanel.Instance.PlaySnowEffect();
        }
        else if (pubbleType == PubbleColorType.PUBBLE_FIRE_TYPE)
        {
            //炸弹泡泡效果播放
            GameObject fireEffect = (GameObject)Instantiate(FireEffectObject);
            GameObject uiRoot = GameObject.Find("UI Root");
            fireEffect.transform.parent = uiRoot.transform;
            fireEffect.transform.position = transform.position;
            fireEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            Destroy(fireEffect, 2.0f);
        }
        else
        {
            //冲击波泡泡

        }
    }



    /*
     * @brief        如果是撞击的顶部limit:给该泡泡定位位置，同时计算周边泡泡
     * @desc         自身此时是发射的道具泡泡：执行自己该有的功能
     */
    override public void MyObjectCollidedTopLimit(GameObject collidedObject)
    {
        //保存发射泡泡的名字或者类型，此时自身为发射泡泡,后面的时候  保存
        PlayLogic.Instance.markShootType = pubbleType;
        //记录发射泡泡
        PlayLogic.Instance.shootGameObject = gameObject;
        //调整自身的位置:这是顶行，按照顶行的规则，贴边开始
        //计算规则:1.顶行一定是能够填满9个泡泡的位置
        //         2.在创建泡泡的时候，保存顶行的九个位置
        //         3.判断当前泡泡x坐标距离哪个 位置最近，直接赋值
        float gameObjectPosX = gameObject.transform.localPosition.x;
        Vector3 newPostion = Vector3.zero;
        for (int i = 0; i < PlayLogic.Instance.markTopPubblePostion.Length; i++)
        {
            if (Mathf.Abs(gameObjectPosX - PlayLogic.Instance.markTopPubblePostion[i].x) < PlayLogic.Instance.pubbleWidth / 2)
            {
                newPostion = PlayLogic.Instance.markTopPubblePostion[i];
                break;
            }
        }
        gameObject.transform.localPosition = newPostion;
        //计算这个发射泡泡 周边的泡泡数组
        CalculateAdjacentObjects();
        //执行发射泡泡相关  功能
        MyEmissionExecuteFunction();
    }

    /*
     * @brief       如果自己是发射泡泡泡，执行发射泡泡的相关作用
     * @desc        继承父类进行重写:发射的是道具泡泡，执行其功能
     */
    override public void MyEmissionExecuteFunction()
    {
        switch (pubbleType)
        { 
            case PubbleColorType.PUBBLE_CROSS_TYPE:
                //穿刺泡泡:可穿透8个泡泡距离，并消除其路径上的泡泡
                break;
            case PubbleColorType.PUBBLE_FIRE_TYPE:
                //炸弹泡泡:发射出去后，触碰到其他泡泡爆炸，可消除触碰泡泡之外的3层泡泡距
                ExecuteFirePubbleFunction();
                break;
            case PubbleColorType.PUBBLE_SNOW_TYPE:
                //冰冻泡泡:时间暂停5S，期间挡板不下降，新泡泡不出现，玩家进行的操作不算到关卡规则中
                ExecuteSnowPubbleFunction();
                break;
            case PubbleColorType.PUBBLE_CRUSH_TYPE:
                //冲击波泡泡:逆推5层
                ExecuteCrushPubbleFunction();
                break;
            default:
                break;
        }
    }

    /*
     * @brief       执行穿透泡泡功能
     * @desc        这个好他妈纠结，跟大众不一个思路，最后写
     */
    void ExecuteCrossPubbleFunction()
    { 
        //这个在发射检测的时候做了其相应的功能
    }

    /*
     * @brief       执行炸弹泡泡效果
     * @desc        ：统计周围泡泡两层（一共12个方向的射线）
     * @desc        ：并设置为毁灭标记，执行各自动画效果，爆炸特效以自身中心
     */
    void ExecuteFirePubbleFunction()
    {
        //射线长度用0.17
        //用父类的六个角度，同时每个角度基础上+ 30度
        float marginAngle = 30.0f;
        for (int i = 0; i < adjacentObjectsAngles.Length; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //这个数很纠结，牵扯到不同坐标系
                float maxDistance = 0.17f;
                //将度数转为弧度
                float radAngle = (adjacentObjectsAngles[i] + marginAngle*j) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(radAngle), Mathf.Sin(radAngle), 0);
                RaycastHit[] hitResults = Physics.RaycastAll(transform.position, dir, maxDistance, pubbleLayerMask);
                for (int k = 0; k < hitResults.Length; k++)
                {
                    GameObject hitGameObject = hitResults[k].collider.gameObject;
                    //根据tag值找:记得设置tag值
                    if (hitGameObject.tag == ConstantValue.PlayObjectTag)
                    {
                        hitGameObject.GetComponent<PubbleObject>().isBurst = true;
                    }
                }
            }
        }
        //设置自身的为毁灭状态
        isBurst = true;
        //延后0.2s执行爆炸效果以及检测掉落
        Invoke("CheckAllObjectsFallOrBurst", 0.2f);
    }

    /*
     * @brief       执行冰冻泡泡的效果
     * @desc        撞击之后，将暂停时间5s
     * @desc        所有的泡泡上蒙一层雪花
     */
    void ExecuteSnowPubbleFunction()
    { 
        //执行动画效果:冰冻
        PlayMyParticlesEffects();
        //销毁自己
        BurstMySelf(false);
    }

    /*
     * @brief       冲击波泡泡
     * @desc        直接将泡泡打回一定距离
     * @desc        穿刺泡泡，毁灭
     * @desc        滑板停止运动
     * @desc        推板从下面运动上来，暂时采用快速下落的 速度来执行
     * @desc        当滑板达到 最底端泡泡的时候，开始推动滑板 向上运动 5 格
     */
    void ExecuteCrushPubbleFunction()
    {
        GameObject crushEffect = Instantiate(CrushEffectObject) as GameObject;
        crushEffect.transform.parent = transform.parent;
        crushEffect.transform.localScale = new Vector3(1,1,1);
        GameObject secondEmition = GameObject.Find(ConstantValue.SecondPositionObjName);
        crushEffect.transform.position = secondEmition.transform.position;
        SlidePlayPanel.Instance.ExeFunctionCrushEffect(crushEffect);
        //销毁自身
        BurstMySelf(false);
    }

}
