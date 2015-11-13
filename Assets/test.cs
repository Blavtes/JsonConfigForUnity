using UnityEngine;
using System.Collections;

using System.Text;
using System.Runtime.InteropServices;
using System;
public class test : MonoBehaviour {

	public GameObject abcBt;
	// Use this for initialization
	public GameObject packFeild;
	public GameObject bluetooth;

	public GameObject plane;
	
	public GUIStyle open;


	void  Awake()
	{
		UIEventListener.Get(abcBt).onClick = TTTTClick;
		packFeild = GameObject.Find("UI Root/Panel/packFeild");
		bluetooth = GameObject.Find("UI Root/Panel/bluetooth");
		UIEventListener.Get(bluetooth).onClick = bluetoothClick;

	}

	void Start () {
		Debug.Log("test....sss.");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void bluetoothClick(GameObject obj)
	{
		if (bluetooth.GetComponent<UIToggle>().value) {
				
		}
	}

	public void testClick(GameObject obj)
	{
		Debug.Log("test....." + packFeild.GetComponent<UIInput>().value);
	}

	public void TTTTClick(GameObject button) {
		Debug.Log("test....." + packFeild.GetComponent<UIInput>().value);
		Debug.Log("test 23 " + bluetooth.GetComponent<UIToggle>().value);

	}

	public void openFile()
	{
		{
			OpenFileName ofn = new OpenFileName();
			
			ofn.structSize = Marshal.SizeOf(ofn);
			
			ofn.filter = "JPG Files (*.jpg)\0*.jpg\0All Files (*.*)\0*.*\0";
			
			ofn.file = new string(new char[256]);
			
			ofn.maxFile = ofn.file.Length;
			
			ofn.fileTitle = new string(new char[64]);
			
			ofn.maxFileTitle = ofn.fileTitle.Length;
			
			ofn.initialDir =UnityEngine.Application.dataPath;//默认路径
			
			ofn.title = "打开文件";
			
			ofn.defExt = "JPG";//显示文件的类型
			//注意 一下项目不一定要全选 但是0x00000008项不要缺少
			ofn.flags=0x00080000|0x00001000|0x00000800|0x00000200|0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
			
			if(DllTest.GetOpenFileName( ofn ))
			{
				
				StartCoroutine(WaitLoad(ofn.file));//加载图片到panle
				
				Debug.Log( "Selected file with full path: {0}"+ofn.file );
				
			}
		}
	}

	//加载图片
	IEnumerator WaitLoad(string fileName)
	{
		WWW wwwTexture=new WWW("file://"+fileName);
		
		Debug.Log(wwwTexture.url);
		
		yield return wwwTexture;
		
		plane.renderer.material.mainTexture=wwwTexture.texture;
	}
}
