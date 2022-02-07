using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AdamsBashforth {
    public static double[] evaluate(double[] y, double[] masses, double G) {
        int nBodies = y.Length / 6;
        double[] solved_vector = new double[y.Length];
        double[] d = new double[3];
        double[] a = new double[3];

        for (int i = 0; i < nBodies; i++) {
            int ioffset = i * 6;
            for (int k = ioffset; k < ioffset + 3; k++) {
                solved_vector[k] = y[k + 3];
            }

            for (int j = 0; j < nBodies; j++) {
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
                        solved_vector[ioffset + k + 3] += a[k];
                    }
                }
            }
        }
        return solved_vector;
    }

    public static double[] adamsBashforth(double dt, double[] y0, double[] y1, double[] y2, double[] y3, double[] masses, double G) {
        double[] w0 = F.m(-9f,evaluate(y0, masses, G));
        double[] w1 = F.m(37f, evaluate(y1, masses, G));
        double[] w2 = F.m(-59f, evaluate(y2, masses, G));
        double[] w3 = F.m(55f, evaluate(y3, masses, G));
        Debug.Log("w0 = " + w0[0] + " " + w0[1]+ " " + w0[2]);

        double[] w4 = F.s(y3, F.m(dt/24f, F.s(new List<double[]>() { w3, w2, w1 , w0})));
        Debug.Log("w4 = " + w4[0] + " " + w4[1] + " " + w4[2]);
        return w4;
    }

} 
