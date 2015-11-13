using UnityEngine;
using System.Runtime.InteropServices;

public class SDK
{
	 [DllImport("__Internal")]
    private static extern void _PayGods(int type);
     public static void CallFunction_iOS_PayGods(int type)
	 {
	 	
        if (Application.platform != RuntimePlatform.OSXEditor) 
        {
             _PayGods(type);
        }
	 }
}