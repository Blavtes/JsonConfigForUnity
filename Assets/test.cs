using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	public GameObject abcBt;
	// Use this for initialization
	public GameObject packFeild;
	public GameObject bluetooth;
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

	}

	public void testClick(GameObject obj)
	{
		Debug.Log("test....." + packFeild.GetComponent<UIInput>().value);
	}

	public void TTTTClick(GameObject button) {
		Debug.Log("test....." + packFeild.GetComponent<UIInput>().value);
		Debug.Log("test 23 " + bluetooth.GetComponent<UIToggle>().value);
	}
}
