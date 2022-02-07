using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopUpMenuManager : MonoBehaviour
{
    public GameObject popUpMenu;
    public ToggleController toggleController;

    //Pop up menu objects
    public Button resumeButton;
    public Button exitButton;

    void Start() {
        popUpMenu.SetActive(false);

        resumeButton.onClick.AddListener(stopOrResumeSimulation);
        exitButton.onClick.AddListener(exitToMainMenu);
    }
    void Update()
    {
        //Show/hide the menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            stopOrResumeSimulation();
        }
            
    }

    void stopOrResumeSimulation() {
        toggleController.pauseOrUnpause();
        popUpMenu.SetActive(!popUpMenu.active);
    }

    void exitToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
