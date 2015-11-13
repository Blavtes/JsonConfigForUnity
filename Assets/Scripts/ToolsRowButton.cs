using UnityEngine;
using System.Collections;

/*
 * @brief       将本行脚本上泡泡打印出来
 */
public class ToolsRowButton : MonoBehaviour {

    GameObject myObject = null;
	// Use this for initialization
	void Start () {
        myObject = gameObject;
	}

    public void SaveRowButtonPubbles()
    {
        ToolsPubbleButton[] pubbleButtons = myObject.GetComponentsInChildren<ToolsPubbleButton>();
        string outPutString = "";
        foreach (ToolsPubbleButton pubbleScript in pubbleButtons)
        {
            outPutString += (int)pubbleScript.myColorType;
            outPutString += ',';
            outPutString += pubbleScript.actionString.Trim();            
            outPutString += '\t';
        }

        outPutString += '\r';
        outPutString += '\n';

        ResourceManager.SaveStringToFile("/files.txt",outPutString);


    }
}
