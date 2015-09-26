using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine.UI; // we need This namespace in order to access UI elements within our script

public class SC_FB_Holder : MonoBehaviour
{

   /* void Start()
    {
        FB.Init(SetInit, OnHideUnity);
    }
    */

    public void InitFB()
    {
        FB.Init(SetInit, OnHideUnity);
    }

    private void SetInit()
    {
        if (FB.IsLoggedIn)
            Debug.Log("user already logged in");
        else
        {
            facbookLogin();
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (isGameShown == false)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void facbookLogin()
    {
        FB.Login("email", AuthCallback);
    }

    //FBResult comes back facebook
    private void AuthCallback(FBResult result)
    {
        if (FB.IsLoggedIn)
        {

            Debug.Log("Logged to Facebook!!!");
          //  FB.API("me?fields=picture.height(200).width(200)", Facebook.HttpMethod.GET, MyPictureGraphCallBack);
           // FB.API("me?fields=name,email", Facebook.HttpMethod.GET, MyCallBack);
        }
    }

}