using UnityEngine;
using System.Collections;


/*
 * @brief       负责播放音效
 * @desc        如果是爆炸等效果，直接绑定在perfab上
 */

public enum SoundType
{
    EMPTY_SOUND_TYPE,               //没有
    PUBBLE_FIRE_TYPE,               //泡泡爆炸的声音
    FIRE_SOUND_TYPE,                //爆炸
    CRUSH_SOUND_TYPE,               //冲击波
    SNOW_SOUND_TYPE,                //冰冻
    THUNDER_SOUND_TYPE,             //闪电
    WARNING_SOUND_TYPE,             //警戒线
};
public class PlayEffectSound : MonoBehaviour {

    public SoundType effectSoundType = SoundType.EMPTY_SOUND_TYPE; 
	// Use this for initialization
	void Start () {
        switch (effectSoundType)
        { 
            case SoundType.EMPTY_SOUND_TYPE:
                //不播放
                break;
            case SoundType.PUBBLE_FIRE_TYPE:
                SoundManager.Instance.PlayBurstSound();
                break;
            case SoundType.FIRE_SOUND_TYPE:
                SoundManager.Instance.PlayFireSound();
                break;
            case SoundType.CRUSH_SOUND_TYPE:
                SoundManager.Instance.PlayCurshSound();
                break;
            case SoundType.SNOW_SOUND_TYPE:
                SoundManager.Instance.PlaySnowEffectSound();
                break;
            case SoundType.THUNDER_SOUND_TYPE:
                SoundManager.Instance.PlayThunderSound();
                break;
            case SoundType.WARNING_SOUND_TYPE:
                break;
            default:
                break;
        }

	}
}
