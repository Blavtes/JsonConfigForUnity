using UnityEngine;
using System.Collections;

public class LoadingScale : MonoBehaviour {

    //延时时间段
    float perTime = 0.3f;
    //延时段数
    public int perTimes = 1;
    //总时间
    float delayAllTime = 0;

	// Use this for initialization
	void Start () {
        delayAllTime = perTime * (float)perTimes;
	}

    float countTime = 0f;
    bool continueCount = true;
	void Update () {
        if (continueCount)
        {
            countTime += Time.deltaTime;
            if (countTime > delayAllTime)
            {
                GetComponent<Animator>().SetBool("PlayScale", true);
                continueCount = false;
            }
        }
	}
}
