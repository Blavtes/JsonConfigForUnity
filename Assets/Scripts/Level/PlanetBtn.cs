using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责选关界面关卡点击事件响应
 * @Author      king
 * @date        2014-10-9
 * @desc        选关界面某一关卡 点击事件响应
 */

public class PlanetBtn : MonoBehaviour
{

    public GameObject startPanel;

    void Awake()
    {
        string name = this.name;
        GameObject button = GameObject.Find(name);
        UIEventListener.Get(button).onClick = ButtonClick;
    }
    /*
     * @brief   第三种方式获取按钮的点击事件
     */
    public void ButtonClick(GameObject button)
    {


        //扣除Item 四个字符 剩下的为本关卡的level_id 
        int level_id = int.Parse(button.name.Substring(4));       

        // 索引到该关卡的信息info
        LevelInfo info = UserInstanse.GetInstance().LevelData[level_id];

        
        // 如果是开启的关卡 则可以点击
        if (info.open == 1 )
        {
            Debug.Log("scrollview  item name :" + button.name + "可以点击，读取此关卡的配置信息");

            SoundManager.Instance.PlayButtonTouchSound();
            // 1. 设置选择的关卡id
            UserInstanse.GetInstance().chooseLevel_id = level_id;
    
            // 2. 下一界面的初始化
            GameObject cur = Instantiate(startPanel) as GameObject;
            GameObject root = GameObject.Find("UI Root");
            cur.gameObject.transform.parent = root.gameObject.transform;
            cur.transform.localScale = new Vector3(1, 1, 1);

        }

       // else
       // {
       //     Debug.Log("scrollview  item name :" + button.name + "不可以点击，原因是没有开启");
      //  }
    }

}
