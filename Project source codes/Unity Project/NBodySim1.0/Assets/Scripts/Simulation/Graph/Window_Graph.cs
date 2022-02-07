using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using CodeMonkey.Utils;
using System;

public class Window_Graph : MonoBehaviour
{
    [SerializeField]
    private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private GameObject background;
    /*private GameObject energyTags;
    private GameObject angularTags;*/
    private GameObject myCircle;

    


    public int mode = ENERGY;
    public int Ndots = 10;
    private int nLabelsY = 10;
    private int siblingIndex = 1;

    private Color KEColor = Color.blue;
    private Color PEColor = Color.green;
    private Color EColor = Color.white;
    private Color LColor = Color.yellow;

    public const int HIDE = 0;
    public const int ENERGY = 1;
    public const int ENERGY_RELATIVE = 2;
    public const int ANGULAR_MOMENTUM = 3;
    public const int ANGULAR_MOMENTUM_RELATIVE = 4;

    private float graphHeight;
    double yMaximum;
    double yMin;
    double yDiff;
    float xSize;

    void Start()
    {
        //Set variable values
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        background = transform.Find("Background").gameObject;
        myCircle = graphContainer.Find("myCircle").gameObject;
        /*energyTags = transform.Find("energyTags").gameObject;
        angularTags = transform.Find("angularTags").gameObject;*/


        background.GetComponent<RectTransform>().SetSiblingIndex(0);

        graphHeight = graphContainer.sizeDelta.y;
        xSize = graphContainer.sizeDelta.x / Ndots;
    }

    public void ShowGraph(List<double> KE, List<double> PE, List<double> E, List<double> L, List<double> Erelative, List<double> Lrelative) {
        deleteOldGraph();
        enableGraph();
        setMaxMin(KE, PE, E, L, Erelative, Lrelative);

        siblingIndex = 1;
        switch (mode) {
            case ENERGY:
                createLine(KE, KEColor);
                createLine(PE, PEColor);
                createLine(E, EColor);
                createXLabels(E);
                break;

            case ENERGY_RELATIVE:
                createLine(Erelative, EColor);
                createXLabels(Erelative);
                break;

            case ANGULAR_MOMENTUM:
                createLine(L, LColor);
                createXLabels(L);
                break;

            case ANGULAR_MOMENTUM_RELATIVE:
                createLine(Lrelative, LColor);
                createXLabels(Lrelative);
                break;

            default:
                Debug.Log("UNREACHEABLE CODE!");
                break;
        }

        
        
        //Create labels
        for(int i = 0; i <= nLabelsY; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.tag = "graphToDelete";
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-50f, i*graphHeight/Ndots);
            float normalizedValue = i *1f/ nLabelsY;
            labelY.GetComponent<Text>().text = (normalizedValue*yDiff - Math.Abs(yMin)).ToString("E2");
        }
    }

    void createLine(List<double> valueList, Color linkColor) {
        //Create circle point
        GameObject lastCircle = null;
        for (int i = 0; i <= Ndots; i++) {
            //set x and y positions
            float xPosition = i * xSize;
            float yPosition = (float)((valueList[(int)((i*1f/Ndots)*(valueList.Count-1))] + Math.Abs(yMin)) / (Math.Abs(yMin) + yMaximum));
            GameObject newCircle = CreateCircle(new Vector2(xPosition, yPosition * graphHeight));
            if (lastCircle != null)
                CreateDotConnection(lastCircle.GetComponent<RectTransform>().anchoredPosition, newCircle.GetComponent<RectTransform>().anchoredPosition, linkColor);
            lastCircle = newCircle;
        }
    }

    void createXLabels(List<double> valueList) {
        for (int i = 0; i <= Ndots; i++) {
            float xPosition = i * xSize;
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.tag = "graphToDelete";
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -20f);
            labelX.GetComponent<Text>().text = ((int)((i * 1f / Ndots) * valueList.Count)).ToString();
        }
    }

    void deleteOldGraph() {
        foreach (GameObject toDelete in GameObject.FindGameObjectsWithTag("graphToDelete")) {
            Destroy(toDelete);
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {

        GameObject circle = new GameObject("circle", typeof(Image));
        circle.tag = "graphToDelete";
        circle.transform.SetParent(graphContainer, false);
        circle.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = circle.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return circle;
    }


    //Connects two points in the graph with the specified color
    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color linkColor) {
        GameObject connection = new GameObject("dotConnection", typeof(Image));
        connection.tag = "graphToDelete";
        connection.transform.SetParent(graphContainer, false);
        connection.GetComponent<Image>().color = linkColor;
        RectTransform rectTransform = connection.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);    //Thickness of the line graph
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVector(dir));
        rectTransform.SetSiblingIndex(siblingIndex);
        siblingIndex++;

    }

    private void setMaxMin(List<double> KE, List<double> PE, List<double> E, List<double> L, List<double> Erelative, List<double> Lrelative) {
        switch (mode) {
            case ENERGY:
                yMaximum = KE.Max();
                if(PE.Max() > yMaximum)
                    yMaximum = PE.Max();
                

                yMin = KE.Min();
                if (PE.Min() < yMin) 
                    yMin = PE.Min();
                
                break;

            case ENERGY_RELATIVE:
                yMaximum = Erelative.Max();
                yMin = Erelative.Min();
                break;

            case ANGULAR_MOMENTUM:
                yMaximum = L.Max();
                yMin = L.Min();
                break;

            case ANGULAR_MOMENTUM_RELATIVE:
                yMaximum = Lrelative.Max();
                yMin = Lrelative.Min();
                break;

            default:
                Debug.Log("UNREACHEABLE CODE!");
                break;
        }
        Debug.Log("yMax value is: " + yMaximum);
        Debug.Log("yMin value is: " + yMin);
        Debug.Log("yDiff value is: " + yDiff);
        yDiff = yMaximum - yMin;
    }

    public void disableGraph() {
        background.SetActive(false);
        graphContainer.gameObject.SetActive(false);
    }

    private void enableGraph() {
        background.SetActive(true);
        graphContainer.gameObject.SetActive(true);
    }
}
