using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

 /// 单例模式的实现

/*
 * @brief       负责玩家所有的信息
 * @Author      King
 * @date        2014-10-13
 * @desc        绑定玩家的的一些必备信息
 */
public class UserInstanse
{
        #region Instance
       // 定义一个静态变量来保存类的实例
        private static UserInstanse userInstance;

        // 定义一个标识确保线程同步
       // private static readonly object locker = new object();

        // 定义私有构造函数，使外界不能创建该类实例
        private UserInstanse()
        {
        }
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        public static UserInstanse GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            //if (userInstance == null)
           // {
              //  lock (locker)
              //  {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (userInstance == null)
                    {
                        Debug.Log("------------------如果类的实例不存在则创建，否则直接返回---------------------");
                        userInstance = new UserInstanse();
                        ResourceManager.SaveLevelInfoToPersistent();
                        userInstance.ReadLevelInfo();
                        userInstance.GetUserInfomation();
                    }
               // }
           // }
             return userInstance;
        }
      #endregion Instance


        #region member

        // 记录所有界面的中文显示 从text中获得
        private Dictionary<string, string> m_DesData = new Dictionary<string, string>();
        public Dictionary<string, string> DesData { get { return m_DesData; } set { m_DesData = value; } }

        // 记录所有关卡的数据 用于关卡选择界面
        private Dictionary<int, LevelInfo> m_LevelData = new Dictionary<int, LevelInfo>();
        public Dictionary<int, LevelInfo> LevelData { get { return m_LevelData; } set { m_LevelData = value; } }


       //当前关卡得星数,默认1 ，当胜利的时候，进行重新赋值，windlg使用并显示，最后保存
        public int currentBarrStarNum = 1;

        //当前关卡得分数,默认1 ，当胜利的时候，进行重新赋值，windlg使用并显示，最后保存
        public int currentBarrScoreNum = 1;
        //玩家的实时钻石数
        public int currentMoneyNumber = 1;

        //玩家的是否开启下一关卡
        public bool isOpenNextLevel = false;


        //玩家是否购买步数
        public int hasBuySteps{ get; set; }

        // 玩家的背景音乐设置
        public bool soundSet { get; set; }

        // 玩家的音效设置
        public bool audioSet { get; set; }

        // 玩家的登陆天数的数组 int [7] 只存三个数字 1：未领  2：应该领取  3：已领 和下面的时间戳判断登陆奖励
        public int[] loginData= new  int[7];

        // 玩家登陆天数的时间戳
        public int daytimeStamp { get; set; }

        // 玩家的体力
        public int powerNum { get; set; }

        // 体力的的时间戳
        public int timeStamp { get; set; }

        //技能泡泡--- 颜色泡泡 ---- 的数量 
        public int colorPubble_Num { get; set; }

        //技能泡泡--- 闪电泡泡 ---- 的数量 
        public int lightPubble_Num { get; set; }

        //技能泡泡--- 火焰泡泡 ---- 的数量 
        public int firePubble_Num { get; set; }

        //技能泡泡--- 穿透泡泡 ---- 的数量 
        public int stonePubble_Num { get; set; }

        //技能泡泡--- 冲击泡泡 ---- 的数量 
        public int stockPubble_Num { get; set; }

        //技能泡泡--- 雪花泡泡 ---- 的数量 
        public int snowPubble_Num { get; set; }

        // 玩家金币的数量
        public int coinNum { get; set; }

        // 选择的关卡
        public int chooseLevel_id { get; set; }

        //记录一共有多少页
        public int pageCount { get; private set; }

        //记录一共多少行:添加该属性是为了 策划的坐标 一共17个，顶行0.5 开始，次行1 开始
        public int rowCount { get; private set; }

        //记录当前关卡信息
        private Dictionary<int, PubblePageInfo> m_BarrData = new Dictionary<int, PubblePageInfo>();
        public Dictionary<int, PubblePageInfo> BubbleData { get { return m_BarrData; } set { m_BarrData = value; } }
        //记录所有的泡泡行信息:不采用分页
        private Dictionary<int, PubbleRowInfo> m_PubbleAllRowInfo = new Dictionary<int, PubbleRowInfo>();
        public Dictionary<int, PubbleRowInfo> PubbleAllRowInfo { get { return m_PubbleAllRowInfo; } set { m_PubbleAllRowInfo = value; } }

        #endregion member

        #region Member Function

        /*
         * @brief       从配置文件中读取所有关卡的数据存到单例中
         */
        public void ReadLevelInfo()
        {
            m_LevelData.Clear();

            object[] array =  ResourceManager.LoadLevelFile().ToArray();
            int rowCount = array.Length;

            for (int i = 0; i < rowCount; ++i)
            {
                LevelInfo data = new LevelInfo((string)array[i]);
                m_LevelData.Add(i, data);
            }
            //读取要显示的中文text
            string[] desInfo = ResourceManager.LoadTxtFile("document/des");
            for (int i = 0; i < desInfo.Length; i++)
            {
                string[] item = desInfo[i].Split('=');
                Debug.Log("中文显示中第" + i + "行的字符分别是" +item[0]+item[1]);
                m_DesData.Add(item[0],item[1]);
            }

            foreach (string key in m_DesData.Keys)
            {
                Debug.Log("Key = {0}"+ key + m_DesData[key]);
            }

        }

        /*
         * @brief       从单例中的数据存放到配置文件中 未完待续
         */
        public void WriteLevelInfoToPersistent()
        {

            // 2. 存放所有的技能泡泡的数据
            ResourceManager.SetUseInfo(ConstantValue.PubbleNumber_Color, colorPubble_Num);
            ResourceManager.SetUseInfo(ConstantValue.PubbleNumber_Light, lightPubble_Num);
            ResourceManager.SetUseInfo(ConstantValue.PubbleNumber_Fire, firePubble_Num);
            ResourceManager.SetUseInfo(ConstantValue.PubbleNumber_Stone, stonePubble_Num);
            ResourceManager.SetUseInfo(ConstantValue.PubbleNumber_Shock, stockPubble_Num);
            ResourceManager.SetUseInfo(ConstantValue.PubbleNumber_Snow, snowPubble_Num);

            // 3. 存放登陆天数 时间戳
            ResourceManager.SetUseInfo(ConstantValue.LoginDayNum_One, loginData[0]);
            ResourceManager.SetUseInfo(ConstantValue.LoginDayNum_Two, loginData[1]);
            ResourceManager.SetUseInfo(ConstantValue.LoginDayNum_Three, loginData[2]);
            ResourceManager.SetUseInfo(ConstantValue.LoginDayNum_Four, loginData[3]);
            ResourceManager.SetUseInfo(ConstantValue.LoginDayNum_Five, loginData[4]);
            ResourceManager.SetUseInfo(ConstantValue.LoginDayNum_Six, loginData[5]);
            ResourceManager.SetUseInfo(ConstantValue.LoginDayNum_Seven, loginData[6]);
            ResourceManager.SetUseInfo(ConstantValue.LoginTimeStamp_Player, daytimeStamp);

            // 4. 存放玩家的一些信息 金币 体力 体力时间戳
            ResourceManager.SetUseInfo(ConstantValue.CoinNum_Player, coinNum);
            ResourceManager.SetUseInfo(ConstantValue.PowerNum_Player, powerNum);
            ResourceManager.SetUseInfo(ConstantValue.TimeStamp_Player, timeStamp);

            // 5. 存放玩家的音量 音效
            if (audioSet)
                ResourceManager.SetUseInfo(ConstantValue.video_player, 1);
            else
                ResourceManager.SetUseInfo(ConstantValue.video_player, 0);

            if (soundSet)
                ResourceManager.SetUseInfo(ConstantValue.sound_player, 1);
            else
                ResourceManager.SetUseInfo(ConstantValue.sound_player, 0);

            // 6. 是否购买步数
            ResourceManager.SetUseInfo(ConstantValue.BuySteps_Player, hasBuySteps);

            // 1. 存放所有的关卡数据
            ResourceManager.ChangeLevelFile(m_LevelData);
        }

        /*
         * @brief       从PlayerPrefs 中获取用户的数据 各种技能泡泡的信息 以及金币的信息
         */
        public void GetUserInfomation()
        {

            loginData[0] = ResourceManager.GetUserInfo(ConstantValue.LoginDayNum_One);
            loginData[1] = ResourceManager.GetUserInfo(ConstantValue.LoginDayNum_Two);
            loginData[2] = ResourceManager.GetUserInfo(ConstantValue.LoginDayNum_Three);
            loginData[3] = ResourceManager.GetUserInfo(ConstantValue.LoginDayNum_Four);
            loginData[4] = ResourceManager.GetUserInfo(ConstantValue.LoginDayNum_Five);
            loginData[5] = ResourceManager.GetUserInfo(ConstantValue.LoginDayNum_Six);
            loginData[6] = ResourceManager.GetUserInfo(ConstantValue.LoginDayNum_Seven);

            daytimeStamp = ResourceManager.GetUserInfo(ConstantValue.LoginTimeStamp_Player);

            colorPubble_Num = ResourceManager.GetUserInfo(ConstantValue.PubbleNumber_Color);
            lightPubble_Num = ResourceManager.GetUserInfo(ConstantValue.PubbleNumber_Light);
            firePubble_Num  = ResourceManager.GetUserInfo(ConstantValue.PubbleNumber_Fire);
            stonePubble_Num = ResourceManager.GetUserInfo(ConstantValue.PubbleNumber_Stone);
            stockPubble_Num = ResourceManager.GetUserInfo(ConstantValue.PubbleNumber_Shock);
            snowPubble_Num  = ResourceManager.GetUserInfo(ConstantValue.PubbleNumber_Snow);

            coinNum = ResourceManager.GetUserInfo(ConstantValue.CoinNum_Player);
            // 玩家是否购买步数 
            hasBuySteps = ResourceManager.GetUserInfo(ConstantValue.BuySteps_Player);

            //测试使用
            /* colorPubble_Num = 0;
             lightPubble_Num = 0;
             firePubble_Num =0;
             stonePubble_Num = 0;
             stockPubble_Num = 0;
             snowPubble_Num = 0;
             coinNum = 0;
             hasBuySteps = 0;*/

            powerNum = ResourceManager.GetUserInfo(ConstantValue.PowerNum_Player);
            timeStamp = ResourceManager.GetUserInfo(ConstantValue.TimeStamp_Player);

            // 游戏开始  默认打开音乐 音效
            int time = PlayerPrefs.GetInt(ConstantValue.FirstTime_Player, 0);
            if (time == 0)
            {
                audioSet = true;
                soundSet = true;

                colorPubble_Num = 5;
                lightPubble_Num = 5;
                firePubble_Num = 5;
                stonePubble_Num = 5;
                stockPubble_Num = 5;
                snowPubble_Num = 5;

                //表示不再是第一次开启
                PlayerPrefs.SetInt(ConstantValue.FirstTime_Player, 1);
            }
            else
            {
                audioSet = ResourceManager.GetUserInfo(ConstantValue.video_player) == 1;
                soundSet = ResourceManager.GetUserInfo(ConstantValue.sound_player) == 1;
            }
   
            for(int i = 0 ; i < loginData.Length; ++i)
            {
                Debug.Log("登陆奖励的天数是第：" + i + "天-------" + loginData[i]);
            }

            Debug.Log("技能泡泡 ------colorPubble_Num-----" + colorPubble_Num);
            Debug.Log("技能泡泡 ------lightPubble_Num-----" + lightPubble_Num);
            Debug.Log("技能泡泡 ------firePubble_Num-----" + firePubble_Num);
            Debug.Log("技能泡泡 ------stonePubble_Num-----" + stonePubble_Num);
            Debug.Log("技能泡泡 ------stockPubble_Num-----" + stockPubble_Num);
            Debug.Log("技能泡泡 ------snowPubble_Num-----" + snowPubble_Num);

            Debug.Log("玩家拥有的货币-----" + coinNum);
            Debug.Log("玩家拥有的体力值-----" + powerNum);

            Debug.Log("玩家的背景音乐-----" + soundSet);
            Debug.Log("玩家的音效-----" + audioSet);
        }

        /*
         * @brief       从配置文件中读取关卡信息
         * @prama       isLight   是否有闪电技能泡泡
         * @prama       isColor   是否有颜色技能泡泡
         */
        public void ReadBarrInfo(bool isColor, bool isLight )
        {
            m_PubbleAllRowInfo.Clear();

            Debug.Log("选中的关卡： ------- " + chooseLevel_id + "从配置文件中读取关卡信息");
            string[] barrInfo = ResourceManager.LoadTxtFile("document/barr" + chooseLevel_id);

            rowCount = barrInfo.Length;
            //配置文件中从下往上显示,因此读取文件也从下往上
            for (int i = barrInfo.Length - 1; i >= 0; i--)
            {
                //Debug.Log("读取文件也从下往上 ~~~~~" + barrInfo[i]);
                string[] itemRowList = barrInfo[i].Split(';');

                PubbleRowInfo data = new PubbleRowInfo(itemRowList);
                m_PubbleAllRowInfo.Add(barrInfo.Length - i - 1, data);
            }
            //记录下当前的总页码数
            pageCount = m_BarrData.Count;


            // Debug.Log("闪电球的行数位置-----" + isLight + isColor);

           /* for (int i = 0; i < m_PubbleAllRowInfo.Count; i++)
            {
                Debug.Log("所在行的行数：" + i + " 所在行的泡泡的数量" + m_PubbleAllRowInfo[i].PubbleRowDic[8].ActionType);
            }*/

                // 如果包含闪电球 则设置配置文件中读取的数据
            if (isLight)
            {
                int[] randNum = ResourceManager.GetNoRepeatRandNumber(barrInfo.Length);
                foreach (int a in randNum)
                {
                    Debug.Log("闪电球的行数位置-----" + a);
                }
                foreach (int cur in randNum)
                {
                    //获取随机生成的第n行的泡泡数据
                    Dictionary<int, PubbleInfo> dict = m_PubbleAllRowInfo[cur].PubbleRowDic;
                    int random = Random.Range(0, dict[8].ActionType-1);
                    Debug.Log("闪电球的位置-----行：" + cur + "random：" + random);
                    int coutTag = 0;
                    for (int i = 0; i < dict.Count; i++)
                    {
                        //Debug.Log("pubble的位置"+ i+ "----pubble的 actiontype值是：----" + dict[i].ActionType);
                        //第0个位置有泡泡 并且该位置被随机成闪电泡泡
                        if (i == 0)
                        {
                            if (random == 0 && dict[0].ActionType == 1)
                            {
                                PubbleInfo pubble = dict[0];
                                pubble.ColorType = PubbleColorType.PUBBLE_THUNDER_TYPE;
                            }
                            if (dict[0].ActionType == 1)
                            {
                                coutTag++;
                            }
                        }
                        else 
                        {
                            //Debug.Log("pubble的位置 else --------" + i + "----pubble的 actiontype值是：----" + dict[i].ActionType);
                            if ((dict[i].ActionType - dict[i - 1].ActionType) == 1)
                            {
                                if (coutTag == random)
                                {
                                       
                                    PubbleInfo pubble = dict[i];
                                    pubble.ColorType = PubbleColorType.PUBBLE_THUNDER_TYPE;
                                }
                                coutTag++;
                            }
                        }
                    }
                }
            }

            // 如果包含颜色球 则设置配置文件中读取的数据
            if (isColor)
            {
                int[] randNum = ResourceManager.GetNoRepeatRandNumber(barrInfo.Length-2);
                foreach (int a in randNum)
                {
                    Debug.Log("颜色球的行数位置-----" + a);
                }
                foreach (int cur in randNum)
                {
                    //获取随机生成的第n行的泡泡数据
                    Dictionary<int, PubbleInfo> dict = m_PubbleAllRowInfo[cur].PubbleRowDic;
                    int random = Random.Range(0, dict[8].ActionType - 1);
                    Debug.Log("颜色球的位置-----行：" + cur + "random：" + random);
                    int coutTag = 0;
                    for (int i = 0; i < dict.Count; i++)
                    {
                        if (i == 0)
                        {
                            if (random == 0 && dict[0].ActionType == 1)
                            {
                                PubbleInfo pubble = dict[0];
                                pubble.ColorType = PubbleColorType.PUBBLE_RAINBOW_TYPE;
                            }
                            if (dict[0].ActionType == 1)
                            {
                                coutTag++;
                            }
                        }
                        else
                        {
                            //Debug.Log("pubble的位置 else --------" + i + "----pubble的 actiontype值是：----" + dict[i].ActionType);
                            if ((dict[i].ActionType - dict[i - 1].ActionType) == 1)
                            {
                                if (coutTag == random)
                                {

                                    PubbleInfo pubble = dict[i];
                                    pubble.ColorType = PubbleColorType.PUBBLE_RAINBOW_TYPE;
                                }
                                coutTag++;
                            }
                        }
                    }
                }
            }
        }

        /*
         * @brief      随机生成两行泡泡
         */
        static public Dictionary<int, PubbleRowInfo> GetRandomInfo()
        {
            Dictionary<int, PubbleRowInfo> m_PubbleAllRowInfo = new Dictionary<int, PubbleRowInfo>();
            string[] barrInfo = ResourceManager.LoadTxtFile("document/barr901");
            int rowCount = barrInfo.Length;
            int random = Random.Range(0, rowCount - 1);

            Debug.Log("读取随机文件-----文件也从下往上 ~~~~~" + barrInfo[random]);
            string[] itemRowList = barrInfo[random].Split(';');
            PubbleRowInfo data = new PubbleRowInfo(itemRowList);
            m_PubbleAllRowInfo.Add(0, data);
            return m_PubbleAllRowInfo;
        }

        /*
         * @brief       过关胜利 开启下一关卡 
         * @prama       star    过关胜利的关卡获得的星数
         * @prama       score   过关胜利的关卡获得的分数
         */
        public void SuccessSaveCurrentLevel(int star, int score, int coin)
        {
            // 存放当前关卡的胜利信息： 星数 分数 以及玩家当前的金币数
            currentBarrScoreNum = score;
            currentBarrStarNum = star;
            coinNum = coin;


            LevelInfo data  =  m_LevelData[chooseLevel_id];
            data.score = score > data.score ? score : data.score;
            data.star = star > data.star ? star : data.star;

            m_LevelData.Remove(chooseLevel_id);
            m_LevelData.Add(chooseLevel_id, data);

            Debug.Log("这一次胜利的关卡是" + chooseLevel_id + "关卡所得的分数：" + m_LevelData[chooseLevel_id].score + "关卡所得的星数" + m_LevelData[chooseLevel_id].star);

            if (chooseLevel_id < 59)
            {
                LevelInfo nextdata = m_LevelData[chooseLevel_id + 1];
                nextdata.open = 1;

                m_LevelData.Remove(chooseLevel_id + 1);
                m_LevelData.Add(chooseLevel_id + 1, nextdata);
                Debug.Log("下一关卡是" + chooseLevel_id + 1 + "关卡是否开启---：" + m_LevelData[chooseLevel_id + 1].open);
            }
        }

        /*
            * @brief       过关失败 保存玩家信息 
            * @prama       score   过关胜利的关卡获得的分数
            */
        public void FialSaveCurrentLevel(int score, int coin)
        {
            // 存放当前关卡的胜利信息： 星数 分数 以及玩家当前的金币数
            currentBarrScoreNum = score;
            coinNum = coin;

            LevelInfo data = m_LevelData[chooseLevel_id];
            data.score = score > data.score ? score : data.score;

            m_LevelData.Remove(chooseLevel_id);
            m_LevelData.Add(chooseLevel_id, data);

            Debug.Log("这一次胜利的关卡是" + chooseLevel_id + "关卡所得的分数：" + m_LevelData[chooseLevel_id].score + "关卡所得的星数" + m_LevelData[chooseLevel_id].star);

        }

        /*
         * @brief       游戏界面 技能泡泡的数量保存
         * @prama       stonePubble   穿透泡泡
         * @prama       firePubble    火焰泡泡
         * @prama       stockPubble   冲击泡泡
         * @prama       stockPubble   雪花泡泡
         */
        public void savePubbleNum(int stonePubble, int firePubble, int stockPubble, int snowPubble)
        {
            stonePubble_Num = stonePubble;
            firePubble_Num  = firePubble;
            stockPubble_Num = stockPubble;
            snowPubble_Num  = snowPubble;
        }

        /*
         *  @brief       返回当前关卡的信息 levelInfo
         */
        public LevelInfo ReturnLevelInfo()
        {
            LevelInfo info = m_LevelData[chooseLevel_id];
            return info;
        }

      
    #endregion Member Function
}
