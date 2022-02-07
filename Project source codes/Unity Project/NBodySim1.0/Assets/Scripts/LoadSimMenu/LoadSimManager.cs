using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadSimManager : MonoBehaviour
{
    public LoadMenuPanelManager panelManager;

    public Button backButton;
    public Button loadSimButton;
    public Button editButton;
    public Button deleteButton;

    void Start()
    {
        setupButtons();
    }

    void Update() {
        //When we select a file, we enable the file dependend buttons
        if(SimManager.selectedFilePath != "NOFILESELECTED") {
            loadSimButton.interactable = true;
            editButton.interactable = true;
            deleteButton.interactable = true;
        }    
    }

    void setupButtons() {
        backButton.onClick.AddListener(goBackToMainMenu);
        loadSimButton.onClick.AddListener(loadSimulation);
        editButton.onClick.AddListener(editSimulation);
        deleteButton.onClick.AddListener(deleteSimulation);

        //We disable all that deal with files and enable them when a file is selected
        loadSimButton.interactable = false;
        editButton.interactable = false;
        deleteButton.interactable = false;
    }

    void loadSimulation() {
        SceneManager.LoadScene("Simulation");
    }

    void goBackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    void editSimulation() {
        NewSimManager.editingMode = true;
        SceneManager.LoadScene("CreateSimulation");
    }

    void deleteSimulation() {
        if (File.Exists(SimManager.selectedFilePath)) {
            File.Delete(SimManager.selectedFilePath);
            Debug.Log("File deleted.");
        }
        else   //This should never happen
            Debug.Log("File not found");
        panelManager.loadPanel();
    }    
}
