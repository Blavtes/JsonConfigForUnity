using UnityEngine;
using System.Collections;

public enum GameStateType
{ 
    GAME_STATE_PREPARE ,        //游戏准备
    GAME_STATE_PLAYING,         //游戏进行中
    GAME_STATE_OTHER_PAUSE,     //其他情况下暂停
    GAME_STATE_PAUSE ,           //游戏暂停
    GAME_STATE_WIN ,             //游戏胜利
    GAME_STATE_GAMEOVER ,        //游戏结束
};


/*
 * @brief       负责游戏的暂停，gameOver，以及下一关卡等
 */
public class GameManager : MonoBehaviour
{

    private static GameManager m_Instance = null;
    public static GameManager Instance { get { return m_Instance; } }
    //记录游戏状态
    //默认是准备状态下，这时候不可以发射，泡泡从底部移动到顶部
    GameStateType gameCurrentState = GameStateType.GAME_STATE_PREPARE;

    //失败页面
    public GameObject loseDlgGameObject ;
    //胜利页面
    public GameObject winDlgGameObject ;
    //购买页面
    public GameObject buyStepDlgGameObject;

    //持有UIroot 跟节点，用于承载胜利，成功页面
    public GameObject uiRootGameObject = null;

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        
    }

    #region 负责游戏失败处理，胜利处理，购买步数处理
    /*
     * @brief       当游戏结束的时候执行函数
     */
    public IEnumerator GameOverFunction()
    {
        //保存数据
        PlayUIScript.Instance.SaveLoseInfoToUserInstance();
        //修改游戏状态
        gameCurrentState = GameStateType.GAME_STATE_GAMEOVER;
        //所有泡泡，全部自由落体,滑板不再下落
        PlayLogic.Instance.FallDownAllPubble();
        yield return new WaitForSeconds(0.5f);

        Debug.Log("弹出失败的页面------------------");
        //弹出失败的页面
        GameObject loseDlg = Instantiate(loseDlgGameObject) as GameObject;
        loseDlg.transform.parent = uiRootGameObject.transform;
        loseDlg.transform.localScale = new Vector3(1.0f,1f,1f);
    }


    /*
     * @brief       胜利
     * @desc        修改游戏状态
     * @desc        不可再发射泡泡:发射器内部实现
     * @desc        弹出胜利页面
     */
    public IEnumerator GameWinFunction()
    {
        if (popBuyStepObject != null)
        {
            Destroy(popBuyStepObject);
        }
        //保存数据
        PlayUIScript.Instance.SaveInfoToUserInstance();
        gameCurrentState = GameStateType.GAME_STATE_WIN;
        yield return new WaitForSeconds(0.2f);
        //弹出胜利的页面
        GameObject winDlg = Instantiate(winDlgGameObject) as GameObject;
        winDlg.transform.parent = uiRootGameObject.transform;
        winDlg.transform.localScale = new Vector3(1.0f, 1f, 1f);
    }


    /*
     * @brief       弹出购买步数窗口
     * @desc        当步数没有的时候弹出
     */
    //用于检测最后一步打完且胜利，现在弹出两个面板，所以在弹出胜利的时候删除购买
    GameObject popBuyStepObject = null;
    public IEnumerator PopBuyStepDialog()
    {
        ChangeGameStateToOther();
        yield return new WaitForSeconds(0.5f);
        if (gameCurrentState == GameStateType.GAME_STATE_OTHER_PAUSE)
        {
            //弹出购买的页面
            popBuyStepObject = Instantiate(buyStepDlgGameObject) as GameObject;
            popBuyStepObject.transform.parent = uiRootGameObject.transform;
            popBuyStepObject.transform.localScale = new Vector3(1.0f, 1f, 1f);
        }
    }


    #endregion 负责游戏失败处理，胜利处理，购买步数处理


    #region 负责获得修改各个游戏状态
    /*
     * @brief       当前是否为准备状态
     * @desc        当游戏开始的时候展示所有的泡泡用到
     */
    public bool CurrentStateIsPrepare()
    {
        return gameCurrentState == GameStateType.GAME_STATE_PREPARE;
    }

    /*
     * @brief       设置当前游戏状态为playing
     * @desc        展示完所有的泡泡后，调用
     */
    public void SetStateToPlaying()
    {
        gameCurrentState = GameStateType.GAME_STATE_PLAYING;
    }

    /*
     * @brief       当前状态是否为playing状态
     */
    public bool CurrentStateIsPlayingState()
    {
        return gameCurrentState == GameStateType.GAME_STATE_PLAYING;
    }

    public void ChangeGameStateToOther()
    {
        gameCurrentState = GameStateType.GAME_STATE_OTHER_PAUSE;
    }


    #endregion 负责获得修改各个游戏状态

    #region 负责游戏的后台跟切入
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        { 
            //当要切入后台的时候，检测当前timeScale == 0
            if (!SlidePlayPanel.Instance.CurrentStateIsPause() && gameCurrentState == GameStateType.GAME_STATE_PLAYING)
            {
                PlayUIScript.Instance.PauseButtonAction();
            }
        }
    }
    #endregion 负责游戏的后台跟切入
}
