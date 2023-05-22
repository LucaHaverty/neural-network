using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotController : MonoBehaviour
{
    public static ScreenshotController instance;

    public Camera networkGraphCam;
    public Camera costGraphCam;
    public string testName;

    private void Awake() { instance = this; }

    public void TakeNetworkPicture(string fileName)
    {
        SaveCameraView(networkGraphCam, fileName);
    }
    
    public void TakeCostPicture(string fileName)
    {
        SaveCameraView(costGraphCam, fileName);
    }
    
    public void SaveCameraView(Camera cam, string fileName)
    {
        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        cam.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        cam.Render();
        Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;
        byte[] byteArray = renderedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Users/lucah/Desktop/Math-IA-Screenshots/"+testName+fileName+".png", byteArray);
    }
}
