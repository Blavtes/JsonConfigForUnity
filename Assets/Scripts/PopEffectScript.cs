using UnityEngine;
using System.Collections;

/*
 * @brief       负责管理弹出的cool，great，prefect效果的消除
 */
public class PopEffectScript : MonoBehaviour {

    const float limitTime = 1.05f;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, limitTime);
	}
}
