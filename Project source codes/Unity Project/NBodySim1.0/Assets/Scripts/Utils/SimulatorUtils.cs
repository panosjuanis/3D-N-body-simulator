using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;

public class SimulatorUtils : MonoBehaviour
{
    public static double[] getVelocities(double[] y) { //Can get called more than once per frame -> fix?
        int nBodies = y.Length / 6;
        double[] velocities = new double[y.Length];

        for (int i = 0; i < velocities.Length; i++)
            velocities[i] = 0;

        for (int i = 0; i < nBodies; i++) {
            int ioffset = i * 6;

            velocities[ioffset + 0] = y[ioffset + 3];
            velocities[ioffset + 1] = y[ioffset + 4];
            velocities[ioffset + 2] = y[ioffset + 5];
        }

        return velocities;
    }
    
}
