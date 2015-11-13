using UnityEngine;
using System.Collections;

public class TipSuccessDlg : MonoBehaviour
{

	// Use this for initialization
    public GameObject successPanel = null;

	void Start () {

        UIEventListener.Get(successPanel).onClick = CloseDlgButton;
	}

    void CloseDlgButton(GameObject button)
    {
        Application.LoadLevel("LevelScene");
    }
}
