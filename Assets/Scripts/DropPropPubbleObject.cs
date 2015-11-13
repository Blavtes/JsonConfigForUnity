using UnityEngine;
using System.Collections;

/*
 * @brief       随泡泡掉落的下来的道具泡泡
 * @desc        如闪电，变色
 * @desc        闪电：该行泡泡全部消除
 * @desc        变色:周围泡泡类型变为发射泡泡
 */
public class DropPropPubbleObject : PubbleObject
{
    /*
     * @brief       如果自己是被撞击泡泡，执行自己泡泡的相关作用
     * @desc        执行掉落道具的功能
     */
    public void ExefunctionDropProp()
    {
        switch (pubbleType)
        {
            case PubbleColorType.PUBBLE_THUNDER_TYPE:
                //如果是闪电泡泡
                ExecuteThunderPubbleFunction();
                break;
            case PubbleColorType.PUBBLE_RAINBOW_TYPE:
                //如果是彩虹泡泡
                StartCoroutine(ExecuteRainbowFunction());
                break;
            default:
                break;        
        }
    }

    /*
     * @brief       执行闪电泡泡的功能：清除一行
     * @desc        先统计该泡泡所在行的所有泡泡：用射线去检测
     * @desc        将统计到的该行泡泡均标记为毁灭，执行毁灭效果，闪电效果记得调节位置
     * @desc        重置所有泡泡状态，检测掉落
     */
    void ExecuteThunderPubbleFunction()
    {
        Vector3 forwardDirection = new Vector3(1, 0, 0);
        Vector3 backDirection = new Vector3(-1, 0, 0);
        RaycastHit[] hitResults = Physics.RaycastAll(transform.position, forwardDirection, 2.0f, pubbleLayerMask);
        RaycastHit[] backHitResults = Physics.RaycastAll(transform.position, backDirection, 2.0f, pubbleLayerMask);

        //统计并设置毁灭标记
        for (int i = 0; i < hitResults.Length; i++)
        {
            GameObject tempHitObject = hitResults[i].collider.gameObject;
            if (tempHitObject.tag == ConstantValue.PlayObjectTag)
            { 
                tempHitObject.GetComponent<PubbleObject>().isBurst = true;
            }
        }
        for (int i = 0; i < backHitResults.Length; i++)
        {
            GameObject tempHitObject = hitResults[i].collider.gameObject;
            if (tempHitObject.tag == ConstantValue.PlayObjectTag)
            {
                tempHitObject.GetComponent<PubbleObject>().isBurst = true;
            }
        }
        //到此，该行泡泡，全部被标记毁灭
        isBurst = true;
        //延后0.2秒，执行毁灭与检测掉落
        Invoke("CheckAllObjectsFallOrBurst", 0.2f);
    }

    /*
     * @brief       如果是彩虹泡泡，执行变色功能：即使有可以消除的也不消除，只是执行变色
     * @desc        统计周边的泡泡
     * @desc        将周边泡泡修改类型，为发射泡泡类型，然后还原所有泡泡的状态
     */
    IEnumerator ExecuteRainbowFunction()
    {
        //将彩虹泡泡周围，普通泡泡 直接修改类型与显示，其他的道具泡泡则销毁重新创建
        for (int i = 0; i < adjacentPlayObjects.Length; i++)
        {
            if (adjacentPlayObjects[i] != null)
            {
                PubbleObject pubbleScript = adjacentPlayObjects[i];
                if ((int)PubbleColorType.PUBBLE_EMPTY_TYPE < (int)pubbleScript.pubbleType && (int)pubbleScript.pubbleType < (int)PubbleColorType.PUBBLE_AIR_TYPE)
                {
                    //如果是普通的脚本
                    pubbleScript.ChangeNormalPubbleTypeToMarkType();
                }
                else
                { 
                    //其他类型，后面再说：现在不搞了：方法 直接销毁重新创建新的
                    pubbleScript.ChangePropPubbleToMarkPubble();
                }
            }
        }
        //修改自己
        ChangePropPubbleToMarkPubble();
        yield return null;
    }
}
