using UnityEngine;
using System.Collections;

public class GoodsTipScript : MonoBehaviour
{


    public GameObject labelTip;

	// Use this for initialization
	void Start () 
    {
	
	}
    private float allTime = 0f;
    int cout = 0;
    void Update()
    {


        float costTime = 2 * 0.02f;
        allTime += costTime;

        //print("======-----距上一次调用Update所用的时间----" + costTime + "总计花费的时间：" + allTime);
        if (allTime > 20)
        {
            //print("GoodsTipScript-----距上一次调用Update所用的时间----" + costTime + "总计花费的时间：" + allTime);
            Destroy(gameObject);
        }


        gameObject.GetComponent<UIPanel>().alpha -= 0.005f;
        transform.localPosition += new Vector3(0, 100*costTime, 0);

        //transform.Translate(0, costTime, 0);
    }

     public  void setTipTitle(string title)
    {
        labelTip.GetComponent<UILabel>().text = title;
    }
}
