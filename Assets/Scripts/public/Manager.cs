using UnityEngine;
using System.Collections;

/*
 * @brief       管理所有的Manager
 * @desc        暂时管理的是 soundManager
 * @desc        
 */
public class Manager : MonoBehaviour {

    

	void Start () {
        CreateSoundManager();
        CreateLogicManager();
        CreateTuitionManager();
        CreateRechargeManager();
    }


    #region 负责创建soundManager，防止重复创建
    public GameObject soundManagerObject = null;
    /*
     * @brief       创建
     */
    void CreateSoundManager()
    {
        //从场景之中查找是否已经存在了 soundManager ，不存在则创建
        GameObject findResult = GameObject.FindGameObjectWithTag(ConstantValue.SoundManagerTag);
        if (findResult == null && soundManagerObject != null)
        {
            Instantiate(soundManagerObject);
        }
    }
    #endregion 负责创建soundManager，防止重复创建

    #region 负责创建 rechargeManager，防止重复创建
    public GameObject rechargeManagerObject = null;
    /*
     * @brief       创建
     */
    void CreateRechargeManager()
    {
        //从场景之中查找是否已经存在了 soundManager ，不存在则创建
        GameObject findResult = GameObject.FindGameObjectWithTag(ConstantValue.RechargeManagerTag);
        if (findResult == null && rechargeManagerObject != null)
        {
            Instantiate(rechargeManagerObject);
        }
    }
    #endregion 负责创建soundManager，防止重复创建

    #region 负责创建LogicManager,防止重复创建
    public GameObject logicManager = null;
    void CreateLogicManager()
    {
        //从场景之中查找是否已经存在了 soundManager ，不存在则创建
        GameObject logicObject = GameObject.FindGameObjectWithTag(ConstantValue.LogicManagerTag);
        if (logicObject == null && logicManager != null)
        {
            Instantiate(logicManager);
        }
    }
    #endregion 负责创建LogicManager,防止重复创建

    #region 负责创建新手指引
    public GameObject tuitionManager = null;
    void CreateTuitionManager()
    {
        //从场景之中查找是否已经存在了 soundManager ，不存在则创建
        GameObject tuitionObject = GameObject.Find(ConstantValue.TuitionName);
        if (tuitionObject == null && tuitionManager != null)
        {
            Instantiate(tuitionManager);
        }
    }

    #endregion 负责创建新手指引
}
