using System;

[Serializable]
public class SimParameters
{

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

}
