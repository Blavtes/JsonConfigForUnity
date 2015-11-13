using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * @brief       关卡以及泡泡信息
 * @brief       BubbleInfo  单个泡泡信息
 * @brief       BubbleRowInfo 一行泡泡的信息
 * @brief       BubblePageInfo 一页泡泡的信息
 * @Author      wolf
 * @desc        策划以excel给出关卡信息，每次至少创建一页泡泡，防止内存过大，同时防止显示断层
 * @desc        关卡信息在DataManger内读取
 */

/*
 * @brief       泡泡类型：颜色，石头。。。
 */
public enum PubbleColorType
{
    PUBBLE_EMPTY_TYPE = 0,  //没有泡泡,即策划给的空值
    PUBBLE_YELLOW_TYPE = 1, //黄色泡泡
    PUBBLE_ORANGE_TYPE = 2, //橙色
    PUBBLE_CYAN_TYPE = 3,   //青色
    PUBBLE_PURPLE_TYPE = 4, //紫色
    PUBBLE_BLUE_TYPE = 5,   //蓝色
    PUBBLE_GREEN_TYPE = 6,   //绿色
    PUBBLE_RED_TYPE = 7,    //红色
    PUBBLE_AIR_TYPE = 8,    //气泡
    PUBBLE_STONE_TYPE = 9,  //石头
    PUBBLE_THUNDER_TYPE = 10,//闪电泡泡，击中消除一行
    PUBBLE_RAINBOW_TYPE = 11,//彩虹泡泡，即变色，击中，周围的泡泡均变成该颜色
    //道具泡泡
    PUBBLE_CROSS_TYPE = 12,  //可穿透其他泡泡，
    PUBBLE_FIRE_TYPE = 13,   //火泡泡，即炸弹，发射出去后，触碰到其他泡泡爆炸，可消除触碰泡泡之外的3层泡泡距
    PUBBLE_SNOW_TYPE = 14,   //雪泡泡,时间暂停5S，期间挡板不下降，新泡泡不出现，玩家进行的操作不算到关卡规则中
    PUBBLE_CRUSH_TYPE = 15,   //冲击波泡泡,将上挡板往上逆推5层
};

/*
 * @brief       所有泡泡的特效类型
 */
public enum PubbleEffectType
{ 
    PUBBLE_EFFECT_EMPTY_TYPE = 0,       //没有特效，即默认值，不显示特效
    PUBBLE_EFFECT_YELLOW_TYPE = 1,      //黄色泡泡，爆炸效果
    PUBBLE_EFFECT_ORANGE_TYPE = 2,      //橙色
    PUBBLE_EFFECT_CYAN_TYPE = 3,        //青色
    PUBBLE_EFFECT_PURPLE_TYPE = 4,      //紫色
    PUBBLE_EFFECT_BLUE_TYPE = 5,        //蓝色
    PUBBLE_EFFECT_GREEN_TYPE = 6,       //绿色
    PUBBLE_EFFECT_RED_TYPE = 7,         //红色
    PUBBLE_EFFECT_AIR_TYPE = 8,         //空气

};


/*
 * @brief       泡泡行为：每次出现随机变化等
 */
public enum PubbleActionType
{ 
    ACTION_A = 0,
    ACTION_B,
    ACTION_C,
    ACTION_D,
    ACTION_E,
};


/*
 * @brief       每个配置文件中泡泡的信息
 */

public struct PubbleInfos
{
    public PubbleColorType ColorType;
    public PubbleActionType ActionType;

};

public class PubbleInfo
{
    #region Properties
    //泡泡颜色
    public PubbleColorType ColorType { get; set; }
    //暂时用于标记本行有多少个泡泡
    public int ActionType { get; set; }
    #endregion Properties

    #region Public Interface
    public PubbleInfo(string itemString)
    {
       // Debug.Log("如果该点没有数据（即不显示泡泡）" + itemString);
        //如果该点有数据（即显示泡泡）
        if (itemString != "0,0")
        {
            //Debug.Log("如果该点没有数据（即不显示泡泡）" + itemString);
            //Debug.Log("//如果该点有数据（即显示泡泡）" + itemString);
            string[] bubbleStr = itemString.Split(',');

            ColorType = (PubbleColorType)int.Parse(bubbleStr[0]);
            //ActionType = int.Parse(bubbleStr[1]);
        }
        else
        {
            ColorType = PubbleColorType.PUBBLE_EMPTY_TYPE;
        }
    }

    public void LogOutPubbleInfo()
    {
        Debug.Log((int)ColorType + "," + (int)ActionType + "\n");
    }

    #endregion Public Interface
}

/*
 * @brief       每行泡泡的信息
 */
public class PubbleRowInfo
{
    #region Properties
    public int PageIndex { get; private set; }
    #endregion Properties

    #region Member
    private Dictionary<int, PubbleInfo> m_PubbleRowDic = new Dictionary<int, PubbleInfo>();
    public Dictionary<int, PubbleInfo> PubbleRowDic { get { return m_PubbleRowDic; } set { m_PubbleRowDic = value; } }
    #endregion Member

    #region public Interface
    public PubbleRowInfo(string[] rowString)
    {
        //最后一个数据代表pageIndex
        PageIndex = int.Parse(rowString[rowString.Length - 1]);

        // 用于统计本行有多少个泡泡
        int cout = 0; 
        //本行所有的pubble信息
        for (int i = 0; i < rowString.Length - 1; i++)
        { 
            //获取出每个数据
            string bubbleString = rowString[i];

            //创建该泡泡数据
            PubbleInfo pubbleInfo = new PubbleInfo(bubbleString);

            if (bubbleString != "0,0") cout += 1;

            pubbleInfo.ActionType = cout;

            //添加到字典
            m_PubbleRowDic.Add(i, pubbleInfo);
            //pubbleInfo.LogOutPubbleInfo();

        }
    }

    public void LogOutRowInfo()
    {
        for (int i = 0; i < m_PubbleRowDic.Count; i++)
        {
            m_PubbleRowDic[i].LogOutPubbleInfo();
        }
    }
    #endregion public Interface
}

/*
 * @brief       每页泡泡的信息
 */
public class PubblePageInfo
{
    #region Member
    public Dictionary<int, PubbleRowInfo> m_PubblePageDic = new Dictionary<int, PubbleRowInfo>();
    public Dictionary<int, PubbleRowInfo> PubblePageDic { get { return m_PubblePageDic; } set { m_PubblePageDic = value; } }
    #endregion Member


    #region public Interface
    public void PubblePageAddRowInfo(PubbleRowInfo rowInfo,int i)
    {
        m_PubblePageDic.Add(i,rowInfo);
    }
    #endregion public Interface
}

/*
 * @brief       每个关卡的信息
 */

public class LevelInfo
{

    #region Properties
    //关卡的id
    public int level_id { get;  set; }

    //关卡是否打开 0:未打开 1:打开
    public int open { get;  set; }


    //关卡类型 1：普通 2：挑战 3： 极限
    public int type { get; set; }


    //关卡获得的星数
    public int star { get;  set; }

    //关卡获得的分数
    public int score { get; set; }

    //关卡挑战模式下 获得的步数
    public int step { get; set; }


    //关卡星级 条件 
    public int star1 { get; set; }
    public int star2 { get; set; }
    public int star3 { get; set; }

    //关卡奖励  
    public int reward1 { get; set; }
    public int reward2 { get; set; }
    public int reward3 { get; set; }

    //是否领取该奖励
    public int getReward { get; set; }


    #endregion Properties


    #region public Interface
    public LevelInfo(string itemString)
    {
        //如果该点有数据（即显示泡泡）
        if (itemString != "")
        {
            string[] item = itemString.Split(',');


            //Debug.Log("item 的长度是------"  + item.Length+ "\n");

            //策划从0开始计算的
            level_id = int.Parse(item[0]);
            open = int.Parse(item[1]);

            type = int.Parse(item[2]);
               
            star = int.Parse(item[3]);
            score = int.Parse(item[4]);

            step = int.Parse(item[5]);

            star1 = int.Parse(item[6]);
            star2 = int.Parse(item[7]);
            star3 = int.Parse(item[8]);

            reward1 = int.Parse(item[9]);
            reward2 = int.Parse(item[10]);
            reward3 = int.Parse(item[11]);

            getReward = int.Parse(item[12]);

        }
        else
        {
            Debug.Log( "关卡配置文件出错" + "\n");
        }

        // 打印关卡的配置信息
        //LogOutLevelInfo();
    }

    public void LogOutLevelInfo()
    {
        Debug.Log("关卡配置: id" + level_id + ",是否打开：" + open + "关卡类型：" + type + "获得的星：" + star + "获得的分数：" + score + "关卡step：" + step + "\n");
    }
    #endregion public Interface
}
