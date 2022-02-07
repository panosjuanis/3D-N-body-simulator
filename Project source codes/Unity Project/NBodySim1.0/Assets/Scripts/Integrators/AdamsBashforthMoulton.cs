using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamsBashforthMoulton : MonoBehaviour
{

    public static double[] adamsBashforthMoulton(double dt, double[] y0, double[] y1, double[] y2, double[] y3, double[] masses, double G) {
        double[] w4 = AdamsBashforth.adamsBashforth(dt, y0, y1, y2, y3, masses, G);
        w4 = AdamsMoulton.adamsMoulton(dt, y1, y2, y3, w4, masses, G);
        
        return w4;
    }

}
