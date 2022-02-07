using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuImageManager : MonoBehaviour
{
    public List<Sprite> backgroundImages;
    public float timeBetweenImages;
    public bool carousel = false;

    private float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenImages = 15;
        elapsedTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carousel) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > timeBetweenImages) {
                elapsedTime = 0;
                Sprite nextIm = backgroundImages[Random.Range(0, backgroundImages.Count)];
                this.gameObject.GetComponent<Image>().sprite = nextIm;
            }
        }
        
    }
}
