using UnityEngine;
using System.Collections;

public class ShowTuition : MonoBehaviour {

	// Use this for initialization
    public bool ShowStartPnaelTuition = false;
	void Start () {
        ShowCurrentTuition();
        ShowTipDlg();
        ShowLimitTip();
	}

    void ShowCurrentTuition()
    {
        if (ShowStartPnaelTuition)
        {
            TuitionManager.Instance.CreateStartTuition();
        }
        else
        {
            TuitionManager.Instance.CreateTuition();
        }
    }

    void ShowTipDlg()
    {
        if (!ShowStartPnaelTuition)
        {
            //判断当前是否为第5关
            if (UserInstanse.GetInstance().chooseLevel_id == 4)
            {
                TuitionManager.Instance.ShowChanllangeTip();
            }
        }
    }

    void ShowLimitTip()
    {
        if (!ShowStartPnaelTuition)
        {
            //判断当前是否为第5关
            if (UserInstanse.GetInstance().chooseLevel_id == 9)
            {
                TuitionManager.Instance.ShowLimitTip();
            }
        }
    }
	
}
