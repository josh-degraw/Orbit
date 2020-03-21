using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus;

public class ImageSwap : MonoBehaviour
{
    public Sprite imageOne;
    public Sprite imageTwo;
    public GameObject diagramOne;
    public GameObject diagramTwo;

    public void Update()
    {
        if (Input.GetButton("Oculus_CrossPlatform_Tap") )
            Swap();
    }

    public void Swap()
    {
        Image currentImage = GetComponent<Image>();
            if(currentImage.sprite == imageOne)
        {
            currentImage.sprite = imageTwo;
        }
        else
        {
            currentImage.sprite = imageOne;
        }
    }
}
