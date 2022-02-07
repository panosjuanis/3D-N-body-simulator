using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    public Button createSimButton;
    public Button loadSimButton;
    public Button instructionsButton;
    public Button exitButton;
    public Button galleryButton;

    //Pop up menu stuff
    public Button[] nextButtons;
    public Button closeButton;
    public GameObject[] popUpMenus;
    public int popUpMenuIndex = 0;


    void Start()
    {
        setupButtons();
    }


    void setupButtons() {
        createSimButton.onClick.AddListener(createSimulation);
        loadSimButton.onClick.AddListener(loadSimulation);
        instructionsButton.onClick.AddListener(nextInstructions);
        exitButton.onClick.AddListener(exitProgram);
        galleryButton.onClick.AddListener(showGallery);
        for(int i = 0; i < nextButtons.Length; i++)
            nextButtons[i].onClick.AddListener(nextInstructions);
        closeButton.onClick.AddListener(hidePopUpMenus);
    }

    //Functions to load scenes
    void createSimulation() {
        SceneManager.LoadScene("CreateSimulation");
    }

    void loadSimulation() {
        SceneManager.LoadScene("LoadSimulation");
    }

    void nextInstructions() {
        hidePopUpMenus();
        popUpMenus[popUpMenuIndex++].SetActive(true);
        if (popUpMenuIndex >= popUpMenus.Length)
            popUpMenuIndex = 0;
    }

    void showGallery() {
        string path = Application.streamingAssetsPath + "/Gallery/";
        OpenInFileBrowser(path);
        /*path = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + path);*/
        //EditorUtility.RevealInFinder(Application.streamingAssetsPath + "/Gallery"); -> only works in editor
    }

    void exitProgram() { //Won't work in unity editor, only in built program, seems to not work properly in Mac
        Application.Quit();
    }

    void hidePopUpMenus() {
        for(int i = 0; i < popUpMenus.Length; i++)
            popUpMenus[i].SetActive(false);
    }

    public static void OpenInMacFileBrowser(string path) {
        bool openInsidesOfFolder = false;

        // try mac
        string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

        if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        //Debug.Log("macPath: " + macPath);
        //Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

        if (!macPath.StartsWith("\"")) {
            macPath = "\"" + macPath;
        }
        if (!macPath.EndsWith("\"")) {
            macPath = macPath + "\"";
        }
        string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
        //Debug.Log("arguments: " + arguments);
        try {
            System.Diagnostics.Process.Start("open", arguments);
        }
        catch (System.ComponentModel.Win32Exception e) {
            // tried to open mac finder in windows
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
    }

    public static void OpenInWinFileBrowser(string path) {
        bool openInsidesOfFolder = false;

        // try windows
        string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

        if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }
        try {
            System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
        }
        catch (System.ComponentModel.Win32Exception e) {
            // tried to open win explorer in mac
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
    }

    public static void OpenInFileBrowser(string path) {
        OpenInWinFileBrowser(path);
        OpenInMacFileBrowser(path);
    }

}
