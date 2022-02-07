using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeapFrog 
{
    public static double[] leapFrog(double dt, double[] y, double[] masses, double G) {
        double[] y_new = F.s(y, F.m(dt/2.0, getAcceleration(y, masses, G)));

        y_new = F.s(y_new, F.m(dt, SimulatorUtils.getVelocities(y_new)));

        y_new = F.s(y_new, F.m(dt / 2.0, getAcceleration(y_new, masses, G)));

        Debug.Log("y = " + String.Join("",new List<double>(y).ConvertAll(i => i.ToString()).ToArray()));
        Debug.Log("y_new = " + String.Join("", new List<double>(y_new).ConvertAll(i => i.ToString()).ToArray()));
        return y_new;
    }


    public static double[] getAcceleration(double[] y, double[] masses, double G) {
        int nBodies = y.Length / 6;
        double[] accelerations = new double[y.Length];
        double[] d = new double[3];
        double[] a = new double[3];

        for (int i = 0; i < nBodies; i++) {
            int ioffset = i * 6;
            for (int k = ioffset; k < ioffset + 3; k++) {
                accelerations[k] = 0; //y[k + 3];
            }

            for (int j = 0; j < nBodies; j++) {
                //accelerations[ioffset + j] = 0;
                int joffset = j * 6;

                if (i != j) {
                    for (int k = 0; k < 3; k++) {
                        d[k] = y[ioffset + k] - y[joffset + k];
                    }
                    double r = Math.Sqrt(Math.Pow(d[0], 2) + Math.Pow(d[1], 2) + Math.Pow(d[2], 2));
                    for (int k = 0; k < 3; k++) {
                        a[k] = (double)(d[k] * G * masses[j] / Math.Pow(r, 3));
                    }
                    for (int k = 0; k < 3; k++) {
                        accelerations[ioffset + k + 3] += a[k];
                    }
                }
            }
        }
        return accelerations;
    }

}
