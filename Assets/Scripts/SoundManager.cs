using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    private static SoundManager mInstance = null;
    public static SoundManager Instance { get { return mInstance; } }

    void Awake()
    {
        mInstance = this;
        //myAudioSource = GetComponent<AudioSource>();
        AudioSource[] aduioSources = GetComponents<AudioSource>();
        myAudioSource = aduioSources[0];
        warningAudioSource = aduioSources[1];
        DontDestroyOnLoad(transform.gameObject);
    }

	void Start () {
        Debug.Log("soundManager--Start");
        PlayCurrentSceneGroundMusic();
        InitWarningSound();
	}

    #region 负责背景音乐的播放
    //音源
    AudioSource myAudioSource = null;
    //保存所有的背景音乐
    public AudioClip[] allGdAudioClips;
    /*
     * @brief       播放背景音乐
     * @desc        根据当前场景播放不同背景，当数组内不存在的时候默认第一个背景
     */
    void PlayCurrentSceneGroundMusic()
    {
        int sceneIndex = Application.loadedLevel;
        if (sceneIndex >= allGdAudioClips.Length)
        {
            sceneIndex = 0;
        }

        myAudioSource.clip = allGdAudioClips[sceneIndex];
        
        Debug.Log("UserInstanse.GetInstance().soundSet ------" + UserInstanse.GetInstance().soundSet);
        if (UserInstanse.GetInstance().soundSet)
        {
            myAudioSource.Play();
        }           
    }
    //缩小声音
    public void ShrinkVolume()
    {
        myAudioSource.volume = 0.3f;
    }
    //放大
    public void BlowUpVolume()
    {
        myAudioSource.volume = 1f;
    }

    #endregion 负责背景音乐的播放

    #region 负责播放各个特效
    //各个按钮的点击音效
    public AudioClip buttonClip = null;
    public void PlayButtonTouchSound()
    {
        if (buttonClip != null  && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(buttonClip, Vector3.zero, 1);
    }

    //消除声音
    public AudioClip burstClip = null;
    public void PlayBurstSound()
    {
        if (burstClip != null  && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(burstClip, Vector3.zero, 1);
    }

    //碰撞音效
    public AudioClip collisionClip = null;
    public void PlayCollisionSound()
    {
        if (collisionClip != null  && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(collisionClip, Vector3.zero, 1);
    }

    //冲击波音效
    public AudioClip crushClip = null;
    public void PlayCurshSound()
    {
        if (crushClip != null && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(crushClip, Vector3.zero, 1);
    }

    //爆炸音效
    public AudioClip fireClip = null;
    public void PlayFireSound()
    {
        if (fireClip != null && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(fireClip, Vector3.zero, 1);
    }

    //彩虹特效
    public AudioClip rainBowClip = null;
    public void PlayRainBowSound()
    {
        if (rainBowClip != null  && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(rainBowClip, Vector3.zero, 1);
    }

    //冰冻特效
    public AudioClip snowClip = null;
    public void PlaySnowEffectSound()
    {
        if (snowClip != null  && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(snowClip, Vector3.zero, 1);
    }

    //闪电特效
    public AudioClip thunderClip = null;
    public void PlayThunderSound()
    {
        if (thunderClip != null && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(thunderClip, Vector3.zero, 1);
    }

    //警戒音效
    public AudioClip warningClip = null;
    AudioSource warningAudioSource = null;
    void InitWarningSound()
    {
        if (warningClip != null && UserInstanse.GetInstance().audioSet && warningAudioSource != null)
        {
            warningAudioSource.clip = warningClip;
        }
    }

    //播放警戒音
    public void PlayWarningSound()
    {
        if (UserInstanse.GetInstance().audioSet && !warningAudioSource.isPlaying)
            warningAudioSource.Play();
    }

    public void PauseWarningSound()
    {
        warningAudioSource.Stop();
    }


    //失败音效
    public AudioClip loseClip = null;
    public void PlayLoseSound()
    {
        //缩小声音
        ShrinkVolume();
        if (loseClip != null  && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(loseClip, Vector3.zero, 1);
    }

    //胜利音效
    public AudioClip winClip = null;
    public void PlayWinSound()
    {
        //缩小声音
        ShrinkVolume();
        if (winClip != null  && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(winClip, Vector3.zero, 1);
    }

    //欢呼音效：cool的时候谈
    public AudioClip cheerClip = null;
    public void PlayCheerClip()
    {
        if (cheerClip != null && UserInstanse.GetInstance().audioSet)
            AudioSource.PlayClipAtPoint(cheerClip, Vector3.zero, 1);
    }

    #endregion 负责播放各个特效

    #region 控制音效与背景是否播放

    public void CloseBackGroundMusic()
    {
        //如果当前正在播放，则停止播放
        if (UserInstanse.GetInstance().soundSet)
        {
            UserInstanse.GetInstance().soundSet = false;
            myAudioSource.Pause();
        }
    }

    public void PlayBackGroundMusic()
    {
        //如果当前没有播放，则进行播放
        if (!UserInstanse.GetInstance().soundSet)
        {
            UserInstanse.GetInstance().soundSet = true;
            myAudioSource.Play();
        }    
    }

    #endregion 控制音效与背景是否播放

    //当预设内失去关联的时候调用
    AudioClip LoadAudioSources(string filename)
    {
        AudioClip audioClip = Resources.Load("music/" + filename) as AudioClip;
        return audioClip;
    }

    
}