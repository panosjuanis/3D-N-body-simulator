using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using NumSharp;
using Random = UnityEngine.Random;

public class SimManager : MonoBehaviour {

    //Canvas objects
    public SimPanelManager panelManager;
    public Toggle playToggle;
    public Button resetButton;
    public EMGraphManager graphManager;

    //Sim data
    public GameObject[] bodies;
    public GameObject bodyPrefab;

    //Variables to import from simParameters
    public string integrator;
    public double G;
    public double dt;
    public double scaleFactor;
    public double fps;
    public int trailSize;
    public int nBodies;
    public double[] y0;
    public double[] masses;
    public double[] radii;
    public string[] names;
    public double[] lastPos;

    //Simulation variables
    private double tfps = 0;
    public double realTime = 0;
    public double simTime = 0;
    public double deltaTime = 1f;
    public int nExecutedFrames = 0;

    //History and trails
    public List<double[]> history = new List<double[]>();
    public List<Vector3[]> trails = new List<Vector3[]>();

    //More canvas stuff...
    public int selectedBodyIndex = 0;

    //Materials to color the spheres
    public Color[] bodyColors = new Color[]{Color.yellow, Color.magenta, Color.cyan, Color.green, Color.red, Color.blue};
    public Material selected;
    public Material unselected;


    //File to import sim from
    public static string selectedFilePath = "NOFILESELECTED";//"NOFILESELECTED";

    void Start() //SimManager.cs
    {
        importVariables();
        instantiateBodies(y0);
        setBodiesParameters();
        initHistory();
        initCanvas();
    }

    
    void Update()
    {
        if (playToggle.isOn) {
            realTime += Time.deltaTime;
            tfps += Time.deltaTime;
            if (tfps < 1 / fps) {
                return;
            }
            //Frame is executed
            nExecutedFrames++;
            simTime += dt;
            deltaTime = tfps;
            tfps = 0;

            nextIter(integrator);
            checkCollisions();
            updatePositions();
            updateTrails();
            updateMaterials();
            graphManager.updateGraph();
        }
    }


    void nextIter(string integrator/* = "Runge Kutta 4"*/) {
        //Hacer que todos los integradores tengan método add y dependiendo de cual sea crear un objeto integrador y hacer siempre add
        if (integrator.Equals("Runge Kutta 4")) {
            history.Add(RK4.rk4(dt, history[history.Count - 1], masses, G));
        }
        else if (integrator.Equals("LeapFrog")) {
            history.Add(LeapFrog.leapFrog(dt, history[history.Count - 1], masses, G));
        }
        else if (integrator.Equals("Euler")) {
            history.Add(Euler.euler(dt, history[history.Count - 1], masses, G));
        }
        else if (integrator.Equals("Adams Bashforth")) {
            history.Add(AdamsBashforth.adamsBashforth(dt, history[history.Count - 4], history[history.Count - 3], history[history.Count - 2], history[history.Count - 1], masses, G));
        }
        else if (integrator.Equals("Adams Bashforth Moulton")) {
            history.Add(AdamsBashforthMoulton.adamsBashforthMoulton(dt, history[history.Count - 4], history[history.Count - 3], history[history.Count - 2], history[history.Count - 1], masses, G));
        }
        else
            Debug.Log("NO INTEGRATOR SELECTED");

        
        if (history.Count > trailSize) {
            history.RemoveAt(0);
        }
        lastPos = history[history.Count - 1];

    }

    void checkCollisions() {
        //If collision happens, remove objects and create a new one
        double[] lastPos = history[history.Count - 1];
        for(int i = 0; i < nBodies; i++) {
            int ioffset = i * 6;
            for(int j = 0; j < nBodies; j++) {
                if(i != j) {
                    int joffset = j * 6;
                    double dist = Vector3.Distance(new Vector3((float)lastPos[ioffset], (float)lastPos[ioffset + 1], (float)lastPos[ioffset + 2]),
                                                   new Vector3((float)lastPos[joffset], (float)lastPos[joffset + 1], (float)lastPos[joffset + 3]));

                    if (dist < radii[i] + radii[j]) {//If distance is less than sum of radii -> collision occurs (only 1 collision can occur per frame)
                        Debug.Log("COLLISION DETECTED");
                        bodyCollision(i, j);
                        return;
                    }

                }
            }
        }

    }
    void bodyCollision(int i, int j) {
        //Debug stuff
        Debug.Log("old_radii = " + String.Join("", new List<double>(radii).ConvertAll(k => k.ToString() + " ").ToArray()));
        Debug.Log("old_masses = " + String.Join("", new List<double>(masses).ConvertAll(k => k.ToString() + " ").ToArray()));
        Debug.Log("old_y = " + String.Join("", new List<double>(history[history.Count - 1]).ConvertAll(k => k.ToString() + " ").ToArray()));
        //Debug stuff

        nBodies--;
        double[] auxMasses = masses;
        double[] auxRadii = radii;
        string[] auxNames = names;

        masses = new double[nBodies];
        radii = new double[nBodies];
        names = new string[nBodies];

        double[] auxY = new double[nBodies * 6];

        int index = 0;
        for (int k = 0; k < nBodies + 1; k++) {
            int indexoffset = index * 6;
            if (k != i && k != j) {
                masses[index] = auxMasses[k];
                radii[index] = auxRadii[k];
                names[index] = auxNames[k];
                auxY[indexoffset] = history[history.Count - 1][k * 6];
                auxY[indexoffset + 1] = history[history.Count - 1][k * 6 + 1];
                auxY[indexoffset + 2] = history[history.Count - 1][k * 6 + 2];
                auxY[indexoffset + 3] = history[history.Count - 1][k * 6 + 3];
                auxY[indexoffset + 4] = history[history.Count - 1][k * 6 + 4];
                auxY[indexoffset + 5] = history[history.Count - 1][k * 6 + 5];
                index++;
            }
        }
        //Set params of new body
        auxY[(index * 6)] = (history[history.Count - 1][i * 6] + history[history.Count - 1][j * 6]) / 2;
        auxY[(index * 6) + 1] = (history[history.Count - 1][i * 6 + 1] + history[history.Count - 1][j * 6 + 1]) / 2;
        auxY[(index * 6) + 2] = (history[history.Count - 1][i * 6 + 2] + history[history.Count - 1][j * 6 + 2]) / 2;
        auxY[(index * 6) + 3] = (auxMasses[i] * history[history.Count - 1][i * 6 + 3] + auxMasses[j] * history[history.Count - 1][j * 6 + 3]) / (auxMasses[i] + auxMasses[j]);
        auxY[(index * 6) + 4] = (auxMasses[i] * history[history.Count - 1][i * 6 + 4] + auxMasses[j] * history[history.Count - 1][j * 6 + 4]) / (auxMasses[i] + auxMasses[j]);
        auxY[(index * 6) + 5] = (auxMasses[i] * history[history.Count - 1][i * 6 + 5] + auxMasses[j] * history[history.Count - 1][j * 6 + 5]) / (auxMasses[i] + auxMasses[j]);
        masses[index] = auxMasses[i] + auxMasses[j];
        names[index] = auxNames[i] + auxNames[j];

        for (int z = 0; z < history.Count; z++) {
            index = 0;
            for (int k = 0; k < nBodies + 1; k++) {
                int indexoffset = index * 6;
                if (k != i && k != j) {
                    history[z][indexoffset] = history[z][k * 6];
                    history[z][indexoffset + 1] = history[z][k * 6 + 1];
                    history[z][indexoffset + 2] = history[z][k * 6 + 2];
                    history[z][indexoffset + 3] = history[z][k * 6 + 3];
                    history[z][indexoffset + 4] = history[z][k * 6 + 4];
                    history[z][indexoffset + 5] = history[z][k * 6 + 5];
                    index++;
                }
            }
            history[z][(index * 6)] = (history[history.Count - 1][i * 6] + history[history.Count - 1][j * 6]) / 2;
            history[z][(index * 6) + 1] = (history[history.Count - 1][i * 6 + 1] + history[history.Count - 1][j * 6 + 1]) / 2;
            history[z][(index * 6) + 2] = (history[history.Count - 1][i * 6 + 2] + history[history.Count - 1][j * 6 + 2]) / 2;
            history[z][(index * 6) + 3] = (auxMasses[i] * history[history.Count - 1][i * 6 + 3] + auxMasses[j] * history[history.Count - 1][j * 6 + 3]) / (auxMasses[i] + auxMasses[j]);
            history[z][(index * 6) + 4] = (auxMasses[i] * history[history.Count - 1][i * 6 + 4] + auxMasses[j] * history[history.Count - 1][j * 6 + 4]) / (auxMasses[i] + auxMasses[j]);
            history[z][(index * 6) + 5] = (auxMasses[i] * history[history.Count - 1][i * 6 + 5] + auxMasses[j] * history[history.Count - 1][j * 6 + 5]) / (auxMasses[i] + auxMasses[j]);
        }




        //Radius is calculated assuming equal density of bodies
        double volumei = Math.PI * (4.0 / 3.0) * Math.Pow(auxRadii[i], 3);
        double volumej = Math.PI * (4.0 / 3.0) * Math.Pow(auxRadii[j], 3);
        double volume = volumei + volumej;
        double radius = Math.Pow((3 * volume / (4.0 * Math.PI)), 1.0 / 3.0);
        radii[index] = radius;

        //Set new history
        Debug.Log("TODO -> add new history after collision");
        history.RemoveAt(history.Count - 1);
        history.Add(auxY);
        for (int z = 0; z < 3; z++)
            history.Add(RK4.rk4(dt, history[history.Count - 1], masses, G));

        destroyBodies();
        instantiateBodies(auxY);
        setBodiesParameters();
        panelManager.resetPanel();


        //Debug stuff
        Debug.Log("new_radii = " + String.Join("", new List<double>(radii).ConvertAll(k => k.ToString() + " ").ToArray()));
        Debug.Log("new_masses = " + String.Join("", new List<double>(masses).ConvertAll(k => k.ToString() + " ").ToArray()));
        Debug.Log("new_y = " + String.Join("", new List<double>(history[history.Count - 1]).ConvertAll(k => k.ToString() + " ").ToArray()));
        }
        /*
        void bodyCollision(int i, int j) {
            //Debug stuff
            Debug.Log("old_radii = " + String.Join("", new List<double>(radii).ConvertAll(k => k.ToString() + " ").ToArray()));
            Debug.Log("old_masses = " + String.Join("", new List<double>(masses).ConvertAll(k => k.ToString() + " ").ToArray()));
            Debug.Log("old_y = " + String.Join("", new List<double>(history[history.Count - 1]).ConvertAll(k => k.ToString() + " ").ToArray()));
            //Debug stuff

            nBodies--;
            double[] auxMasses = masses;
            double[] auxRadii = radii;
            string[] auxNames = names;

            masses = new double[nBodies];
            radii = new double[nBodies];
            names = new string[nBodies];

            double[] auxY = new double[nBodies * 6];

            int index = 0;
            for (int k = 0; k < nBodies + 1; k++) {
                int indexoffset = index * 6;
                if(k != i && k != j) {
                    masses[index] = auxMasses[k];
                    radii[index] = auxRadii[k];
                    names[index] = auxNames[k];
                    auxY[indexoffset] = history[history.Count - 1][k*6];
                    auxY[indexoffset+1] = history[history.Count - 1][k*6+1];
                    auxY[indexoffset+2] = history[history.Count - 1][k*6+2];
                    auxY[indexoffset+3] = history[history.Count - 1][k*6+3];
                    auxY[indexoffset+4] = history[history.Count - 1][k*6+4];
                    auxY[indexoffset+5] = history[history.Count - 1][k*6+5];
                    index++;
                }
            }
            //Set params of new body
            auxY[(index * 6)] = (history[history.Count - 1][i * 6] + history[history.Count - 1][j * 6]) / 2;
            auxY[(index * 6)+1] = (history[history.Count - 1][i * 6+1] + history[history.Count - 1][j * 6+1]) / 2;
            auxY[(index * 6)+2] = (history[history.Count - 1][i * 6+2] + history[history.Count - 1][j * 6+2]) / 2;
            auxY[(index * 6)+3] = (auxMasses[i] * history[history.Count - 1][i * 6+3] + auxMasses[j] * history[history.Count - 1][j * 6+3]) / (auxMasses[i] + auxMasses[j]);
            auxY[(index * 6)+4] = (auxMasses[i] * history[history.Count - 1][i * 6+4] + auxMasses[j] * history[history.Count - 1][j * 6+4]) / (auxMasses[i] + auxMasses[j]);
            auxY[(index * 6)+5] = (auxMasses[i] * history[history.Count - 1][i * 6+5] + auxMasses[j] * history[history.Count - 1][j * 6+5]) / (auxMasses[i] + auxMasses[j]);
            masses[index] = auxMasses[i] + auxMasses[j];
            names[index] = auxNames[i] + auxNames[j];


            //Radius is calculated assuming equal density of bodies
            double volumei = Math.PI * (4.0 / 3.0) * Math.Pow(auxRadii[i],3);
            double volumej = Math.PI * (4.0 / 3.0) * Math.Pow(auxRadii[j],3);
            double volume = volumei + volumej;
            double radius = Math.Pow((3 * volume / (4.0 * Math.PI)), 1.0 / 3.0);
            radii[index] = radius;

            //Set new history
            Debug.Log("TODO -> add new history after collision");
            history.Add(auxY);
            for(int z = 0; z < 3; z++)
                history.Add(RK4.rk4(dt, history[history.Count - 1], masses, G));

            destroyBodies();
            instantiateBodies(auxY);
            setBodiesParameters();
            panelManager.resetPanel();


            //Debug stuff
            Debug.Log("new_radii = " + String.Join("", new List<double>(radii).ConvertAll(k => k.ToString() + " ").ToArray()));
            Debug.Log("new_masses = " + String.Join("", new List<double>(masses).ConvertAll(k => k.ToString() + " ").ToArray()));
            Debug.Log("new_y = " + String.Join("", new List<double>(history[history.Count - 1]).ConvertAll(k => k.ToString() + " ").ToArray()));
            //Debug stuff
        }*/

        void destroyBodies() {
        for (int i = 0; i < bodies.Length; i++)
            Destroy(bodies[i]);
    }

    void updatePositions() {
        for(int i = 0; i < nBodies; i++) {
            double[] y = history[history.Count - 1];                 //Latest positions
            bodies[i].transform.position = new Vector3((float)y[i * 6], (float)y[i * 6 + 1], (float)y[i * 6 + 2]) * (1f / (float)scaleFactor);
        }
    }

    
    void updateTrails() {
        //TODO -> if trail size is variable, update the size unless it is equal to existing size
        if (history.Count <= trailSize) {
            for (int j = 0; j < nBodies; j++) {
                bodies[j].GetComponent<LineRenderer>().positionCount = history.Count;
            }
        }
        for(int i = 0; i < history.Count; i++) {
            for(int j = 0; j < nBodies; j++) {
                Vector3 qScaled = new Vector3((float)history[i][j * 6], (float)history[i][j * 6 + 1], (float)history[i][j * 6 + 2]) * (1f / (float)scaleFactor);
                bodies[j].GetComponent<LineRenderer>().SetPosition(i, qScaled);
            }
        }
        
    }
    

    void initHistory() {
        if(integrator.Equals("Runge Kutta 4") || integrator.Equals("LeapFrog") || integrator.Equals("Euler")) {
            history.Add(y0);
        }
        if(integrator.Equals("Adams Bashforth") || integrator.Equals("Adams Bashforth Moulton")) {
            history.Add(y0);
            for (int i = 0; i < 3; i++) {
                history.Add(RK4.rk4(dt, history[history.Count - 1], masses, G));
            }
                
        }
        //Debug.Log("Size of history after initHistory() = " + history.Count);


    }

    void instantiateBodies(double[] y0) {
        bodies = new GameObject[nBodies];
        for (int i = 0; i < nBodies; i++) {
            bodies[i] = Instantiate(bodyPrefab, new Vector3((float)y0[i * 6], (float)y0[i * 6 + 1], (float)y0[i * 6 + 2]), Quaternion.identity);

            // Set colors
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            if (i < bodyColors.Length)
                gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(bodyColors[i], 1.0f), new GradientColorKey(bodyColors[i], 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f), new GradientAlphaKey(alpha, 1.0f) }
                );
            else
                gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f), 1f), new GradientColorKey(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f), 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1f), new GradientAlphaKey(alpha, 1.0f) }
                );
            bodies[i].GetComponent<LineRenderer>().colorGradient = gradient;
        }
    }

    void setBodiesParameters() {
        for (int j = 0; j < nBodies; j++) {
            bodies[j].GetComponent<LineRenderer>().positionCount = history.Count;
        }

    }

    void setFPS(int fps) {
        Time.fixedDeltaTime = 1 / fps;
    }

    void initCanvas() {
        setupPanel();
        resetButton.onClick.AddListener(resetScene);
    }

    void resetScene() {
        SceneManager.LoadScene("Simulation");
    }

    public void updateMaterials() {
        for(int i = 0; i < nBodies; i++) {
            bodies[i].GetComponent<MeshRenderer>().material = unselected;
            if(i == selectedBodyIndex)
                bodies[i].GetComponent<MeshRenderer>().material = selected;
        }
    }

    private void importVariables() {
        string simParametersString = File.ReadAllText(selectedFilePath);
        SimParameters simParameters = JsonUtility.FromJson<SimParameters>(simParametersString);
        integrator = simParameters.integrator;
        G = simParameters.G;
        dt = simParameters.dt;
        scaleFactor = simParameters.scaleFactor;
        fps = simParameters.fps;
        trailSize = simParameters.trailSize;
        nBodies = simParameters.nBodies;
        y0 = F.arrayDeepCopy(simParameters.y0);
        masses = F.arrayDeepCopy(simParameters.masses);
        names = F.arrayDeepCopy(simParameters.names);
        radii = F.arrayDeepCopy(simParameters.radii);

        //Debug.Log("Body data read from file: ");
        foreach (double f in y0) Debug.Log(f);

    }

    private void setupPanel() {
        panelManager.setupPanel();
    }

    void debugStuff() {
        Debug.Log("Last history: " + np.array(history[history.Count - 1]));
    }

}
