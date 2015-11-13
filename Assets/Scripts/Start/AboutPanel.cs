using UnityEngine;
using System.Collections;

using System.Text;
/*
 * @brief       当前的职责是负责帮助界面
 * @Author      king
 * @date        2014-11-5
 * @desc        帮助界面的事件相应
 */

public class AboutPanel : MonoBehaviour
{

    public GameObject returnBtn;
    public GameObject LevelDesLabel;

    void Awake()
    {
        UIEventListener.Get(returnBtn).onClick = ReturnBtnClick;
        LevelDesLabel.GetComponent<UILabel>().text = ConstantString.ABOUT_DES_TXT;
    }
    // 返回按钮的点击事件响应
    void ReturnBtnClick(GameObject button)
    {
        Debug.Log("点击返回按钮 -- 返回按钮的名称 :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        Application.LoadLevel("StartScene");
    }
}
