using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;


public class ResourceManager {

    /*
     * @brief       从Resources文件夹下读取内容：一定是在Resources目录下
     * @prama       fileName    文件名（不带类型.txt ....）
     * @desc        如果Resources下还有文件夹则 "folder/filename" (没有后缀名)
     */
    public static string[] LoadTxtFile(string fileName)
    {
        string[] allString = null;
        TextAsset textAset = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
        Debug.Log("Debug.Log  :" + textAset.text);
        if (textAset != null)
        {
            //'\n','\r' 换行，回车
            allString = textAset.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            //释放内存
            Resources.UnloadAsset(textAset);
        }
        else
        {
            Debug.LogWarning("read error is file :" + fileName);
        }
        return allString;
    }

    public static void SaveStringToFile(string filename, string content)
    {
#if UNITY_WP8
        //做存储
#else
//        File.WriteAllText(Application.dataPath + filename, content);
#endif
        
    }

    /*
     * @brief       在可读写的路径下创建txt文件 并复制关卡的资源进去
     */
    public static void SaveLevelInfoToPersistent()
    {
       // PlayerPrefs.SetInt(ConstantValue.FirstTime_Player, 0);
        int time = PlayerPrefs.GetInt(ConstantValue.FirstTime_Player, 0);
        if (time != 0) return;  


        string pPath = Application.persistentDataPath + "/level.txt";
        Debug.Log("文件原生的读入数据txt Application.persistentDataPath----------------level.txt" + pPath);

        string[] info = ResourceManager.LoadTxtFile("document/level");

        //文件流信息
        StreamWriter sw;
        FileInfo t = new FileInfo(pPath);
        if (!t.Exists)
        {
            //如果此文件不存在则创建
            sw = t.CreateText();
        }
        else
        {
            //如果此文件存在则打开
            sw = t.AppendText();
        }
        for (int i = 0; i < info.Length; ++i)
        {
            //以行的形式写入信息
            sw.WriteLine(info[i]);
        }

        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();

        
    }

    /*
     *  读取Application.persistentDataPath + "/level.txt";  返回arrayList 
     */
    public static  ArrayList LoadLevelFile()
    {
        //使用流的形式读取
        StreamReader sr = null;

        string path = Application.persistentDataPath + "/level.txt";

        sr = File.OpenText(path);

        string line;
        ArrayList arrlist = new ArrayList();
        while ((line = sr.ReadLine()) != null)
        {
            //一行一行的读取
            //将每一行的内容存入数组链表容器中
            arrlist.Add(line);
        }
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
        //将数组链表容器返回
        return arrlist;
    }

    /*
    *  修改关卡配置文件 重新生成  未完待续
    */
    public static void ChangeLevelFile( Dictionary<int, LevelInfo> LevelData)
    {

        string path = Application.persistentDataPath + "/level.txt";

        //删除文件 level.txt
        File.Delete(path);

        //使用流的形式读取
        StreamWriter sr = null;
        FileInfo t = new FileInfo(path);
        if (!t.Exists)
        {
            //如果此文件不存在则创建
            sr = t.CreateText();
        }

        for (int i = 0; i < LevelData.Count; ++i)
        {
            LevelInfo info = LevelData[i];
            //以行的形式写入信息
            string line = info.level_id.ToString() + "," + info.open.ToString() + "," + info.type.ToString() + "," + info.star.ToString() + "," + info.score.ToString() + "," + info.step.ToString() + "," + info.star1.ToString() + "," + info.star2.ToString() + "," + info.star3.ToString() + "," + info.reward1.ToString() + "," + info.reward2.ToString() + "," + info.reward3.ToString()+ "," + info.getReward.ToString();
            Debug.Log("重新写入的字符串是：" + line);
            sr.WriteLine(line);
        }
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
    }

    /*-----------------------string key： 玩家 某种信息的 key  psender ：数量-------- ---------------*/
    static public int GetUserInfo(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }
    static public void SetUseInfo(string key, int pSender)
    {
        PlayerPrefs.SetInt(key, pSender);
    }

    /*----------------------------公共方法 unix的时间戳与System.DateTime 之间的转换-------------------------------------*/
    /*
    * @brief        时间转换   unix的时间戳转换成System.DateTime
    * @param       timeStamp   表示unix的时间戳
    */
#if !UNITY_WP8
    public static System.DateTime ConvertUnixToDataTime(int timeStamp)
    {
        System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp.ToString() + "0000000");
        System.TimeSpan toNow = new System.TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
#endif

    /*
     * @brief        时间转换   System.Date转换成Timeunix的时间戳
     */
    public static int GetUnixTimeStamp()
    {
        System.DateTime timenow = System.DateTime.Now;
        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(timenow - startTime).TotalSeconds;
    }



    /*
    * @brief        随机数的生成 生成 0--到len 的4个随机数  待修改
    */
    static public int[] GetNoRepeatRandNumber(int len)
    {
        int[] RandomNum = new int[4];

        for (int i = 0; i < 4; ++i)
        {
            int cur = Random.Range(0, len);
            RandomNum[i] = cur;
        }
        return RandomNum;

        // 回头再修改
        /*
         Debug.Log("数组的长度是" + RandomNum.Length);
        bool tag = false; // 标记 是否有重复的数字
        int randNum_tag = 0;
        do 
        {
            int cur = Random.Range(0, len);
            tag = false;

            for (int i = 0; i < RandomNum.Length; ++i)
            {
                if (RandomNum[i] == cur) 
                {
                    tag = true;
                }
            }
            if (!tag) 
            {
                RandomNum[randNum_tag] = cur;
                randNum_tag++;
            }
        } while (RandomNum.Length != 4);

        return RandomNum;
         * */
    }


}
