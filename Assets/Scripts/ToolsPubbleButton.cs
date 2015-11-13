using UnityEngine;
using System.Collections;


public class ToolsPubbleButton : MonoBehaviour {
    public PubbleColorType myColorType = PubbleColorType.PUBBLE_EMPTY_TYPE;
    public string actionString = "";
    GameObject myObject = null;

    void Start()
    {
        myObject = gameObject;
    }

    public void SelectColorType()
    {
        switch (UIPopupList.current.value.Trim())
        {
            case "无":
                myColorType = PubbleColorType.PUBBLE_EMPTY_TYPE;
                break;
            case "黄":
                myColorType = PubbleColorType.PUBBLE_YELLOW_TYPE;
                break;
            case "橙":
                myColorType = PubbleColorType.PUBBLE_ORANGE_TYPE;
                break;
            case "绿":
                myColorType = PubbleColorType.PUBBLE_GREEN_TYPE;
                break;
            case "紫":
                myColorType = PubbleColorType.PUBBLE_PURPLE_TYPE;
                break;
            case "蓝":
                myColorType = PubbleColorType.PUBBLE_BLUE_TYPE;
                break;
            case "青":
                myColorType = PubbleColorType.PUBBLE_CYAN_TYPE;
                break;
            case "红":
                myColorType = PubbleColorType.PUBBLE_RED_TYPE;
                break;
            case "气泡":
                //myColorType = PubbleColorType.PUBBLE_GAS_TYPE;
                break;
            case "石头":
                myColorType = PubbleColorType.PUBBLE_STONE_TYPE;
                break;
            default:
                break;
        }


        int indexPerfab = (int)myColorType;
        if (indexPerfab >= 0)
        {
            if (indexPerfab > ToolsManager.Instance.pubblePerfabs.Length - 1)
            {
                indexPerfab = ToolsManager.Instance.pubblePerfabs.Length - 1;
            }

            GameObject pubbleObject = Instantiate(ToolsManager.Instance.pubblePerfabs[indexPerfab]) as GameObject;
            pubbleObject.transform.parent = myObject.transform;
            pubbleObject.transform.localPosition = Vector3.zero;
        }
    }

    public void selectActionType()
    {
        actionString = UIPopupList.current.value;
    }

	

}
