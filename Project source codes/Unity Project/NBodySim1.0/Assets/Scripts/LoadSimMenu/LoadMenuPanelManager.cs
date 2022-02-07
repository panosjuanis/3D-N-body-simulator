using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LoadMenuPanelManager : MonoBehaviour
{
    public GameObject cellPrefab;
    private List<string> files;
    private List<GameObject> cells;
    public static string path;

    void Start()
    {
        loadPanel();
    }

    public void loadPanel() {
        readFiles();
        generateCells();
    }

    private void readFiles() {
        //Add all json files to a list and sort it
        files = new List<string>();
        path = Application.streamingAssetsPath + "/Examples/";
        foreach (string file in System.IO.Directory.GetFiles(path)) {
            if (file.EndsWith(".json"))
                files.Add(file);

        }
        files.Sort();
    }

    private void generateCells() {
        //If cells != null -> it is being called after edit/deletion of file -> 
        //we must delete the old cells and re create them
        if(cells != null) {
            foreach (GameObject cell in cells)
                Destroy(cell);
        }

        cells = new List<GameObject>();
        foreach (string file in files) {
            GameObject cell = Instantiate(cellPrefab);
            cell.transform.SetParent(this.gameObject.transform, false);
            cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Path.GetFileNameWithoutExtension(file);
            cell.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(file));
            cells.Add(cell);
        }

        //Set the first element of the list at the top
        this.gameObject.GetComponent<RectTransform>().position = new Vector3(981.7f, -50000, 0);
    }

    void ButtonClicked(string path)
    {
        SimManager.selectedFilePath = path;
        Debug.Log(path);
    }
}
