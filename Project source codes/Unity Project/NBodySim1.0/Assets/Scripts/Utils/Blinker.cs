using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Blinker : MonoBehaviour
{
    //Color params
    public int r = 33;
    public int g = 255;
    public int b = 0;

    private float elapsedTime = 0;
    private float blinkTime = 2;
    private bool increasing = true;
    private float timeLeftActive = 0;

    public bool stayAlive = true;
    

    void Update()
    {

        if (increasing) {
            elapsedTime += Time.deltaTime;
        }
        else
            elapsedTime -= Time.deltaTime;

        if (elapsedTime > blinkTime) {
            increasing = false;
            elapsedTime = blinkTime - 0.01f; //offset so that when at the top value, mod is not 0
        }
        else if (elapsedTime < 0) {
            increasing = true;
            elapsedTime = 0;
        }
        timeLeftActive -= Time.deltaTime;

        this.gameObject.GetComponent<TextMeshProUGUI>().color = new Color32((byte)r, (byte)g, (byte)b, (byte)((elapsedTime % blinkTime)*255/2));

        if(stayAlive == false && timeLeftActive < 0) 
            this.gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 0); //Make text not visible

        
    }

    public void blink(int nTimes) {
        elapsedTime = 0; //To start text form 0 alpha
        timeLeftActive = blinkTime * nTimes;
    }
}
