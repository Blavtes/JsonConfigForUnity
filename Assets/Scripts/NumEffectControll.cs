using UnityEngine;
using System.Collections;

public class NumEffectControll : MonoBehaviour {

    //记得在预设内赋值
    public UILabel numberLabel = null;

	// Use this for initialization
	void Start () {
	
	}

    /*
     * @brief       修改显示的内容
     * @desc        分数
     */
    public void ChangeShowNumber(int number)
    {
        numberLabel.text = "" + number;
    }



}
