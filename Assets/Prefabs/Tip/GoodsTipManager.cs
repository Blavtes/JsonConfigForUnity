using UnityEngine;
using System.Collections;

public class GoodsTipManager : MonoBehaviour
{

   // 多个提示框
    public GameObject TipObject;
    object [] allTitles;
    public void setTipTitles(object[] titles)
    {
        allTitles = titles;

        StartCoroutine(NewGoodsTips());
    }

     IEnumerator NewGoodsTips()
     {
         Debug.Log("多个提示框--------    IEnumerator NewGoodsTips()" + allTitles.Length);
         for (int i = 0; i < allTitles.Length; ++i)
         {
             GameObject tip = Instantiate(TipObject) as GameObject;
             tip.gameObject.transform.parent = gameObject.transform;
             tip.transform.localScale = new Vector3(1, 1, 1);
             tip.GetComponent<GoodsTipScript>().setTipTitle((string)allTitles[i]);
             yield return new WaitForSeconds(0.4f);
             Debug.Log("NewGoodsTip-----------------------" + Time.time);
         }
         Debug.Log("DestroyGameObject-----------------------" + Time.time);
         yield return new WaitForSeconds(0.6f);
         Destroy(gameObject);
     }

     //单个提示框
    string singleTitle;
    public void setTipTitle(string title)
    {
        singleTitle = title;
        StartCoroutine(NewGoodsTip());
    }

    IEnumerator NewGoodsTip()
    {
        GameObject tip = Instantiate(TipObject) as GameObject;
        tip.gameObject.transform.parent = gameObject.transform;
        tip.transform.localScale = new Vector3(1, 1, 1);
        tip.GetComponent<GoodsTipScript>().setTipTitle(singleTitle);

        print("NewGoodsTip-----------------------" + Time.time);
        yield return new WaitForSeconds(0.8f);
        print("NewGoodsTip-----------------------" + Time.time);
        
        Destroy(gameObject);
    }
}
