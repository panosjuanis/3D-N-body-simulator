using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System.IO;

public class NewSimPanelManager : MonoBehaviour
{

    //Main Canvas params
    public Button newBodyButton;
    public Button saveBodyButton;
    public Button backPopUpMenuButton;
    public GameObject bodyButtonPrefab;
    public GameObject popUpMenu;
    public List<GameObject> bodies = new List<GameObject>();
    public Blinker blinkText;

    //Body Canvas params
    public InputField bodyName;
    public InputField qx;
    public InputField qy;
    public InputField qz;
    public InputField vx;
    public InputField vy;
    public InputField vz;
    public InputField mass;
    public InputField radius;

    //Colors for body cells
    private Color selected = Color.green;
    private Color unselected = Color.cyan;

    private int selectedBodyIndex = -1;
    private bool editingBody = false;
    private int lastId = 0;

    //METHODS
    void Start()
    {
        //We start with the body creation menu disabled
        popUpMenu.SetActive(false);

        //Set up button listeners
        newBodyButton.onClick.AddListener(showPopUpMenu);
        saveBodyButton.onClick.AddListener(saveNewBody);
        backPopUpMenuButton.onClick.AddListener(hidePopUpMenu);
    }

    void saveNewBody() {
        Debug.Log("SAVING BODY");
        //First we check whether the input is valid, if it is we either edit or create the body
        if (!inputFieldsEmpty()) {
            Debug.Log("ALL FIELDS HAVE A VALUE");
            if (editingBody) {
                Debug.Log("SelectedBodyIndex: " + selectedBodyIndex);
                GameObject cell = bodies[selectedBodyIndex];
                setBodyDataParams(cell);
                editingBody = false;
            }
            else {
                //Create new body and add it to the panel
                GameObject cell = Instantiate(bodyButtonPrefab);
                setBodyDataParams(cell);
                cell.transform.SetParent(this.gameObject.transform, false);
                cell.transform.Find("BodyName").GetComponent<TextMeshProUGUI>().SetText(bodyName.text);
                cell.transform.Find("Id").GetComponent<Text>().text = lastId.ToString();
                cell.GetComponent<Button>().onClick.AddListener(() => setBodyCanvasParams(int.Parse(cell.transform.Find("Id").GetComponent<Text>().text)));
                lastId++;
                bodies.Add(cell);
            }

            //After adding or updating the body, hide pop up menu and clear body input fields
            clearBodyInputFields();
            hidePopUpMenu();
        }
        else {
            Debug.Log("ALL FIELDS NEED TO HAVE A VALUE!");
            blinkText.blink(4);
        }

            
    }

    //If we are in edit mode, we load the bodies
    public void loadBodies(SimParameters sp) {
        for(int i = 0; i < sp.nBodies; i++) {
            GameObject cell = Instantiate(bodyButtonPrefab);
            cell.GetComponent<BodyData>().setValues(sp.names[i], sp.y0[i*6], sp.y0[i*6+1], sp.y0[i*6+2], sp.y0[i * 6 + 3], sp.y0[i * 6 + 4], sp.y0[i * 6 + 5], sp.masses[i], sp.radii[i]);
            cell.transform.SetParent(this.gameObject.transform, false);
            cell.transform.Find("BodyName").GetComponent<TextMeshProUGUI>().SetText(sp.names[i]);
            cell.transform.Find("Id").GetComponent<Text>().text = lastId.ToString();
            cell.GetComponent<Button>().onClick.AddListener(() => setBodyCanvasParams(int.Parse(cell.transform.Find("Id").GetComponent<Text>().text)));
            lastId++;
            bodies.Add(cell);
        }
    
    }

    public bool hasBodies() {
        return bodies.Count > 0;
    }

    void hidePopUpMenu() {
        popUpMenu.SetActive(false);
    }

    void showPopUpMenu() {
        popUpMenu.SetActive(true);
    }

    void setBodyCanvasParams(int bodyIndex) {
        selectedBodyIndex = bodyIndex;

        //Since we are editing the body, we need to set editingBodyParam to true so no body is created when clicking 'Save body' button
        editingBody = true;
        showPopUpMenu();

        bodyName.text = bodies[selectedBodyIndex].GetComponent<BodyData>().bodyName;

        qx.text = bodies[selectedBodyIndex].GetComponent<BodyData>().qx.ToString(CultureInfo.InvariantCulture);
        qy.text = bodies[selectedBodyIndex].GetComponent<BodyData>().qy.ToString(CultureInfo.InvariantCulture);
        qz.text = bodies[selectedBodyIndex].GetComponent<BodyData>().qz.ToString(CultureInfo.InvariantCulture);

        vx.text = bodies[selectedBodyIndex].GetComponent<BodyData>().vx.ToString(CultureInfo.InvariantCulture);
        vy.text = bodies[selectedBodyIndex].GetComponent<BodyData>().vy.ToString(CultureInfo.InvariantCulture);
        vz.text = bodies[selectedBodyIndex].GetComponent<BodyData>().vz.ToString(CultureInfo.InvariantCulture);

        mass.text = bodies[selectedBodyIndex].GetComponent<BodyData>().mass.ToString(CultureInfo.InvariantCulture);
        radius.text = bodies[selectedBodyIndex].GetComponent<BodyData>().radius.ToString(CultureInfo.InvariantCulture);
    }

    void setBodyDataParams(GameObject cell) { 
        //Since a lot of the users will be spanish, we replace the commas for dots (',' -> '.')
        qx.text = qx.text.Replace(',', '.');
        qy.text = qy.text.Replace(',', '.');
        qz.text = qz.text.Replace(',', '.');
        vx.text = vx.text.Replace(',', '.');
        vy.text = vy.text.Replace(',', '.');
        vz.text = vz.text.Replace(',', '.');
        mass.text = mass.text.Replace(',', '.');
        radius.text = radius.text.Replace(',', '.');

        //TODO -> check that inputs are valid (tryParse all)
        cell.GetComponent<BodyData>().setValues( bodyName.text,
                                                double.Parse(qx.text, CultureInfo.InvariantCulture),
                                                double.Parse(qy.text, CultureInfo.InvariantCulture),
                                                double.Parse(qz.text, CultureInfo.InvariantCulture),
                                                double.Parse(vx.text, CultureInfo.InvariantCulture),
                                                double.Parse(vy.text, CultureInfo.InvariantCulture),
                                                double.Parse(vz.text, CultureInfo.InvariantCulture),
                                                double.Parse(mass.text, CultureInfo.InvariantCulture),
                                                double.Parse(radius.text, CultureInfo.InvariantCulture));
    }

    bool inputFieldsEmpty() {
        //TODO -> give info to user for which fields need value
        return     bodyName.text == ""
                || qx.text == ""
                || qy.text == ""
                || qz.text == ""
                || vx.text == ""
                || vy.text == ""
                || vz.text == ""
                || mass.text == ""
                || radius.text == "";
    }



    //TODO -> delete these unused methods?
    void updateButtonColors() {
        for (int i = 0; i < bodies.Count; i++) {
            if (i == selectedBodyIndex)
                bodies[i].GetComponent<Image>().color = selected;
            else
                bodies[i].GetComponent<Image>().color = unselected;
        }
    }

    void clearBodyInputFields() {
        bodyName.text = "";
        qx.text = "";
        qy.text = "";
        qz.text = "";
        vx.text = "";
        vy.text = "";
        vz.text = "";
        mass.text = "";
        radius.text = "";
}

}
