using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * If tab is pressed, changes to nextInputField
 * */
public class InputFieldTabulation : MonoBehaviour
{

    public InputField nextInputField;
    void Update()
    {
        if (this.GetComponent<InputField>().isFocused && Input.GetKeyDown(KeyCode.Tab))
            nextInputField.Select();
        
    }
}
