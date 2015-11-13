using UnityEngine;
using System.Collections;
using System.Text;

/*
 * @brief       当前的职责是负责帮助界面
 * @Author      king
 * @date        2014-11-5
 * @desc        帮助界面的事件相应
 */

public class HelpPanel : MonoBehaviour
{

    public GameObject returnBtn;
    public GameObject LevelDesLabel;

    void Awake()
    {
        UIEventListener.Get(returnBtn).onClick = ReturnBtnClick;

        LevelDesLabel.GetComponent<UILabel>().text = ConstantString.HELP_DES_TXT;
    }


    public static string get_uft8(string unicodeString)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        byte[] encodedBytes = utf8.GetBytes(unicodeString);
        string decodedString = utf8.GetString(encodedBytes);
        return decodedString;
    }
    public static string get_gb2312(string unicodeString)
    {
        Encoding gb2312 = Encoding.GetEncoding("gb2312");
        byte[] encodedBytes = gb2312.GetBytes(unicodeString);
        string decodedString = gb2312.GetString(encodedBytes);
        return decodedString;
    }

    //UnicodeEncoding//

    // 返回按钮的点击事件响应
    void ReturnBtnClick(GameObject button)
    {
        Debug.Log("点击返回按钮 -- 返回按钮的名称 :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        Application.LoadLevel("StartScene");
    }
}
