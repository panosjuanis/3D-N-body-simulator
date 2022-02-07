using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleToggleColorUpdater : MonoBehaviour
{
    private Toggle toggle;
    private ColorBlock colors;

    private Color onColorNormalOrSelected;
    private Color onColorPressed;
    private Color onColorHighlighted;

    private Color offColorNormalOrSelected;
    private Color offColorPressed;
    private Color offColorHighlighted;

    void Start() {
        toggle = this.gameObject.GetComponent<Toggle>();

        onColorNormalOrSelected = new Color(0, 255, 0, 255);
        onColorHighlighted = new Color(0, 255, 0, 100);
        onColorPressed = new Color(0, 255, 0, 175);

        offColorNormalOrSelected = new Color(0, 0, 0, 0);
        offColorHighlighted = new Color(0, 255, 0, 100);
        offColorPressed = new Color(0, 255, 0, 175);

        toggle.onValueChanged.AddListener(delegate { changeValue(); });
    }

    private void changeValue() {
        colors = this.gameObject.GetComponent<Toggle>().colors;
        if (toggle.isOn) {
            colors.normalColor = onColorNormalOrSelected;
            colors.selectedColor = onColorNormalOrSelected;
            colors.highlightedColor = onColorHighlighted;
            colors.pressedColor = onColorPressed;

        }
        else {
            colors.normalColor = offColorNormalOrSelected;
            colors.selectedColor = offColorNormalOrSelected;
            colors.highlightedColor = offColorHighlighted;
            colors.pressedColor = offColorPressed;
        }
        this.gameObject.GetComponent<Toggle>().colors = colors;
    }
}
