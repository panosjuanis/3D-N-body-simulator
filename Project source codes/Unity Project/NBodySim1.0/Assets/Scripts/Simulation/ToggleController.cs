using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public Toggle playToggle;
    public Toggle pauseToggle;

    void Update()
    {
        //Make toggle logic work with space
        if (Input.GetKeyDown(KeyCode.Space)) {
            pauseOrUnpause();
                
        }
        if (playToggle.isOn) {
            //Play is lit, pause is off
            var colors = playToggle.colors;
            colors.normalColor = new Color(0x00, 0xFF, 0x00, 0.5f);
            playToggle.colors = colors;

            colors = pauseToggle.colors;
            colors.normalColor = new Color(0xFF, 0x00, 0x00, 0);
            pauseToggle.colors = colors;
        }
        if(pauseToggle.isOn) {
            //Pause is lit, play is off
            var colors = pauseToggle.colors;
            colors.normalColor = new Color(0xFF, 0x00, 0x00, 0.5f);
            pauseToggle.colors = colors;

            colors = playToggle.colors;
            colors.normalColor = new Color(0x00, 0xFF, 0x00, 0.0f);
            playToggle.colors = colors;
        }
    }

    public void pauseOrUnpause() {
        if (playToggle.isOn) {
            playToggle.isOn = false;
            pauseToggle.isOn = true;
        }
        else {
            playToggle.isOn = true;
            pauseToggle.isOn = false;
        }
    }
}
