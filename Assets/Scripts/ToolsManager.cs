using UnityEngine;
using System.Collections;

public class ToolsManager : MonoBehaviour {

    //perfabs
    public GameObject[] pubblePerfabs;

    private static ToolsManager m_Instance = null;
    public static ToolsManager Instance { get { return m_Instance; } private set { m_Instance = value; } }

    void Start()
    {
        m_Instance = this;
    }


    public void SaveAllPubblesInfo()
    {
        GameObject ojsj = GameObject.Find("AlonePubble");
        ToolsRowButton spcrpp = ojsj.GetComponent<ToolsRowButton>();
        spcrpp.SaveRowButtonPubbles();
    }
}
