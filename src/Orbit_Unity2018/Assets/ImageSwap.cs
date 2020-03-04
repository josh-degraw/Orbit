using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSwap : MonoBehaviour
{
    public Sprite imageOne;
    public Sprite imageTwo;

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
