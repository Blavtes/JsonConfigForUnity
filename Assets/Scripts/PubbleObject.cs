using UnityEngine;
using System.Collections;

/*
 * @brief       作为所有泡泡的基类
 * @desc        10.09代码重构
 * @Author      King               
 */

public class PubbleObject : MonoBehaviour {

    public float onDrawRadiaus = 0.05f;
    public float onDrawY = 0.05f;
    //泡泡类型
    public PubbleColorType pubbleType;
    //特效gameobject :父类内赋值,子类内播放
    public GameObject pubbleEffectObject = null;
    //离子特效类型
    public PubbleEffectType pubbleEffectType = PubbleEffectType.PUBBLE_EFFECT_EMPTY_TYPE;



    //做射线检测用，只在该layer层
    public LayerMask pubbleLayerMask;
    //默认层，当掉落的时候改为该层
    LayerMask defaultLayer;
    void Awake()
    {
        pubbleLayerMask = 1 << (LayerMask.NameToLayer("PlayObject"));//实例化mask到cube这个自定义的层级之上。
        //此处手动修改layer ，因为NGUI的layer 默认初始化为UI,一定要写在Awake函数
        gameObject.layer = ConstantValue.PubbleMaskLayer;
        defaultLayer = 1 << (LayerMask.NameToLayer("Default"));
    }

    public void Start()
    {
        gameObject.layer = ConstantValue.PubbleMaskLayer;
    }

    /*
     * @brief       修改本泡泡的类型
     * @desc        当遇到变色泡泡的时候，其周围的泡泡要变成发射泡泡类型
     * @desc        分情况：当前是普通泡泡调用该方法 //当前是掉落道具泡泡，当前是发射道具泡泡
     * @desc        修改显示的图片
     * @desc        修改gameobject的名字
     * @desc        类型
     * @desc        离子特效
     * @desc        LayerMask
     */
    public void ChangeNormalPubbleTypeToMarkType()
    {
        //变色的时候，只会是普通的常规泡泡,该步骤设置显示
        string pubbleSpriteName = "";
        string perfabName = "";
        switch (PlayLogic.Instance.markShootType)
        { 
            case PubbleColorType.PUBBLE_YELLOW_TYPE :
                pubbleSpriteName = ConstantValue.YellowFile;
                perfabName = ConstantValue.YellowConeName;
                break;
            case PubbleColorType.PUBBLE_ORANGE_TYPE :
                pubbleSpriteName = ConstantValue.OrangeFile;
                perfabName = ConstantValue.OrangeConeName;
                break;
            case PubbleColorType.PUBBLE_CYAN_TYPE :
                pubbleSpriteName = ConstantValue.CyanFile;
                perfabName = ConstantValue.CyanConeName;
                break;
            case PubbleColorType.PUBBLE_PURPLE_TYPE :
                pubbleSpriteName = ConstantValue.PurpleFile;
                perfabName = ConstantValue.PurpleConeName;
                break;
            case PubbleColorType.PUBBLE_BLUE_TYPE :
                pubbleSpriteName = ConstantValue.BlueFile;
                perfabName = ConstantValue.BlueConeName;
                break;
            case PubbleColorType.PUBBLE_GREEN_TYPE :
                pubbleSpriteName = ConstantValue.GreenFile;
                perfabName = ConstantValue.GreenConeName;
                break;
            case PubbleColorType.PUBBLE_RED_TYPE :
                pubbleSpriteName = ConstantValue.RedFile;
                perfabName = ConstantValue.RedConeName;
                break;
            default:
                break;
        }
        //修改显示图片
        gameObject.GetComponent<UISprite>().spriteName = pubbleSpriteName;
        //修改名称
        gameObject.name = perfabName;

        //如果当前存在color脚本
        //直接修改类型，离子特效（maskLayer 一样不用修改）
        ColorPubbleObject colorScript = gameObject.GetComponent<ColorPubbleObject>();
        //修改类型:修改了类型，离子特效就随着修改了
        colorScript.pubbleType = PlayLogic.Instance.markShootType;
    }

    /*
     * @brief       当彩虹泡泡修改道具泡泡为标记泡泡的时候调用方法
     * @desc        将
     */
    public void ChangePropPubbleToMarkPubble()
    {
        Destroy(gameObject);
        //clone 一个泡泡
        GameObject clonePubble = Instantiate(PlayLogic.Instance.pubbleKindPrefabs[(int)PlayLogic.Instance.markShootType - 1]) as GameObject;
        clonePubble.transform.parent = transform.parent;
        clonePubble.transform.localPosition = transform.localPosition;
        clonePubble.transform.localScale = transform.localScale;
        
        clonePubble.GetComponent<PubbleObject>().CalculateAdjacentObjects();

        //修改完之后要重新计算自己以及自己周围的泡泡 周围的 泡泡

    }



    /*
     * @brief       给泡泡特效赋值
     */
    public void AssignEffectValue()
    {
        //父类内，赋值 特效类型，同时获取特效对象
        if (pubbleEffectType == PubbleEffectType.PUBBLE_EFFECT_EMPTY_TYPE)
        {
            //特效类型与颜色类型，索引值一样
            pubbleEffectType = (PubbleEffectType)pubbleType;
            //赋值特效
            if (PlayLogic.Instance.pubbleEffectPerfabs.Length >= (int)pubbleEffectType)
            {
                pubbleEffectObject = PlayLogic.Instance.pubbleEffectPerfabs[(int)pubbleEffectType - 1];
            }
        }
    }

    /*
     * @brief       播放动画效果
     */
    virtual public void PlayMyParticlesEffects()
    {
        AssignEffectValue();
        if (pubbleEffectObject != null)
        {
            GameObject particlesObj = Instantiate(pubbleEffectObject) as GameObject;
            particlesObj.gameObject.transform.parent = gameObject.transform.parent;
            particlesObj.transform.localScale = new Vector3(1,1,1);
            particlesObj.transform.position =  new Vector3(transform.position.x,transform.position.y,transform.position.z);
            Destroy(particlesObj, 2f);
        }
    }



    //每个泡泡周边的泡泡数量 6
    static int adjacentObjectsNumber = 6;
    //周边6个泡泡所在的角度
    public static float[] adjacentObjectsAngles = { 0, 60, 120, 180, 240, 300 };
    //周边6个泡泡相对于本泡泡的角度数组
    public PubbleObject[] adjacentPlayObjects;
    /*
     * @brief       计算本泡泡周边的那 6个泡泡
     * @desc        三种情况下调用该方法:创建的时候，发射泡泡撞击到其他泡泡，发射泡泡撞击到顶部
     */
    internal void CalculateAdjacentObjects()
    {
        //赋值为null 便于释放内存
        adjacentPlayObjects = null;
        adjacentPlayObjects = new PubbleObject[adjacentObjectsNumber];

        for (int i = 0; i < adjacentObjectsNumber; i++)
        {
            PubbleObject tempObject = GetObjectInTheDirection(adjacentObjectsAngles[i]);
            adjacentPlayObjects[i] = tempObject;
            if (tempObject)
            {
                //此处3 为相邻点的相对位置差
                //B在A的正左边（即0度）也就是A.adjacentPlayingObjects[0]
                //则A在B的正右边(即180度)也就是B.adjacentPlayingObjects[0 + 3]
                if (i < 3)
                {
                    tempObject.adjacentPlayObjects[i + 3] = this;
                }
                else
                {
                    tempObject.adjacentPlayObjects[i - 3] = this;
                }
            }
        }
    }

    /*
     * @brief       根据传入的角度，找到本泡泡，这个角度上的 相邻泡泡
     * @desc        通过射线碰撞的原理(计算泡泡后边泡泡 的时候调用)
     */
    PubbleObject GetObjectInTheDirection(float angle)
    {
        //光线投射碰撞类
        RaycastHit hit;
        //这个数很纠结，牵扯到不同坐标系        
        float maxDistance = 0.1f;

        //将度数转为弧度
        float radAngle = angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(radAngle), Mathf.Sin(radAngle), 0);
        if (Physics.Raycast(transform.position, dir, out hit, maxDistance, pubbleLayerMask))
        {
            //根据tag值找:记得设置tag值
            if (hit.collider.gameObject.tag == ConstantValue.PlayObjectTag)
            {
                return hit.collider.gameObject.GetComponent<PubbleObject>();
            }
            print(hit.collider.gameObject.name); // coz of striker
        }
        return null;
    }

    /*
     * @brief       将自身从周边泡泡的周边数组中删除
     * @desc        当自身毁灭的时候调用
     */
    public void RemoveMeFromNeighbourAdjacentObjects()
    {
        for (int i = 0; i < adjacentObjectsNumber; i++)
        {
            //如果周边泡泡 不为 null
            if (adjacentPlayObjects != null && i < adjacentPlayObjects.Length &&  adjacentPlayObjects[i])
            {
                if (i < 3)
                {
                    adjacentPlayObjects[i].adjacentPlayObjects[i + 3] = null;
                }
                else
                {
                    adjacentPlayObjects[i].adjacentPlayObjects[i - 3] = null;
                }
            }
        }
    }






    /*
     * @brief       自身撞击到其他泡泡的时候，处理函数,自身为发射泡泡
     * @param       collidedObject : 表示被发射上来的泡泡撞击到的那个泡泡
     * @desc        当发射泡泡碰撞到collidedObject 后开始检测时调用
     * @desc        此处需要注意：发射泡泡是普通泡泡还是道具泡泡
     * @desc        此处需要注意：被击中泡泡是普通泡泡还是道具泡泡
     */
    public void MyObjectCollidedOtherObject(GameObject collidedObject)
    {
        //保存发射泡泡的gameobject 用于以自己为中心开始向周围爆炸
        PlayLogic.Instance.shootGameObject = gameObject;
        //获取被碰撞物体的object
        PubbleObject objectScript = collidedObject.GetComponent<PubbleObject>();
        //保存发射泡泡的名字或者类型，此时自身为发射泡泡,后面的时候  保存
        PlayLogic.Instance.markShootType = pubbleType;
        PlayLogic.Instance.markCollidedPubbleType = objectScript.pubbleType;
        //调整自身的位置，因为刚刚发射来，所以要与被撞击者collidedObject 有合理的相对位置
        AdjustMyObjectPosition(collidedObject);

        //计算这个发射泡泡 周边的泡泡数组
        CalculateAdjacentObjects();
        //把刚刚发射的泡泡作为准则去判断，是否为最底下的标记
        SlidePlayPanel.Instance.CheckWhoIsMostObject(gameObject);
        //播放碰撞效果()
        PlayCollideEffects();

        //规则：如果发射泡泡 为道具泡泡，则 发射泡泡优先
        //      如果普通泡泡，撞击了道具泡泡，则掉落泡泡 优先
        //      如果是普通泡泡，撞击了普通泡泡，则无所谓了
        if ((int)pubbleType <= (int)PubbleColorType.PUBBLE_STONE_TYPE)
        {
            if (PlayLogic.Instance.markCollidedPubbleType >= PubbleColorType.PUBBLE_THUNDER_TYPE)
            {
                //掉落道具泡泡，执行其行为，则：还原PlayCollideEffects 内标记的毁灭信号，发射泡泡为道具的时候不需要执行，不会有标记
                PlayLogic.Instance.ResetAllObjectsBurstMark();
                //如果被撞击者 为 道具泡泡，执行其行为
                objectScript.ExefunctionDropProp();
            }
            else
            {
                //执行发射泡泡相关  功能,即普通泡泡的相关功能
                MyEmissionExecuteFunction();
            }
        }
        else
        {
            //不可能是 掉落道具泡泡，所以else 只能发射道具泡泡
            //执行发射泡泡相关  功能
            MyEmissionExecuteFunction();
            //掉落道具的功能不执行
            //objectScript.ExefunctionDropProp();
        }
        
        
    }

    /*
     * @brief       记录最底下的那个泡泡
     * @desc        规则，发射泡泡
     */
    void RecordBottomPubbleTranform()
    { 
        
    }

    /*
     * @brief       如果自己是发射泡泡泡，执行发射泡泡的相关作用
     * @desc        各个子类里面重写，实现自己相应的功能
     * @desc        普通泡泡:检测掉落以及毁灭
     * @desc        掉落道具泡泡：不需要做任何操作
     * @desc        发射道具泡泡：重写实现功能
     */
    virtual public void MyEmissionExecuteFunction()
    { 
        
    }

    

    /*
     * @brief       检测是否要掉落泡泡或者  毁灭
     * @desc        就是将
     */
    public void CheckAllObjectsFallOrBurst()
    {
        PlayLogic.Instance.CheckAllPubbleFallOrBurstInLogic();
    }

    /*
     * @desc        如果是撞击的顶部limit:给该泡泡定位位置，同时计算周边泡泡，然后播放效果
     * @desc        当泡泡是道具的时候，在子类里面重写，执行其功能
     */
    virtual public void MyObjectCollidedTopLimit(GameObject collidedObject)
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
                //将自己添加到 顶行数组之中
                PlayLogic.Instance.markTopRowPubble[i] = GetComponent<PubbleObject>();
                break;
            }
        }
        gameObject.transform.localPosition = newPostion;
        //计算这个发射泡泡 周边的泡泡数组
        CalculateAdjacentObjects();
        //执行碰撞效果
        PlayCollideEffects();
        //执行发射泡泡相关  功能
        MyEmissionExecuteFunction();
    }

    /*
     * @brief       此时自身为发射泡泡，遇到其他泡泡后，设置自身的位置
     * @parma       collidedObt       被撞击的泡泡
     * @desc        由于道具泡泡也会发射出来，所以提取到基类了
     */
    public void AdjustMyObjectPosition(GameObject collidedObt)
    {
        Vector3 collidedObtPos = collidedObt.transform.localPosition;
        float x, y;
        //如果自身的位置在泡泡的左边:正左边，左上角，左下角
        //将泡泡的尺寸分为3 等，上等  左上，  中等， 正左，  下等左下
        if (transform.localPosition.x < collidedObtPos.x)
        {
            if (transform.localPosition.y - collidedObtPos.y > PlayLogic.Instance.pubbleWidth / 3)
            {
                //如果在左上角:一定不会超出左边界，如果超出，则发射不上来
                x = collidedObtPos.x - PlayLogic.Instance.pubbleWidth / 2 - PlayLogic.Instance.marginEachPubble / 2;
                y = collidedObtPos.y + PlayLogic.Instance.markMoveUnit;
            }
            else if (Mathf.Abs(transform.localPosition.y - collidedObtPos.y) < PlayLogic.Instance.pubbleWidth / 3)
            {
                //如果在正左边
                x = collidedObtPos.x - PlayLogic.Instance.pubbleWidth - PlayLogic.Instance.marginEachPubble;
                y = collidedObtPos.y;
                //判断是否超出左边界  或者  被撞击泡泡左边 有泡泡(碰撞的时候有一定偏移) 3表示正左边
                if (x < PlayLogic.Instance.leftLimitPosX || collidedObt.GetComponent<PubbleObject>().adjacentPlayObjects[3] != null)
                { 
                    //如果超出，则位置移到左下角，（左下角一定没有泡泡）
                    x = collidedObtPos.x - PlayLogic.Instance.pubbleWidth / 2 - PlayLogic.Instance.marginEachPubble / 2;
                    y = collidedObtPos.y - PlayLogic.Instance.markMoveUnit;
                }
            }
            else
            {
                //如果在左下角
                x = collidedObtPos.x - PlayLogic.Instance.pubbleWidth / 2 - PlayLogic.Instance.marginEachPubble / 2;
                y = collidedObtPos.y - PlayLogic.Instance.markMoveUnit;
                //判断是否超出左边界  && 右下角没有泡泡 :移到右下角
                if (x < PlayLogic.Instance.leftLimitPosX && collidedObt.GetComponent<PubbleObject>().adjacentPlayObjects[5] == null)
                {
                    //如果超出，则位置移到右下角
                    x = collidedObtPos.x + PlayLogic.Instance.pubbleWidth / 2 + PlayLogic.Instance.marginEachPubble / 2;
                }
            }
        }
        //  如果在右边：右上，正右，右下
        else
        {
            if (transform.localPosition.y - collidedObtPos.y > PlayLogic.Instance.pubbleWidth / 3)
            {
                //如果在右上角:一定不会超出右边界
                x = collidedObtPos.x + PlayLogic.Instance.pubbleWidth / 2 + PlayLogic.Instance.marginEachPubble / 2;
                y = collidedObtPos.y + PlayLogic.Instance.markMoveUnit;
            }
            else if (Mathf.Abs(transform.localPosition.y - collidedObtPos.y) <  PlayLogic.Instance.pubbleWidth / 3 )
            {
                //如果在正右
                x = collidedObtPos.x + PlayLogic.Instance.pubbleWidth + PlayLogic.Instance.marginEachPubble;
                y = collidedObtPos.y;
                //判断是否超出右边界,或者 正右边有泡泡
                if (x > PlayLogic.Instance.rightLimitPosX || collidedObt.GetComponent<PubbleObject>().adjacentPlayObjects[0] != null)
                {
                    //如果超出，则位置移到右下角，（右下角一定没有泡泡）
                    x = collidedObtPos.x + PlayLogic.Instance.pubbleWidth / 2 + PlayLogic.Instance.marginEachPubble / 2;
                    y = collidedObtPos.y - PlayLogic.Instance.markMoveUnit;
                }
            }
            else
            {
                //如果在右下角
                x = collidedObtPos.x + PlayLogic.Instance.pubbleWidth / 2 + PlayLogic.Instance.marginEachPubble/2;
                y = collidedObtPos.y - PlayLogic.Instance.markMoveUnit;
                //判断是否超出右边界 而且 左下角没有泡泡:超出左移 一个单位，至左下角
                if (x > PlayLogic.Instance.rightLimitPosX && collidedObt.GetComponent<PubbleObject>().adjacentPlayObjects[4] == null)
                {
                    x = collidedObtPos.x - PlayLogic.Instance.pubbleWidth / 2 - PlayLogic.Instance.marginEachPubble / 2;
                }
            }
        }
        //判断有没有超出左右边界
        //左右边界只能用作 判断不能用作计算
        if (x < PlayLogic.Instance.leftLimitPosX)
        {
            //如果超出做边界,则缩进半个距离
            //x = PlayLogic.Instance.leftLimitPosX + PlayLogic.Instance.pubbleWidth + PlayLogic.Instance.marginEachPubble;
            Debug.Log("超出了左边界.......");
        }
        else if (x > PlayLogic.Instance.rightLimitPosX)
        {
            //x = PlayLogic.Instance.rightLimitPosX - (PlayLogic.Instance.pubbleWidth + PlayLogic.Instance.marginEachPubble);
            Debug.Log("超出了右边界.......");
        }

        Vector3 orginPosition = transform.localPosition;
        Vector3 newPos = new Vector3(x, y, 0);
        transform.localPosition = newPos;

        if ((Mathf.Abs(x - collidedObtPos.x) < 10 && Mathf.Abs(y - collidedObtPos.y) < 10))
        {
            OutLog.Log("orgin x :" + orginPosition.x + "   orgin y :" + orginPosition.y);
            OutLog.Log("colli x :" + collidedObtPos.x + "   orgin y :" + collidedObtPos.y);
            OutLog.Log("newps x :" + newPos.x + "   newps y :" + newPos.y);
        }
    }

    //标记是否在播放特效
    public bool isPlayEffects;
    //标记是否要销毁本泡泡
    public bool isBurst;
    //标记是否正在销毁（执行销毁动画,正在执行，则再次检测到不执行）
    public bool isDestroying;

    //是否有关联泡泡:在检测掉落的时候统一置为false，然后从顶部开始遍历，有关联则为true，最后掉落false
    public bool isConnected = false;
    //当掉落的时候，修改,用于检测最低端的 bottom 是不是有效
    public bool isDropIng = false;
    //是否正在被检测跟踪，正在被跟踪则不再被检测
    public bool isTrancingConnected = false;
    /*
     * @brief       播放碰撞效果
     * @desc        发射泡泡周围的泡泡都执行动画，同时周围相同颜色的泡泡也播放动画
     * @desc        在三个地方调用：发射泡泡碰撞其他泡泡，发射泡泡碰撞顶界限，发射泡泡播放特效其周围的也随之播放
     */
    void PlayCollideEffects()
    {
        //只有发射的普通泡泡才可以播放效果
        if (pubbleType >= PubbleColorType.PUBBLE_AIR_TYPE)
            return;
        if (!isPlayEffects)
        {
            //将自身标记为要摧毁
            isBurst = true;
            //记录要摧毁的泡泡数
            PlayLogic.Instance.markNeedBurstAccount++;
            //自身播放缩放动画
            PlayScaleEffect();

            //遍历周边6个泡泡
            for (int i = 0; i < adjacentObjectsNumber; i++)
            {
                //如果该泡泡 与记录的发射泡泡 颜色一致,则该泡泡周边的泡泡也执行动态特效
                if (adjacentPlayObjects[i] != null)
                {
                    if (adjacentPlayObjects[i].pubbleType == PlayLogic.Instance.markShootType)
                    {
                        //执行动态效果
                        adjacentPlayObjects[i].PlayCollideEffects();
                    }
                    else
                    {
                        //如果颜色不一致：则将该泡泡标记为正在动态效果，同时执行动态效果
                        adjacentPlayObjects[i].PlayScaleEffect();
                    }
                }
            }
        }
    }

    /*
     * @brief       播放碰撞之后的缩放效果
     */
    public void PlayScaleEffect()
    {
        //标记为正在播放缩放动画
        isPlayEffects = true;
        iTween.PunchScale(gameObject, new Vector3(.2f, .2f, .2f), 1f);
        //StartCoroutine(sclePubble());
        //sclePubble();
    }

    void sclePubble()
    {
        Vector3 amous = new Vector3(.2f, .2f, .2f);
        float time = 1.0f;

        //iTween.PunchScale(gameObject, Hash("amount", amous, "time",time, "looptype","pingPong"));
        Hashtable parameters = new Hashtable();
        parameters.Add("amount", amous);
        parameters.Add("time", time);
        parameters.Add("looptype", iTween.LoopType.loop);
        //iTween.MoveTo(gameObject, parameters);

        iTween.PunchScale(gameObject, parameters);
    }

    private Hashtable Hash(string p1, Vector3 amous, string p2, float time)
    {
        throw new System.NotImplementedException();
    }



    /*
     * @brief       跟踪泡泡，查看其是否有关联
     * @desc        从顶行泡泡开始检测
     */
    internal void TracePubbleIsConnection()
    {
        if (isTrancingConnected || isBurst)
            //如果正在被跟踪或者标记为要毁灭了，则不再进行跟踪检测
            return;
        //标记为正在跟踪检测
        isTrancingConnected = true;
        //标记为有关联泡泡：从顶行泡泡开始计算
        isConnected = true;
        //遍历周边的6个泡泡,然后进行关联跟踪检测
        for (int i = 0; i < adjacentObjectsNumber; i++)
        {
            //如果存在泡泡
            if (adjacentPlayObjects[i])
            {
                //继续跟踪该泡泡是否有关联
                adjacentPlayObjects[i].TracePubbleIsConnection();
            }
        }
    }

    /*
     * @biref       执行毁灭（分为颜色一样的 进行爆炸效果，  没有连接到一起 的 掉落）
     * @prama       falldown    是否掉落:true 执行掉落,false 执行爆炸
     * @desc        标记为正在销毁
     * @desc        将自己从其他泡泡的周边数组之中移除
     * @desc        将自己的tag设置为未设置
     */
    public void BurstMySelf(bool falldown)
    {
        //如果已经在执行毁灭了，则跳过
        if (isDestroying)
            return;
        //标记正在毁灭
        isDestroying = true;
        //将自身从周边泡泡的 周边数组之中移除
        RemoveMeFromNeighbourAdjacentObjects();
        //修改自身的tag值,防止 发射的新泡泡  错误查找
        gameObject.tag = "Untagged";

        //执行动画的一些效果
        if (falldown)
        {
            FallDownMyPubbleObject();
        }
        else
        {
            //初始化离子
            PlayMyParticlesEffects();
            //弹出分数
            PlayNumberEffect();
            //弹出钻石
            PlayDiamondEffect();
            //销毁物体
            Destroy(gameObject);
        }
    }

    /*
     * @brief       重置本泡泡的记录状态
     * @desc        当发射泡泡撞击了其他泡泡的时候进行检测，不符合毁灭条件，则直接将所有泡泡的标记状态恢复
     */
    public void ResetPubbleState()
    {
        isTrancingConnected = false;
        isPlayEffects = false;
        isBurst = false;
    }

    /*
     * @brief       游戏结束的时候掉落自身
     */
    internal void FallDownMyPubbleObject()
    {
        //修改自己的layer，防止遇到问题
        gameObject.layer = defaultLayer;
        isDropIng = true;
        //销毁自身碰撞器组件 球体碰撞器（在爆破过程中还有泡泡发出，所以先销毁，防止影响）
        Destroy(GetComponent<SphereCollider>());
        //打开重力
        rigidbody.useGravity = true;
        //关闭运动学
        rigidbody.isKinematic = false;
        //添加一个力
        rigidbody.AddForce(new Vector3(0, Random.Range(1.0f, 2.0f), 0), ForceMode.VelocityChange);
        //2.0f后销毁泡泡
        Invoke("DestroyGameObject",2.0f);
    }

    public Transform GetPubbleObjectTransform()
    {
        return gameObject.transform;
    }

    void DestroyGameObject()
    {
        DestroyImmediate(gameObject);
    }


    #region 掉落道具的功能
    /*
     * @brief       如果自己是被撞击泡泡，执行自己泡泡的相关作用（很可能是道具泡泡）
     * @desc        各个子类里面重写，实现自己相应的功能
     * @desc        普通泡泡:不需要重写，不做任何操作
     * @desc        掉落道具泡泡：需要重写，实现自己的功能
     * @desc        发射道具泡泡：不需要重写，压根不会掉落下来
     * 
     * @desc        不重写了，只在此处实现掉落泡泡的功能:原因 子类重载函数，父类调用不到，详细请问 ：王冠卿
     */
    //闪电特效
    public GameObject thunderEffectObject = null;
    //彩虹特效
    public GameObject rainBowEffectObject = null;
    /*
    public void DropPropExecuteFunction()
    {
        //如果是闪电泡泡，则将特效居中
        if (thunderEffectObject != null)
        {
            
            //执行闪电泡泡 功能,方法在 其子函数内
            //(gameObject.GetComponent<PubbleObject>() as DropPropPubbleObject).MyCollidedExecuteFunction();
            //(this as DropPropPubbleObject).ExefunctionDropProp();
            //exeFuctionDropDelegate();
            DropPropExecuteFunction();
        }
        else if (rainBowEffectObject != null)
        {
            
            //执行闪电泡泡 功能,方法在 其子函数内
            //(gameObject.GetComponent<PubbleObject>() as DropPropPubbleObject).MyCollidedExecuteFunction();
            //ExefunctionDropProp();
            //(this as DropPropPubbleObject).ExefunctionDropProp();
        }
    }
     * */

    /*
     * @brief       如果自己是被撞击泡泡，执行自己泡泡的相关作用
     * @desc        执行掉落道具的功能
     */
    public void ExefunctionDropProp()
    {
        switch (pubbleType)
        {
            case PubbleColorType.PUBBLE_THUNDER_TYPE:
                //如果是闪电泡泡
                PlayThunderEffect();
                ExecuteThunderPubbleFunction();
                break;
            case PubbleColorType.PUBBLE_RAINBOW_TYPE:
                //如果是彩虹泡泡
                StartCoroutine(PlayRainBowEffect());
                break;
            default:
                break;
        }
    }

    void PlayThunderEffect()
    {
        GameObject particlesObj = Instantiate(thunderEffectObject) as GameObject;
        particlesObj.gameObject.transform.parent = gameObject.transform.parent;
        particlesObj.transform.position = new Vector3(0, transform.position.y, transform.position.z);
        particlesObj.transform.localScale = new Vector3(1, 1, 0);
        Destroy(particlesObj, 2.0f);
    }

    IEnumerator PlayRainBowEffect()
    {
        GameObject particlesObj = Instantiate(rainBowEffectObject) as GameObject;
        particlesObj.gameObject.transform.parent = gameObject.transform.parent;
        particlesObj.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        particlesObj.transform.localScale = Vector3.one;
        Destroy(particlesObj, 0.4f);
        yield return new WaitForSeconds(0.2f);
        ExecuteRainbowFunction();
        yield return null;
    }


    /*
     * @brief       执行闪电泡泡的功能：清除一行
     * @desc        先统计该泡泡所在行的所有泡泡：用射线去检测
     * @desc        将统计到的该行泡泡均标记为毁灭，执行毁灭效果，闪电效果记得调节位置
     * @desc        重置所有泡泡状态，检测掉落
     */
    void ExecuteThunderPubbleFunction()
    {
        Vector3 forwardDirection = new Vector3(1, 0, 0);
        Vector3 backDirection = new Vector3(-1, 0, 0);
        RaycastHit[] hitResults = Physics.RaycastAll(transform.position, forwardDirection, 2.0f, pubbleLayerMask);
        RaycastHit[] backHitResults = Physics.RaycastAll(transform.position, backDirection, 2.0f, pubbleLayerMask);

        //统计并设置毁灭标记
        for (int i = 0; i < hitResults.Length; i++)
        {
            GameObject tempHitObject = hitResults[i].collider.gameObject;
            if (tempHitObject.tag == ConstantValue.PlayObjectTag)
            {
                tempHitObject.GetComponent<PubbleObject>().isBurst = true;
            }
        }
        for (int i = 0; i < backHitResults.Length; i++)
        {
            GameObject tempHitObject = backHitResults[i].collider.gameObject;
            if (tempHitObject.tag == ConstantValue.PlayObjectTag)
            {
                tempHitObject.GetComponent<PubbleObject>().isBurst = true;
            }
        }
        //到此，该行泡泡，全部被标记毁灭
        isBurst = true;
        //延后0.2秒，执行毁灭与检测掉落
        Invoke("CheckAllObjectsFallOrBurst", 0.2f);
    }

    /*
     * @brief       如果是彩虹泡泡，执行变色功能：即使有可以消除的也不消除，只是执行变色
     * @desc        统计周边的泡泡
     * @desc        将周边泡泡修改类型，为发射泡泡类型，然后还原所有泡泡的状态
     */
    void ExecuteRainbowFunction()
    {
        //将彩虹泡泡周围，普通泡泡 直接修改类型与显示，其他的道具泡泡则销毁重新创建
        for (int i = 0; i < adjacentPlayObjects.Length; i++)
        {
            if (adjacentPlayObjects[i] != null)
            {
                PubbleObject pubbleScript = adjacentPlayObjects[i];
                if ((int)PubbleColorType.PUBBLE_EMPTY_TYPE < (int)pubbleScript.pubbleType && (int)pubbleScript.pubbleType < (int)PubbleColorType.PUBBLE_AIR_TYPE)
                {
                    //如果是普通的脚本
                    pubbleScript.ChangeNormalPubbleTypeToMarkType();
                }
                else
                {
                    //其他类型，后面再说：现在不搞了：方法 直接销毁重新创建新的
                    pubbleScript.ChangePropPubbleToMarkPubble();
                }
            }
        }
        //修改自己
        ChangePropPubbleToMarkPubble();
    }
    #endregion  掉落道具的功能


    #region 分数的特效
    //特效
    public GameObject numEffectObject = null;

    /*
     * @brief       创建数字特效
     */
    void PlayNumberEffect()
    {
        if (numEffectObject != null)
        {
            GameObject numEffect = Instantiate(numEffectObject) as GameObject;
            numEffect.transform.parent = gameObject.transform.parent;
            numEffect.transform.localScale = Vector3.one;
            numEffect.transform.localPosition = gameObject.transform.localPosition;
            int showNumber = PlayLogic.Instance.GetBurstTimes() * 10;
            numEffect.GetComponent<NumEffectControll>().ChangeShowNumber(showNumber);
            Destroy(numEffect, 1.2f);

            //修改左上角分数
            PlayUIScript.Instance.LerpShowScore(showNumber);
        }
    }

    #endregion 分数的特效

    #region 钻石的特效：当创建所有泡泡的时候，随机几个泡泡可以有钻石奖励，当爆炸的时候弹出特效，并添加
    //默认是不能得到钻石的
    private bool canGetDiamond = false;
    public GameObject diamondEffectObject = null;
    private GameObject myDiamongObject = null;

    public void ChangeDiamongMark()
    {
        canGetDiamond = true;
        if (diamondEffectObject != null)
        {
            //在泡泡上面添加一个钻石,位于中下位置
            myDiamongObject = Instantiate(diamondEffectObject) as GameObject;
            myDiamongObject.transform.parent = gameObject.transform;
            myDiamongObject.transform.localScale = Vector3.one;
            myDiamongObject.transform.localPosition = new Vector3(0, 0, gameObject.transform.localPosition.z);
        }
    }

    void PlayDiamondEffect()
    {
        if (diamondEffectObject != null && canGetDiamond)
        {
            //设置其父节点为：
            myDiamongObject.transform.parent = gameObject.transform.parent;
            myDiamongObject.transform.localScale = Vector3.one;
            myDiamongObject.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y -PlayLogic.Instance.pubbleWidth / 2 + 16, gameObject.transform.localPosition.z);

            
            myDiamongObject.GetComponentInChildren<Animator>().SetBool("HiddenScale", true);
            //修改右上角钻石数
            PlayUIScript.Instance.ChangeDiamondNumber();
            Destroy(myDiamongObject, 0.5f);
        }
    }

    #endregion 钻石的特效：当创建所有泡泡的时候，随机几个泡泡可以有钻石奖励，当爆炸的时候弹出特效，并添加

}
