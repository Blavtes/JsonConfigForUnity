using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责购买技能泡泡提示界面的布局 以及事件相应
 * @Author      king
 * @date        2014-10-20
 * @desc        关于技能泡泡没有时， 弹出的道具购买提示框
 */

public class PropTipPanel : MonoBehaviour
{

    public GameObject powerPanel;

    public GameObject OkBtn;
    public GameObject DeleteBtn;


    void Awake()
    {
        UIEventListener.Get(OkBtn).onClick = okBtnClick;
        UIEventListener.Get(DeleteBtn).onClick = deleteBtnClick;

    }
    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        Debug.Log("点击关闭按钮----  button name :" + button.name);
        GameObject.Destroy(gameObject);

    }

    // 确认按钮的点击事件响应
    void okBtnClick(GameObject button)
    {

       GameObject.Destroy(gameObject);

    }
}

