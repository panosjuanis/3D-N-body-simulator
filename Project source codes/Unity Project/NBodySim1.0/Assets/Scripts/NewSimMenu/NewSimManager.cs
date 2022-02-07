using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Globalization;

public class NewSimManager : MonoBehaviour
{
    public DataManager dataManager;
    public NewSimPanelManager panelManager;
    public static bool editingMode = false;

    private SimParameters simParameters;
    private BodyData[] bodies;

    //Canvas input fields
    public InputField G;
    public InputField dt;
    public InputField FPS;
    public InputField trailSize;
    public InputField simName;

    public Dropdown integrator;

    //Canvas elements
    public Button saveSimButton;
    public Button backButton;

    public Blinker blinkText;


    void Start() {
        saveSimButton.onClick.AddListener(saveSim);
        backButton.onClick.AddListener(backToMainMenu);
        if (editingMode)
            editSimulation();
    }

    public void editSimulation() {
        editingMode = true;
        importSimParameters();
        setBodies();
        setInputFields();
    }

    void saveSim() {
        if (!inputFieldsEmpty() && panelManager.hasBodies()) {
            dataManager.saveSimulationParameters(editingMode);
            editingMode = false;
            SceneManager.LoadScene("Simulation");
        }
        else
            blinkText.blink(4);
    }

    void backToMainMenu() {
        editingMode = false;
        SceneManager.LoadScene("MainMenu");
    }

    void importSimParameters() {
        string simParametersString = File.ReadAllText(SimManager.selectedFilePath);
        simParameters = JsonUtility.FromJson<SimParameters>(simParametersString);

    }

    void setBodies() {
        panelManager.loadBodies(simParameters);
    }

    void setInputFields() {
        //set integrator value
        List<Dropdown.OptionData> list = integrator.options;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].text.Equals(simParameters.integrator)) { integrator.value = i; }
        }
        G.text = simParameters.G.ToString(CultureInfo.InvariantCulture);
        dt.text = simParameters.dt.ToString(CultureInfo.InvariantCulture);
        FPS.text = simParameters.fps.ToString(CultureInfo.InvariantCulture);
        trailSize.text = simParameters.trailSize.ToString(CultureInfo.InvariantCulture);

        //First we remove all the relative path, then we remove the .json
        simName.text = SimManager.selectedFilePath.Split('/')[SimManager.selectedFilePath.Split('/').Length - 1];
        simName.text = simName.text.Split('.')[simName.text.Split('.').Length - 2];
    }

    bool inputFieldsEmpty() {
        return simName.text == ""
                || G.text == ""
                || FPS.text == ""
                || trailSize.text == "";
    }



}
