using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyData : MonoBehaviour
{
    public string bodyName;
    public double mass, radius, qx, qy, qz, vx, vy, vz;

    public void setValues(string name, double qx, double qy, double qz, double vx, double vy, double vz, double mass, double radius) {
        this.bodyName = name;
        this.qx = qx;
        this.qy = qy;
        this.qz = qz;

        this.vx = vx;
        this.vy = vy;
        this.vz = vz;

        this.mass = mass;
        this.radius = radius;
    }

    public void setValues(BodyData bd) {
        this.bodyName = bd.bodyName;

        this.qx = bd.qx;
        this.qy = bd.qy;
        this.qz = bd.qz;

        this.vx = bd.vx;
        this.vy = bd.vy;
        this.vz = bd.vz;

        this.mass = bd.mass;
        this.radius = bd.radius;
    }

    
}
