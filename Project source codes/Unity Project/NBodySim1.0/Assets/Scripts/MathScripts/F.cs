using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F
{
    //Calculates multiplication of array by scalar
    public static double[] m(double x, double[] y) {
        double[] s = new double[y.Length];
        for(int i = 0; i < y.Length; i++) {
            s[i] = y[i] * x;
        }
        return s;
    }

    //Calculates sum of list of arrays
    public static double[] s(List<double[]> l) {
        double[] t = new double[l[0].Length];
        for(int i = 0; i < t.Length; i++) {
            t[i] = 0;
        }
        for(int i = 0; i < l.Count; i++) {
            for(int j = 0; j < t.Length; j++) {
                t[j] += l[i][j];
            }
        }
        return t;
    }

    //Calculates sum of two arrays
    public static double[] s(double[] a, double[] b) {
        double[] c = new double[a.Length];
        for (int i = 0; i < a.Length; i++) {
            c[i] = a[i] + b[i];
        }
        return c;
    }

    public static double[] arrayDeepCopy(double[] origin) {
        double[] aux = new double[origin.Length];
        for (int i = 0; i < origin.Length; i++) 
            aux[i] = origin[i];
        
        return aux;
    }

    public static string[] arrayDeepCopy(string[] origin) {
        string[] aux = new string[origin.Length];
        for (int i = 0; i < origin.Length; i++)
            aux[i] = origin[i];

        return aux;
    }
}
