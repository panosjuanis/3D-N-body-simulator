using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Euler {
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

    public static double[] euler(double dt, double[] y, double[] masses, double G) {
        double[] k = F.m(dt, evaluate(y, masses, G));


        double[] y_new = F.s(y, k);
        return y_new;
    }
}

