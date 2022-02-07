using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimPanelManager : MonoBehaviour
{
    public SimManager simManager;
    public GameObject cellPrefab;
    public CanvasManager canvasManager;


    public void setupPanel() {
        
        for (int i = 0; i < simManager.nBodies; i++) {
            GameObject cell = Instantiate(cellPrefab);
            cell.transform.SetParent(this.gameObject.transform, false);
            cell.transform.Find("name").GetComponent<TextMeshProUGUI>().text = simManager.names[i];
            cell.transform.Find("name").GetComponent<TextMeshProUGUI>().color = simManager.bodies[i].GetComponent<LineRenderer>().colorGradient.colorKeys[0].color;
            cell.transform.Find("id").GetComponent<Text>().text = i.ToString();
            cell.transform.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(cell.transform.Find("id").GetComponent<Text>().text));
        }
        //Set the first element of the list at the top
        this.gameObject.GetComponent<RectTransform>().position = new Vector3(1735, -100, 0);
    }

    public void resetPanel() {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        setupPanel();
    }

    void ButtonClicked(string id) {
        simManager.selectedBodyIndex = int.Parse(id);
    }

    void CreateNewBody() {
        Debug.Log("CREATE NEW BODY SELECTED, NOT IMPLEMENTED");
    }

    
}
