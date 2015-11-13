using UnityEngine;
using System.Collections;

/*
 * @brief       管理屏幕内的点击事件
 * @desc        主要是负责touchbeigin，move，end
 * @desc        该脚本绑定在TouchPanel,用的是NGUI的eventLister 事件监听
 * @desc        compent->NGUI->interal->Event Lister
 */
public class TouchManager : MonoBehaviour
{
    #region Unity OverLoad

    static TouchManager m_Instance = null;
    public static TouchManager Instance { get { return m_Instance; } }

    void Start()
    {
        m_Instance = this;
        UIEventListener listener = UIEventListener.Get(gameObject);
        listener.onDrag += PlayerDrag;
        listener.onDragOut += PlayerDragOut;
        listener.onClick += PlayerClick;
    }


    //拖拽完之后，不会执行Click函数
    void PlayerDrag(GameObject gameobject, Vector2 delta)
    {
        //只有playing状态下才可以 拖拽
        if (!GameManager.Instance.CurrentStateIsPlayingState()) return;
        Vector3 touchPosition = Input.mousePosition;
        RotateFish(touchPosition);
        Debug.Log("my is PlayerDrag ");
    }

    //拖拽结束
    void PlayerDragOut(GameObject go)
    {
        //只有playing状态下才可以 拖拽
        if (!GameManager.Instance.CurrentStateIsPlayingState()) return;
        Vector3 touchPosition = Input.mousePosition;
        RotateFish(touchPosition,true);
        Debug.Log("my is ovver is");
    }

    public void PlayerClick(GameObject gameobject)
    {
        Debug.Log("my is ovver PlayerClick");
        if (GameManager.Instance.CurrentStateIsPlayingState())
        {
            Vector3 touchPosition = Input.mousePosition;
            RotateFish(touchPosition,true);
        }
        else if (GameManager.Instance.CurrentStateIsPrepare())
        {
            //如果是准备状态，点击之后，直接play
            //所有泡泡划上去，同时修改状态

        }
    }

    public bool CanPlayShoot()
    {
        if (GameManager.Instance.CurrentStateIsPlayingState())
        {
            Vector3 touchPosition = Input.mousePosition;
            RotateFish(touchPosition, true);
            return true;
        }
        else if (GameManager.Instance.CurrentStateIsPrepare())
        {
            //如果是准备状态，点击之后，直接play
            //所有泡泡划上去，同时修改状态

        }
        return false;
    }


    void RotateFish(Vector3 touchPos,bool shoot = false)
    {
        StartCoroutine(PlayUIScript.Instance.RotateFishGameObject(touchPos,shoot));
    }
    

    #endregion Unity OverLoad
}
