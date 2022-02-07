using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Numerics;

public class EMGraphManager : MonoBehaviour
{
    public SimManager simManager;
    public Window_Graph graph;
    public Button stateButton;

    private double[] y;
    

    private List<double> KE = new List<double>();
    private List<double> PE = new List<double>();
    private List<double> E = new List<double>();
    private List<double> Erelative = new List<double>();
    private List<double> L = new List<double>();
    private List<double> Lrelative = new List<double>();

    //CANVAS STUFF
    public GameObject[] tags = new GameObject[4];

    void Start() {
        stateButton.onClick.AddListener(switchState);
    }

    public void updateGraph() {  
        updatePositions();
        calculateEnergy();
        calculateMomentum();
        testPrints();
        if (graph.mode != Window_Graph.HIDE && KE.Count > (graph.Ndots + 1))
            graph.ShowGraph(KE, PE, E, L, Erelative, Lrelative);
        else
            graph.disableGraph();
    }

    void calculateEnergy() {
        //Calculate KE
        var v = np.array(SimulatorUtils.getVelocities(y)).reshape(-1, 6)[":,:3"];
        var m = np.array(simManager.masses).copy().reshape(-1, 1);

        KE.Add((np.sum(m * np.power(v, 2), dtype: np.float64) / 2).GetData<double>()[0]);

        //Calculate PE
        var yAux = np.array(y).copy().reshape(-1, 6);
        var qx = yAux[":, 0"].reshape(-1, 1);
        var qy = yAux[":, 1"].reshape(-1, 1);
        var qz = yAux[":, 2"].reshape(-1, 1);


        var dx = qx.T - qx;
        var dy = qy.T - qy;
        var dz = qz.T - qz;

        
        var inv_r = np.sqrt(np.power(dx, 2) + np.power(dy, 2) + np.power(dz, 2));
        inv_r = invert_r(inv_r);


        PE.Add((simManager.G * np.sum(np.sum(triu((m * m.T) * inv_r), dtype: np.float64), dtype: np.float64)).GetData<double>()[0]);

        E.Add(KE[KE.Count - 1] + PE[PE.Count - 1]);

        Erelative.Add((E[0] - E[E.Count - 1]) / E[0]);
    }

    void calculateMomentum() {
        double[] Lsum = { 0.0, 0.0, 0.0 };
        for (int i = 0; i < simManager.nBodies; i++) {
            int ioffset = i * 6;
            double[] q = { y[ioffset + 0], y[ioffset + 1], y[ioffset + 2] };
            double[] v = { y[ioffset + 3], y[ioffset + 4], y[ioffset + 5] };
            double[] cross = new double[3];
            cross[0] = q[1] * v[2] - q[2] * v[1];
            cross[1] = q[2] * v[0] - q[0] * v[2];
            cross[2] = q[0] * v[1] - q[1] * v[0];
            Lsum = F.s(Lsum, F.m(simManager.masses[i], cross));
            //Debug.Log("Lsum = " + Lsum);
            
        }
        L.Add(Math.Sqrt(Math.Pow(Lsum[0], 2)+ Math.Pow(Lsum[1], 2)+ Math.Pow(Lsum[2], 2)));
        Lrelative.Add((L[0] - L[L.Count - 1]) / L[0]);
    }

    void updatePositions() {
        y = simManager.history[simManager.history.Count - 1];
    }

    void testPrints() {
        /*Debug.Log("KE IS: " + KE[KE.Count - 1]);
        Debug.Log("PE IS: " + PE[PE.Count - 1]);
        Debug.Log("E IS: " + E[E.Count - 1]);*/
    }

    NDArray invert_r(NDArray inv_r) {
        NDArray toReturn = inv_r.copy();

        for(int i = 0; i < inv_r.shape[0]; i++) {
            for(int j = 0; j < inv_r.shape[1]; j++) {
                if(inv_r[i, j].GetData<double>()[0] != 0)
                    toReturn[i, j] = 1f / inv_r[i, j];
            }
        }

        return toReturn;
    }

    NDArray triu(NDArray x) { //C # port of np.triu, for more info look up numpy

        NDArray toReturn = x.copy();

        for (int i = 0; i < x.shape[0]; i++) {
            for (int j = 0; j < x.shape[1]; j++) {
                if (i <= j)
                    toReturn[i, j] = 0;
            }
        }

        return toReturn;
    }

    void switchState() {
        for (int i = 0; i < 4; i++)
            tags[i].SetActive(false);
        switch (graph.mode) {
            case (Window_Graph.HIDE):
                stateButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "SHOW RELATIVE ENERGY";
                tags[Window_Graph.ENERGY - 1].SetActive(true);
                break;
            case (Window_Graph.ENERGY):
                stateButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "SHOW ANGULAR MOMENTUM";
                tags[Window_Graph.ENERGY_RELATIVE - 1].SetActive(true);
                break;
            case (Window_Graph.ENERGY_RELATIVE):
                stateButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "SHOW ANGULAR MOMENTUM RELATIVE";
                tags[Window_Graph.ANGULAR_MOMENTUM - 1].SetActive(true);
                break;
            case (Window_Graph.ANGULAR_MOMENTUM):
                stateButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "HIDE";
                tags[Window_Graph.ANGULAR_MOMENTUM_RELATIVE - 1].SetActive(true);
                break;
            case (Window_Graph.ANGULAR_MOMENTUM_RELATIVE):
                stateButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "SHOW ENERGY";
                
                break;
        }
        graph.mode++;
        if (graph.mode > Window_Graph.ANGULAR_MOMENTUM_RELATIVE)
            graph.mode = Window_Graph.HIDE;
    }



}
