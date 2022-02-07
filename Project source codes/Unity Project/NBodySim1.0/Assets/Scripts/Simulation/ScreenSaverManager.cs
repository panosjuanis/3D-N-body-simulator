using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Profiling;

public class ScreenSaverManager : MonoBehaviour
{
    private string destinationFolder;

    public Button screenshotButton;
    public Toggle recordToggle;

    void Start() {
        screenshotButton.onClick.AddListener(saveScreenshot);
        destinationFolder = Application.streamingAssetsPath + "/Gallery/";
    }

    void Update()
    {
        
    }

    void saveScreenshot() {
        string filepath = buildFilePath();
        ScreenCapture.CaptureScreenshot(filepath);
    }

    string buildFilePath() {
        string simName = SimManager.selectedFilePath.Split('/')[SimManager.selectedFilePath.Split('/').Length - 1];
        simName = simName.Replace(".json", "");
        string simFolder = destinationFolder + simName + "/";
        
        //check if directory doesn't exit
        if (!Directory.Exists(simFolder)) {
            //if it doesn't, create it
            Directory.CreateDirectory(simFolder);
        }
        return simFolder + System.DateTime.UtcNow.ToString("dd MMMM HH_mm_ss") + ".png";
    }

}
