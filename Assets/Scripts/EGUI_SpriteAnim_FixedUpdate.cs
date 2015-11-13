using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EGUI_SpriteAnim_FixedUpdate : MonoBehaviour {
	
	/// <summary>
	/// 全部帧循环完成的时间.
	/// </summary>
	[SerializeField] float mLoopTime = 1f;
	/// <summary>
	/// 所有帧图片名字的前半部分.
	/// 如 SpriteName_0,SpriteName_1时，这个变量为SpriteName_
	/// </summary>
	[SerializeField] string mSpriteName;
	/// <summary>
	/// 帧图片的起始序号.
	/// </summary>
	[SerializeField] int mFramStart = 0;
	/// <summary>
	/// 所有帧图片的数量.
	/// </summary>
	[SerializeField] int mFramCount = 0;
	/// <summary>
	/// 这个序列帧是否是循环的.
	/// </summary>
	[SerializeField] bool mLoop = true;

    /// 这个序列帧放大倍数的.
    /// </summary>
    [SerializeField] float mScale = 1f;

	/// <summary>
	///  auto make pixel perfect.
	/// </summary>
	[SerializeField] bool mMakePixelPerfect = true;
	/// <summary>
	/// 自动生成的序列帧图片名列表.
	/// </summary>
	List<string> mSpriteNames = new List<string>();
	
    //尺寸
    [SerializeField]
    int[] mSaveSizeX;
    [SerializeField]
    int[] mSaveSizeY;
	/// <summary>
	/// Target Sprite.
	/// </summary>
	UISprite mSprite;
	/// <summary>
	/// Anim Timeer.
	/// </summary>
	float mDelta = 0f;
	/// <summary>
	/// Current sprite name index.
	/// </summary>
	int mIndex = 0;
	/// <summary>
	/// Anim is Active.
	/// </summary>
	bool mActive = true;

	/// <summary>
	/// Set the animation to be looping or not
	/// </summary>
	public bool loop { get { return mLoop; } set { mLoop = value; } }

	/// <summary>
	/// Returns is the animation is still playing or not
	/// </summary>
	public bool isPlaying { get { return mActive; } }
	
	public delegate void ANIMMSG(EGUI_SpriteAnim_FixedUpdate _Anim);
	ANIMMSG m_OnAnimFinish;

    public delegate void FinishDoSomething();
    public FinishDoSomething FinishHanelDelegate;

	/// <summary>
	/// Auto play At Start.
	/// </summary>
	public bool m_AutoPlay;
    float m_fLastTime;

    public int m_SpriteX, m_SpriteY;
	
	void Start () 
	{
        m_fLastTime = Time.realtimeSinceStartup;

		SetupSpriteName();
		mActive = m_AutoPlay;
		if( m_AutoPlay )
		{
			Play( null);
		}
		
	}
	
	void SetupSpriteName()
	{
		if (mSprite == null) mSprite = GetComponent<UISprite>(); 
		/// Build the sprite list 
		if( mSpriteNames == null )
		{
			mSpriteNames = new List<string>();
		}
		
		mSpriteNames.Clear();
		for( int i=0; i<mFramCount; ++i)
		{
			mSpriteNames.Add( mSpriteName + (mFramStart + i).ToString() );
		}
	}
	
	void Update ()
	{
		
		if( !mActive )
		{
			return;
		}
		
		if(mSprite != null)
		{
			if( !mSprite.enabled )
			{
				mSprite.enabled = true;
			}
		}


        float fDeltaTime = Time.realtimeSinceStartup - m_fLastTime;
        m_fLastTime = Time.realtimeSinceStartup;
        mDelta += fDeltaTime;

		//time between tow frams
		float rate = mLoopTime / mFramCount;

		if (rate < mDelta)
		{				
			//reset timeer when change frame
			mDelta = (rate > 0f) ? mDelta - rate : 0f;
			//finish process
			if (++mIndex >= mFramCount)
			{
				mIndex = 0;
				mActive = loop;
				if( !mActive  && mSprite != null)
				{
					mSprite.enabled = false;
					if( m_OnAnimFinish != null )
					{
						m_OnAnimFinish( this );
						m_OnAnimFinish = null;
					}
                    if (FinishHanelDelegate != null)
                    {
                        FinishHanelDelegate();
                        FinishHanelDelegate = null;
                    }
				}
			}

			if (mActive && mSprite != null)
			{
				mSprite.spriteName = mSpriteNames[mIndex];

                if(mMakePixelPerfect)
                {
                    mSprite.MakePixelPerfect();
                }
                else
                {
                    if (mSaveSizeX.Length > mIndex)
                    {
                        mSprite.SetDimensions(mSaveSizeX[mIndex], mSaveSizeY[mIndex]);
                        mSprite.transform.localScale = new Vector3(mScale, mScale, 1);
                    }
                    else
                    {
                        mSprite.SetDimensions(m_SpriteX, m_SpriteY);
                    }
                }
				
			}
		}
			

		
	}
	
	/// <summary>
	/// Reset the animation to frame 0 and activate it.
	/// </summary>
	

	public void Play( ANIMMSG _Callback )
	{
		if( mSpriteNames.Count <= 1 || !Application.isPlaying || mLoopTime <= 0f )
		{
			return;
		}
		
		mIndex = 0;
		
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			if( mSprite.enabled )
			{
				mSprite.enabled = false;
			}
			mSprite.spriteName = mSpriteNames[mIndex];
            if (mMakePixelPerfect)
                mSprite.MakePixelPerfect();
            else
                mSprite.SetDimensions(m_SpriteX, m_SpriteY);
		}
		m_OnAnimFinish = _Callback;
		
		if( !mActive )
		{
			mActive = true;
		}
	}

	public void SetIcon( string _SpriteName, int _Count, float _LoopTime )
	{
		mSpriteName = _SpriteName;
		mFramCount = _Count;
		mLoopTime = _LoopTime;
		SetupSpriteName();		
	}
	
	public void Stop()
	{
		enabled = false;
		mActive = false;
		mSpriteNames.Clear();
	}
	
}
