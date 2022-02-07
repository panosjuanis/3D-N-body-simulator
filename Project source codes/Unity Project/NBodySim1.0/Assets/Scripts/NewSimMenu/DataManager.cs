using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public NewSimPanelManager newSimPanelManager;


    //Canvas input fields
    public InputField G;
    public InputField dt;
    public InputField scaleFactor;
    public InputField FPS;
    public InputField trailSize;
    public InputField simName;

    public Dropdown integrator;

    


    public void saveSimulationParameters(bool editingMode) {
        //If we were editing the simulation, we delete the old file 
        if (editingMode) {
            if (File.Exists(SimManager.selectedFilePath)) {
                File.Delete(SimManager.selectedFilePath);
                Debug.Log("File deleted.");
            }
            else   //This should never happen
                Debug.Log("File not found");
        }
        //We remove '.' and '/' so saving the file works properly
        simName.text = simName.text.Replace('.', '_');
        simName.text = simName.text.Replace('/', '_');
        string path = Application.streamingAssetsPath + "/Examples/" + simName.text + ".json";
        SimManager.selectedFilePath = path;

        SimParameters sp = createSimParametersObject();
        saveToJson(sp, path);
    }

    private SimParameters createSimParametersObject() {
        SimParameters sp = new SimParameters();

        sp.integrator = integrator.options[integrator.value].text;

        //Since a lot of the users will be spanish, we replace the commas for dots (',' -> '.')
        //TODO -> check that the input is correct (no strings, random chars...)
        G.text           = G.text.Replace(',', '.');
        dt.text          = dt.text.Replace(',', '.');
        scaleFactor.text = scaleFactor.text.Replace(',', '.');
        FPS.text         = FPS.text.Replace(',', '.');

        sp.G           =-Math.Abs(double.Parse(G.text, CultureInfo.InvariantCulture));
        sp.dt          = Math.Abs(double.Parse(dt.text, CultureInfo.InvariantCulture));
        sp.scaleFactor = calculateScaleFactor();
        sp.fps         = Math.Abs(double.Parse(FPS.text, CultureInfo.InvariantCulture));

        sp.trailSize   = Math.Abs(int.Parse(trailSize.text, CultureInfo.InvariantCulture));
        sp.nBodies     = newSimPanelManager.bodies.Count;

        sp.y0     = new double[sp.nBodies * 6];
        sp.masses = new double[sp.nBodies];
        sp.radii = new double[sp.nBodies];
        sp.names  = new string[sp.nBodies];

        List<GameObject> bodies = newSimPanelManager.bodies;
        for(int i = 0; i < sp.nBodies; i++) {
            BodyData body = bodies[i].GetComponent<BodyData>();

            sp.names[i] = body.bodyName;
            sp.masses[i] = body.mass;
            sp.radii[i] = body.radius;

            sp.y0[i * 6 + 0] = body.qx;
            sp.y0[i * 6 + 1] = body.qy;
            sp.y0[i * 6 + 2] = body.qz;

            sp.y0[i * 6 + 3] = body.vx;
            sp.y0[i * 6 + 4] = body.vy;
            sp.y0[i * 6 + 5] = body.vz;

        }
        return sp;
    }

    private void saveToJson(SimParameters sp, string path) {
        string spString = JsonUtility.ToJson(sp);
        System.IO.File.WriteAllText(path, spString);
        Debug.Log("Saving simulation: " + simName.text + " to path: " + path);
    }

    private double calculateScaleFactor() {
        double maxDist = 0;

        foreach(GameObject go1 in newSimPanelManager.bodies) {
            BodyData b1 = go1.GetComponent<BodyData>();
            foreach (GameObject go2 in newSimPanelManager.bodies) {
                BodyData b2 = go2.GetComponent<BodyData>();

                double dist = Vector3.Distance(new Vector3((float)b1.qx, (float)b1.qy, (float)b1.qz), new Vector3((float)b2.qx, (float)b2.qy, (float)b2.qz));
                if (dist > maxDist)
                    maxDist = dist;

            }
        }
        Debug.Log("The maximum distance is: " + maxDist);
        return maxDist/20;
    }

}
