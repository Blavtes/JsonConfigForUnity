using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * @brief       存放常量
 * @desc        tags
 * @desc        gameObjectNameame
 * @desc        layer
 */

public class ConstantValue
{

    /*
     * @brief       PlayerPrefs 用到的关键字 0: 是玩家第一次进入游戏  1：玩家购买大礼包  2： 购买过大礼包
     */
    public const string FirstTime_Player = "FirstTime_Player";    // 用来判断是否是第一次进入 
    public const string Deblocking_Player = "Deblocking_Player";  // 用来判断玩家是否解锁
    public const string BuySteps_Player = "BuySteps_Player";      // 用来判断玩家是否购买交换步数


    /*
     * @brief       PlayerPrefs 用到的关键字
     */
    public const string sound_player = "sound_player";            // 背景音乐
    public const string video_player = "video_player";            // 音效

    /*
     * @brief       所有技能泡泡的数量
     * @desc        存放到PlayerPrefs 里面
     */
    public const string PubbleNumber_Color = "PubbleNumber_Color";
    public const string PubbleNumber_Light = "PubbleNumber_Light";
    public const string PubbleNumber_Fire  = "PubbleNumber_Fire";
    public const string PubbleNumber_Stone = "PubbleNumber_Stone";
    public const string PubbleNumber_Shock = "PubbleNumber_Shock";
    public const string PubbleNumber_Snow  = "PubbleNumber_Snow";

    public const string CoinNum_Player = "CoinNum_Player";

    public const string PowerNum_Player = "PowerNum_Player";
    public const string TimeStamp_Player = "TimeStamp_Player";

    public const string LoginDayNum_One = "LoginDayNum_One";     //登陆天数 第1天
    public const string LoginDayNum_Two = "LoginDayNum_Two";     //登陆天数 第2天
    public const string LoginDayNum_Three = "LoginDayNum_Three"; //登陆天数 第3天
    public const string LoginDayNum_Four = "LoginDayNum_Four";   //登陆天数 第4天
    public const string LoginDayNum_Five = "LoginDayNum_Five";   //登陆天数 第5天
    public const string LoginDayNum_Six = "LoginDayNum_Six";     //登陆天数 第6天
    public const string LoginDayNum_Seven = "LoginDayNum_Seven"; //登陆天数 第7天
    public const string LoginTimeStamp_Player = "LoginTimeStamp_Player"; // 登陆的时间戳


    /*
     * @brief       四个道具泡泡按钮的名字
     */
    public const string Skill1Button = "Skill1Button";
    public const string Skill2Button = "Skill2Button";
    public const string Skill3Button = "Skill3Button";
    public const string Skill4Button = "Skill4Button";
	
    /*
     * @brief       泡泡使用的tag值
     * @desc        当作为预备泡泡，未与大众泡泡一致时，使用otherstag
     * @desc        大众泡泡的Tag,预备泡泡发射之后设置为大众Tag
     */
    public const string OthersTag = "Others";
    public const string PlayObjectTag = "Play Object";

    /*
     * @brief       Gameobject的name
     */
    //UIRoot的name
    public const string UIRootName = "UI Root";
    //左右边界的name
    public const string LeftMarginName = "Left";
    public const string RightMarginName = "Right";
    //顶部边界的name
    public const string TopLimitName = "TopLimitBox";
    //顶部界限父节点name（texture与boxcollider尺寸不一样，因此加了个父节点）
    public const string TopLimitParenObjectName = "TopLimit";
    public const string FirstPositonObjName = "FirstPositionObject";
    public const string SecondPositionObjName = "SecondPositionObject";
    public const string PlayUITopUI = "TopUI";
    public const string OverLimitObject = "OverLimitObject";
    //第三减速带名字
    public const string ThirdCutSpeedObject = "ThirdCutObject";

    //声音管理的tag
    public const string SoundManagerTag = "SoundManager";
    //充值管理的tag
    public const string RechargeManagerTag = "RechargeManager";

    //逻辑管理的tag
    public const string LogicManagerTag = "LogicManager";
    public const string TuitionName = "TuitionManager(Clone)";
    /*
     * @brief       常规图片的名字
     * @desc        遇见彩虹泡泡即变色泡泡的时候使用
     */
    public const string YellowFile = "pubble_yellow";
    public const string OrangeFile = "pubble_orange";
    public const string CyanFile = "pubble_cyan";
    public const string PurpleFile = "pubble_purple";
    public const string BlueFile = "pubble_blue";
    public const string GreenFile = "pubble_green";
    public const string RedFile = "pubble_red";


    /*
     * @brief       预设的名字，记住是clone出来的
     * @desc        同样在遇到变色泡泡的时候使用
     */
    public const string YellowConeName = "Pubble_Yellow(Clone)";
    public const string OrangeConeName = "Pubble_Orange(Clone)";
    public const string CyanConeName = "Pubble_Orange(Clone)";
    public const string PurpleConeName = "Pubble_Purple(Clone)";
    public const string BlueConeName = "Pubble_Blue(Clone)";
    public const string GreenConeName = "Pubble_Green(Clone)";
    public const string RedConeName = "Pubble_Red(Clone)";
    //气泡的clone名称，当检测是不是碰撞到气泡的时候使用
    public const string AirConeName = "Pubble_AIR(Clone)";
    public const string BackTipName = "BackTipDlg(Clone)";

    public const string TuitionManagerName = "TuitionManager(Clone)";
    //游戏场景名称
    public const string PlaySceneName = "PlayScene";

    /*
     * @brief       所有泡泡（发射之后）的层级
     * @desc        做射线检测用
     */
    public const int PubbleMaskLayer = 8;


    //
    public const string StartSceneName = "StartScene";
    public const string LevelSceneName = "LevelScene";
    //public const string PlaySceneName = "PlayScene";


   /* public struct propDetail
    {
        int index;
        string dex;
    }
    public propDetail[5]  prop = 
    {
    {0;"test"},{1,"test"},{2,"test"},{3;"test"},{4;"test"},{5;"test"}
    }*/

}
