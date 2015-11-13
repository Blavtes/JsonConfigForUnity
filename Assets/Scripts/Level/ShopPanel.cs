using UnityEngine;
using System.Collections;

/*
 * @brief       当前负责商城界面的布局 以及事件相应
 * @Author      king
 * @date        2014-10-9
 * @desc        关于商城界面的布局以及事件相应
 */

public class ShopPanel : MonoBehaviour
{

    public GameObject contentPanel;

     public GameObject propPanel;
     public GameObject coinPanel;

    public GameObject propBtn;
    public GameObject coinBtn;
    public GameObject deleteBtn;

    void Awake()
    {
        UIEventListener.Get(deleteBtn).onClick = deleteBtnClick;
        UIEventListener.Get(propBtn).onClick = propBtnClick;
        UIEventListener.Get(coinBtn).onClick = coinBtnClick;

        propPanel = Instantiate(propPanel) as GameObject;
        propPanel.gameObject.transform.parent = contentPanel.gameObject.transform;
        propPanel.transform.localScale = new Vector3(1, 1, 1);
        propPanel.transform.localPosition = new Vector3(0, 0, 0);

        coinPanel = Instantiate(coinPanel) as GameObject;
        coinPanel.gameObject.transform.parent = contentPanel.gameObject.transform;
        coinPanel.transform.localScale = new Vector3(1, 1, 1);
        coinPanel.transform.localPosition = new Vector3(400, 0, 0);
        coinPanel.active = false;
    }

    // 关闭按钮的点击事件响应
    void deleteBtnClick(GameObject button)
    {
        Debug.Log("点击关闭按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        GameObject.Destroy(gameObject);

    }

    // 道具按钮的点击事件响应
    void propBtnClick(GameObject button)
    {
        Debug.Log("点击道具按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        if (propPanel.active == false)
        {
            propPanel.active = true;
            coinPanel.active = false;
            propBtn.GetComponent<UISprite>().depth = 8;
            coinBtn.GetComponent<UISprite>().depth = 6;
        }

    }
    // 金币按钮的点击事件响应
    void coinBtnClick(GameObject button)
    {
        Debug.Log("点击金币按钮----  button name :" + button.name);
        SoundManager.Instance.PlayButtonTouchSound();
        if (coinPanel.active == false)
        {
            coinPanel.transform.localPosition = new Vector3(0, 0, 0);
            coinPanel.active = true;
            propPanel.active = false;
            propBtn.GetComponent<UISprite>().depth = 6;
            coinBtn.GetComponent<UISprite>().depth = 8;
        }

    }

}
