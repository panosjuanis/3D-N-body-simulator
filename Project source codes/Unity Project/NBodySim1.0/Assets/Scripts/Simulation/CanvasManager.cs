using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class CanvasManager : MonoBehaviour
{
    public string G_UNIT = " m^3*kg^−1*s^-2";
    public string T_UNIT = " s";
    public string Q_UNIT = " m";
    public string V_UNIT = " m/s";
    public string MASS_UNIT = " kg";

    public static double STREAM_INCREASE_FACTOR = 1.01f;
    public static double STREAM_DECREASE_FACTOR = 0.99f;

    public static double SINGLE_INCREASE_FACTOR = 1.1f;
    public static double SINGLE_DECREASE_FACTOR = 0.925f;

    public GameObject cellPrefab;

    public SimManager simManager;

    //Hide canvas related params
    public GameObject hideableObjects;
    public Button hideButton;
    private bool hiddenCanvas = false;

    //User interactable Input Fields and related Buttons + bool increase fields
    public InputField GIF;
    public InputField dtIF;
    public InputField fpsIF;
    public InputField zoomIF;

    public Button GUp;
    public Button GDown;
    public Button DTUp;
    public Button DTDown;
    public Button FPSUp;
    public Button FPSDown;
    public Button ZoomUp;
    public Button ZoomDown;

    private bool GIncrease = false;
    private bool DTIncrease = false;
    private bool FPSIncrease = false;
    private bool ZoomIncrease = false;

    private bool GDecrease = false;
    private bool DTDecrease = false;
    private bool FPSDecrease = false;
    private bool ZoomDecrease = false;


    //User cannot interact with these objects
    public TextMeshProUGUI simTimeText;
    public TextMeshProUGUI realTimeText;
    public TextMeshProUGUI nFramesText;
    public TextMeshProUGUI integratorText;

    //Body params
    public TextMeshProUGUI bodyNameText;
    public TextMeshProUGUI massText;

    public TextMeshProUGUI qx;
    public TextMeshProUGUI qy;
    public TextMeshProUGUI qz;

    public TextMeshProUGUI vx;
    public TextMeshProUGUI vy;
    public TextMeshProUGUI vz;


    void Start()
    {
        initInputFields();
        initButtons();
    }

    void Update()
    {
        if (!hiddenCanvas) {
            updateBodyTexts();
            updateInputFieldValues();
            updateInputFieldTexts();
            updateStaticTexts();
        }
    }


    void updateBodyTexts() {
        int selectedBodyIndex = simManager.selectedBodyIndex;

        bodyNameText.text = simManager.names[selectedBodyIndex].ToString();
        massText.text = simManager.masses[selectedBodyIndex].ToString("E5");

        qx.text = simManager.history[simManager.history.Count - 1][selectedBodyIndex * 6].ToString("E5");
        qy.text = simManager.history[simManager.history.Count - 1][selectedBodyIndex * 6 + 1].ToString("E5");
        qz.text = simManager.history[simManager.history.Count - 1][selectedBodyIndex * 6 + 2].ToString("E5");

        vx.text = simManager.history[simManager.history.Count - 1][selectedBodyIndex * 6 + 3].ToString("E5");
        vy.text = simManager.history[simManager.history.Count - 1][selectedBodyIndex * 6 + 4].ToString("E5");
        vz.text = simManager.history[simManager.history.Count - 1][selectedBodyIndex * 6 + 5].ToString("E5");

    }
    
    void initInputFields() {
        GIF.onEndEdit.AddListener(delegate { setValue("G", GIF.text); });
        dtIF.onEndEdit.AddListener(delegate { setValue("dt", dtIF.text); });
        fpsIF.onEndEdit.AddListener(delegate { setValue("fps", fpsIF.text); });
        zoomIF.onEndEdit.AddListener(delegate { setValue("scaleFactor", zoomIF.text); });
    }

    void initButtons() { //Assign listeners to all buttons
        //Listeners for a single click
        GUp.onClick.AddListener(() => changeValue("G", SINGLE_INCREASE_FACTOR));
        DTUp.onClick.AddListener(() => changeValue("dt", SINGLE_INCREASE_FACTOR));
        FPSUp.onClick.AddListener(() => changeValue("fps", SINGLE_INCREASE_FACTOR));
        ZoomUp.onClick.AddListener(() => changeValue("scaleFactor", SINGLE_INCREASE_FACTOR));

        GDown.onClick.AddListener(() => changeValue("G", SINGLE_DECREASE_FACTOR));
        DTDown.onClick.AddListener(() => changeValue("dt", SINGLE_DECREASE_FACTOR));
        FPSDown.onClick.AddListener(() => changeValue("fps", SINGLE_DECREASE_FACTOR));
        ZoomDown.onClick.AddListener(() => changeValue("scaleFactor", SINGLE_DECREASE_FACTOR));

        //Listeners for mouse down, when mouse is down we start increasing
        AddTriggerToButton(GUp, EventTriggerType.PointerDown, "G", "increase", true);
        AddTriggerToButton(GUp, EventTriggerType.PointerUp, "G", "increase", false);
        AddTriggerToButton(DTUp, EventTriggerType.PointerDown, "dt", "increase", true);
        AddTriggerToButton(DTUp, EventTriggerType.PointerUp, "dt", "increase", false);
        AddTriggerToButton(FPSUp, EventTriggerType.PointerDown, "fps", "increase", true);
        AddTriggerToButton(FPSUp, EventTriggerType.PointerUp, "fps", "increase", false);
        AddTriggerToButton(ZoomUp, EventTriggerType.PointerDown, "scaleFactor", "increase", true);
        AddTriggerToButton(ZoomUp, EventTriggerType.PointerUp, "scaleFactor", "increase", false);

        //Idem for decrease
        AddTriggerToButton(GDown, EventTriggerType.PointerDown, "G", "decrease", true);
        AddTriggerToButton(GDown, EventTriggerType.PointerUp, "G", "decrease", false);
        AddTriggerToButton(DTDown, EventTriggerType.PointerDown, "dt", "decrease", true);
        AddTriggerToButton(DTDown, EventTriggerType.PointerUp, "dt", "decrease", false);
        AddTriggerToButton(FPSDown, EventTriggerType.PointerDown, "fps", "decrease", true);
        AddTriggerToButton(FPSDown, EventTriggerType.PointerUp, "fps", "decrease", false);
        AddTriggerToButton(ZoomDown, EventTriggerType.PointerDown, "scaleFactor", "decrease", true);
        AddTriggerToButton(ZoomDown, EventTriggerType.PointerUp, "scaleFactor", "decrease", false);

        //Listener to hide button
        hideButton.onClick.AddListener(hideCanvas);
    }

    void hideCanvas() {
        if (hiddenCanvas) {
            hideableObjects.transform.localScale = new Vector3(1, 1, 1);
        }
        else {
            hideableObjects.transform.localScale = new Vector3(0, 0, 0);
        }
        hiddenCanvas = !hiddenCanvas;
    }

    void AddTriggerToButton(Button b, EventTriggerType eventTrigger, string param, string increaseOrDecrease, bool value) {
        var et = new EventTrigger.Entry();
        et.eventID = eventTrigger;
        et.callback.AddListener((e) => setIncreaseOrDecrease(param,increaseOrDecrease, value));
        b.gameObject.GetComponent<EventTrigger>().triggers.Add(et);
    }

    void setIncreaseOrDecrease(string param, string increaseOrDecrease, bool value) {
        switch (param) {
            case "G":
                if (increaseOrDecrease == "increase")
                    GIncrease = value;
                if (increaseOrDecrease == "decrease")
                    GDecrease = value;
                break;

            case "dt":
                if (increaseOrDecrease == "increase")
                    DTIncrease = value;
                if (increaseOrDecrease == "decrease")
                    DTDecrease = value;
                break;

            case "fps":
                if (increaseOrDecrease == "increase")
                    FPSIncrease = value;
                if (increaseOrDecrease == "decrease")
                    FPSDecrease = value;
                break;

            case "scaleFactor":
                if (increaseOrDecrease == "increase")
                    ZoomIncrease = value;
                if (increaseOrDecrease == "decrease")
                    ZoomDecrease = value;
                break;
            default:
                Debug.Log("ERROR: SHOULD NOT REACH THIS CODE");
                break;
        }
    }

    void changeValue(string param, double factor) {
        switch (param) {
            case "G":
                Debug.Log("Increasing g");
                simManager.G = simManager.G * factor;
                break;
            case "dt":
                Debug.Log("Increasing dt");
                simManager.dt = simManager.dt * factor;
                break;
            case "fps":
                Debug.Log("Increasing fps");
                simManager.fps = simManager.fps * factor;
                break;
            case "scaleFactor":
                Debug.Log("Increasing zoom");
                simManager.scaleFactor = simManager.scaleFactor * factor;
                break;

            default:
                Debug.Log("ERROR: SHOULD NOT REACH THIS CODE");
                break;
        }
    }
    void setValue(string param, string value) {
        //Try to parse number, if valid, update value
        value = value.Replace(",", ".");
        double number;
        bool success = double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
        if (!success) {
            Debug.Log("INCORRECT FORMAT!");
            return;
        }
        Debug.Log("Success!! the value is: " + number);
        switch (param) {
            case "G":
                simManager.G = number;
                break;
            case "dt":
                simManager.dt = number;
                break;
            case "fps":
                simManager.fps = number;
                break;
            case "scaleFactor":
                simManager.scaleFactor = number;
                break;

            default:
                Debug.Log("ERROR: SHOULD NOT REACH THIS CODE");
                break;
        }
    }

    void updateInputFieldValues() {
        //Upload the increases
        if (GIncrease)
            changeValue("G", STREAM_INCREASE_FACTOR);
        if (DTIncrease)
            changeValue("dt", STREAM_INCREASE_FACTOR);
        if (FPSIncrease)
            changeValue("fps", STREAM_INCREASE_FACTOR);
        if (ZoomIncrease)
            changeValue("scaleFactor", STREAM_INCREASE_FACTOR);


        //Upload the decreases
        if (GDecrease)
            changeValue("G", STREAM_DECREASE_FACTOR);
        if (DTDecrease)
            changeValue("dt", STREAM_DECREASE_FACTOR);
        if (FPSDecrease)
            changeValue("fps", STREAM_DECREASE_FACTOR);
        if (ZoomDecrease)
            changeValue("scaleFactor", STREAM_DECREASE_FACTOR);
    }

    void updateInputFieldTexts() {
        if (EventSystem.current.currentSelectedGameObject == null) {
            GIF.text = simManager.G.ToString(CultureInfo.InvariantCulture);
            dtIF.text = simManager.dt.ToString(CultureInfo.InvariantCulture);
            fpsIF.text = simManager.fps.ToString(CultureInfo.InvariantCulture);
            zoomIF.text = simManager.scaleFactor.ToString(CultureInfo.InvariantCulture);
            return;
        }
        switch (EventSystem.current.currentSelectedGameObject.name) {
            case "Gif":
                dtIF.text = simManager.dt.ToString(CultureInfo.InvariantCulture);
                fpsIF.text = simManager.fps.ToString(CultureInfo.InvariantCulture);
                zoomIF.text = simManager.scaleFactor.ToString(CultureInfo.InvariantCulture);
                break;
            case "DTif":
                GIF.text = simManager.G.ToString(CultureInfo.InvariantCulture);
                fpsIF.text = simManager.fps.ToString(CultureInfo.InvariantCulture);
                zoomIF.text = simManager.scaleFactor.ToString(CultureInfo.InvariantCulture);
                break;

            case "FPSif":
                GIF.text = simManager.G.ToString(CultureInfo.InvariantCulture);
                dtIF.text = simManager.dt.ToString(CultureInfo.InvariantCulture);
                zoomIF.text = simManager.scaleFactor.ToString(CultureInfo.InvariantCulture);
                break;

            case "Zoomif":
                GIF.text = simManager.G.ToString(CultureInfo.InvariantCulture);
                dtIF.text = simManager.dt.ToString(CultureInfo.InvariantCulture);
                fpsIF.text = simManager.fps.ToString(CultureInfo.InvariantCulture);
                break;
            default:
                break;
        }
    }

    void updateStaticTexts() {
        simTimeText.text = simManager.simTime.ToString("E2") + T_UNIT + " - " + (simManager.simTime / 3.154e+7).ToString("0.0") + " years";
        realTimeText.text = simManager.realTime.ToString("E2") + T_UNIT;
        nFramesText.text = simManager.nExecutedFrames.ToString() + " (" + (1f / simManager.deltaTime).ToString("0.0") + ")";
        integratorText.text = simManager.integrator;
    }
       
    
}
