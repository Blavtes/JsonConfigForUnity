using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * @brief       当前负责读取关卡信息
 * @Author      king
 * @date        2014-10-9
 * @desc        利用单例中从配置文件读入的数据 来显示某个关卡的相关信息
 */



public class ItemData : MonoBehaviour
{

    public GameObject levelNum;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    // Use this for initialization
	void Start ()
    {
     
       // GameObject object  = gameObject.get
        string name = gameObject.name;

        //扣除Item 四个字符 剩下的为本关卡的level_id 
        int level_id = int.Parse(name.Substring(4));

        // Debug.Log("绑定在Item上的脚本 start level_id:" + level_id);
        LevelInfo info = UserInstanse.GetInstance().LevelData[level_id];

        //设置游戏的关卡数
        levelNum.GetComponent<UILabel>().text = (level_id + 1).ToString();

       // Debug.Log("item: id" + info.level_id + ",open：" + info.open + "star：" + info.star + "\n");
        if(info.open == 1)
        {
            star1.active = true;
            star2.active = true;
            star3.active = true;
            levelNum.active = true;
            
        }
        if(info.star > 0)
        {
            UISprite sprite = star1.GetComponent<UISprite>();
            sprite.spriteName = "star_light";
        }
        if (info.star > 1)
        {
            UISprite sprite = star2.GetComponent<UISprite>();
            sprite.spriteName = "star_light";
       }
        if (info.star > 2)
        {
            UISprite sprite = star3.GetComponent<UISprite>();
            sprite.spriteName = "star_light";
        }

     }
  
}
