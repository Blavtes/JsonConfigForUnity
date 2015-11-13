
#define EndlishLanguge

using UnityEngine;
using System.Collections;


/*
 * @brief       所有字符串
 */
public class ConstantString {



#if EndlishLanguge 
    public const string TUITION_TIP_CHALLENGE_TXT = "末位数字为5的关卡均为挑战关卡，挑战关卡满星会有意想不到的奖励哦！要在步数用完之前过关哦！";

    public const string TUITION_TIP_LIMIT_TXT = "10的倍数关为极限关卡，会有新的泡泡不断涌出哦！祝你好运！";



    public const string HELP_DES_TXT = "手指点击想要的发射的方向发射泡泡\n至少3个相同泡泡连在一起才能消除哦\n满星过关会有奖励哦\n彩虹泡泡:具有将周围泡泡变成同色泡泡的能力\n闪电泡泡:可以消除整排的泡泡\n火球泡泡:消除爆炸范围内的所有泡泡\n冰冻泡泡:使泡泡无法下降，持续5秒\n穿透泡泡:消除直线上触碰到的泡泡\n冲击泡泡:使泡泡退后";

    public const string ABOUT_DES_TXT = "游戏名称：泡泡去哪儿了\n游戏类型：三消类\n版本号：V1.0\n开发商：北京鑫美网络科技有限公司\n客服电话：010-53638861\n客服邮箱:   xmwlgame@126.com\n客服Q群:   44291759 (欢迎加入)\n免责声明:本游戏版权归属北京鑫美网络科技有限公司所有，游戏中文字、图片等内容为游戏版权所有者个人态度或立场，中国移动对此不承担任何法律责任。";

    public const string LoginTip1 = "金币 * 100";
    public const string LoginTip2 = "火焰泡泡 * 2";
    public const string LoginTip3 = "冲击泡泡 * 2";
    public const string LoginTip4 = "金币 * 300";
    public const string LoginTip5 = "冰冻泡泡 * 2";
    public const string LoginTip6 = "穿透泡泡 * 2";
    public const string LoginTip7 = "金币 * 500";

    public static string[] LevelBottomTitles = { "钻石*300,变色泡泡 *3, 闪电泡泡 *3, 火球泡泡 *3,穿透泡泡 *3,冲击泡泡 *3,冰冻泡泡各 *3", "钻石*6000,变色泡泡 *15, 闪电泡泡 *15, 火球泡泡 *15,穿透泡泡 *15,冲击泡泡 *15,冰冻泡泡各 *15" };

    public static string[] LevelBuyTipContent = {   "钻石*6000 变色泡泡、闪电泡泡、火球泡泡、穿透泡泡、冲击泡泡、冰冻泡泡各15个！机会不容错过！游戏需要痛快！只需12元即可获得！","是否花费300钻换取5个变色泡泡？",  "是否花费300钻换取5个火球泡泡？","是否花费300钻换取5个冰冻泡泡？", "是否花费500钻购买5个闪电泡泡？", "是否花费500钻购买5个穿透泡泡？","是否花费500钻购买5个冲击泡泡？", "进入游戏就可获得福利！只需1元就可获得超值物品! 礼包内含：钻石*300，变色泡泡、闪电泡泡、火球泡泡、穿透泡泡、冲击泡泡、冰冻泡泡各3个！机会不容错过！游戏需要痛快！","是否花费3元开启无限交换功能？"};

    public static string[] LevelPropTips =  { "钻石*6000,变色泡泡 *15, 闪电泡泡 *15, 火球泡泡 *15,穿透泡泡 *15,冲击泡泡 *15,冰冻泡泡各 *15","变色泡泡 *5","火球泡泡 *5","冰冻泡泡 *5","闪电泡泡 *5","穿透泡泡 *5","冲击泡泡 *5"};

    public static string[] LevelPropTitles = { "促销大礼包！便宜到爆！来看看吧！", "具有将周围泡泡变成同色泡泡的能力", "消除爆炸范围内的所有泡泡", "使泡泡无法下降，持续5秒", "可以消除整排的泡泡", "消除直线上触碰到的泡泡", "使泡泡退后" };

    public static string[] LevelCoinTitles = { "立即获得800钻石", "立即获得3000钻石", "立即获得12000钻石"};

    public static string[] LevelCoinTips = { "钻石 *800", "钻石 *3000", "钻石 *12000" };


    public static string[] LevelCostTitle = { "是否花费3元购买800钻石", "是否花费6元购买3000钻石", "是否花费12元购买12000钻石" };

    public static string LevelStartLightDes = "可以消除整排的泡泡";
    public static string LevelStartColorDes = "具有将周围泡泡变成同色泡泡的能力";
    public static string[] LevelStartAllTitles = { "变色泡泡 * 5", "闪电泡泡 * 5" };

#else
    public const string TUITION_TIP_CHALLENGE_TXT = "The last digit of the level 5 are challenging levels, challenging levels full of stars have an unexpected bonus! Watch out! To run a few steps before the pass!";

    public const string TUITION_TIP_LIMIT_TXT = "Multiple of 10 points off the limit, there will be new bubbles constantly pouring Oh! Good luck to you!";



    public const string HELP_DES_TXT = "Click on the direction you want to launch your fingers launch bubbles.\nAt least three identical bubbles together in order to eliminate.\nStar full clearance will reward!\nRainbow Bubble: has the ability to turn into a bubble around the bubbles of the same color.\nLightning Bubble: You can remove an entire row of bubbles.\nBubble fireball: Eliminate all bubbles within the blast radius.\nFrozen Bubble: Make bubble can not be lowered for 5 seconds.\nPenetrate the bubble: the elimination of the straight line touching the bubble.\nBubble Shock: Make bubble back.";

    public const string ABOUT_DES_TXT = "Game Title:Where the bubble\nGenre: three consumer class\nVersion: V1.0\nDeveloper: Beijing Xin Mei Network Technology Co., Ltd.\nCustomer Service Tel: 010-53638861\nCustomer Service Email: xmwlgame@126.com\nCustomer Q group: 44291759 (Welcome)\nDisclaimer: This game belongs to the genus Beijing Xin Mei Network Technology Co., Ltd. all, the game text, images and other \ncontent for the game Copyright individual attitude or position, China Mobile does not assume any legal liability.";

    public const string LoginTip1 = "diamonds *100";
    public const string LoginTip2 = "fireball bubble * 2";
    public const string LoginTip3 = "impact bubble * 2";
    public const string LoginTip4 = "diamonds * 300";
    public const string LoginTip5 = "frozen bubble * 2";
    public const string LoginTip6 = "penetrate bubble * 2";
    public const string LoginTip7 = "diamonds * 500";

    public static string[] LevelBottomTitles = { "diamonds *300,color bubble *3, lightning bubble *3, fireball bubble *3,penetrate bubble *3,impact bubble *3,frozen bubble *3", "diamonds*6000,color bubble *15, lightning bubble *15, fireball bubble *15,penetrate bubble *15,impact bubble *15,frozen bubble *15" };

    public static string[] LevelBuyTipContent = { "diamonds *6000 color bubble、lightning bubble、fireball bubble、penetrate bubble、impact bubble、frozen bubble each 15！ Opportunity not to be missed! The game requires happy! Only 12 yuan you can get!", "Whether spending in exchange for 5 300 diamond color bubble？", "Whether spending in exchange for 5 300 diamond fireball bubble？", "Whether spending in exchange for 5 300 diamond frozen bubble？", "Whether spending in exchange for 5 500 diamond lightning bubble？", "Whether spending in exchange for 5 500 diamond penetrate bubble？", "Whether spending in exchange for 5 500 diamond impact bubble？", "Enter the game you can get benefits! Only 1 yuan can get premium items!Package Includes: 200 diamonds, color bubble, bubble lightning fireball bubble, bubble penetration, impact bubbles, frozen bubbles each two! Opportunity not to be missed! The game requires happy!", "Whether to spend 2 yuan to open unlimited switching function？" };

    public static string[] LevelPropTips = { "diamonds*6000,color bubble *15, lightning bubble *15, fireball bubble *15,penetrate bubble *15,impact bubble *15,frozen bubble各 *15", "color bubble *5", "fireball bubble *5", "frozen bubble *5", "lightning bubble *5", "penetrate bubble *5", "impact bubble *5" };

    public static string[] LevelPropTitles = { "Promotional spree! Cheaper to burst! Check it out!", "Has the ability to turn into a bubble around the bubbles of the same color.", "Eliminate all bubbles within the blast radius.", "So that bubbles can not be lowered for 5 seconds.", "You can eliminate a whole row of bubbles.", "Eliminate bubbles touch the straight line.", "Make bubble back." };

    public static string[] LevelCoinTitles = { "Immediately get 800diamonds", "Immediately get 3000diamonds", "Immediately get 12000diamonds" };

    public static string[] LevelCoinTips = { "diamonds *800", "diamonds *3000", "diamonds *12000" };


    public static string[] LevelCostTitle = { "Whether to spend 3 yuan to buy 800 diamonds?", "Whether to spend 2 yuan to buy 3000 diamonds?", "Whether to spend 12 yuan to buy 12000 diamonds?" };

    public static string LevelStartLightDes = "You can eliminate a whole row of bubbles.";
    public static string LevelStartColorDes = "To turn into a bubble around the bubbles of the same color.";
    public static string[] LevelStartAllTitles = { "color bubble * 5", "lightning bubble * 5" };

#endif

}
