using UnityEngine;
using System.Collections;

/*
 * @brief       用于记录普通泡泡的状态，用于设置sprite的Depth
 * @desc        默认是NormalState
 * @desc        NORMAl:默认值，此时不会执行任何操作
 * @desc        FIRST:此时会查看sprite的Depth以修改显示
 * @desc        SECOND:此时会修改sprite的Depth恢复原来
 */
public enum PreParePubbleState
{
    PREPARE_NORMAL_STATE = 0,
    PREPARE_FIRST_STATE = 1,
    PREPARE_SECOND_STATE = 2
};

/*
 * @brief       绑定在普通的泡泡上
 * @desc        10.09代码重构
 * @Author      King
 */
public class ColorPubbleObject : PubbleObject {
    //预备泡泡显示出来的depth度
    int showDepthValue = 16;
    int normalDepthValue = 1;
    PreParePubbleState preparePubbleState = PreParePubbleState.PREPARE_NORMAL_STATE;
    #region public Function

    /*
    * @brief       现在泡泡 用Sprite显示，在创建预备泡泡的时候便于显示提高depth，成为发射泡泡的时候，修改回原来的depth
    */
    public void SetShowPubbleDepth()
    {
        preparePubbleState = PreParePubbleState.PREPARE_FIRST_STATE;
        StartCoroutine(ExecuteCheckPubbleDepth());
    }

    public void SetOrginPubbleDepth()
    {
        preparePubbleState = PreParePubbleState.PREPARE_SECOND_STATE;
    }

    IEnumerator ExecuteCheckPubbleDepth()
    {
        //如果当该泡泡预备状态不是normal的时候，开始检测深度
        while (preparePubbleState != PreParePubbleState.PREPARE_NORMAL_STATE)
        {
            if (preparePubbleState == PreParePubbleState.PREPARE_FIRST_STATE)
            {
                gameObject.GetComponent<UISprite>().depth = showDepthValue;
            }
            else
            {
                gameObject.GetComponent<UISprite>().depth = normalDepthValue;
                //修改为普通状态则不再进行该操作
                preparePubbleState = PreParePubbleState.PREPARE_NORMAL_STATE;
            }
            //等待0.5s
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }


    /*
     * @brief       如果自己是发射泡泡泡，执行发射泡泡的相关作用
     * @desc        继承自父类，当前为普通泡泡，则做毁灭以及掉落检测
     */
    override public void MyEmissionExecuteFunction()
    {
        //延后0.2秒，为播放缩放动画预留时间。检测是否掉落,并将相同颜色的毁灭
        Invoke("CheckAllObjectsFallOrBurst", 0.2f);
    }


    /*
     * @brief       触发器
     */
    /*
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTrigger is 气泡 is :" + other.gameObject.name);
    }
    */
    #endregion public Function
}

