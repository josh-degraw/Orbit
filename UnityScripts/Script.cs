using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTestLibrary;

using Orbit;


public class Script : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        print("Script started...");
        StartCoroutine(Timer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Timer()
    {
        int minutes = 0;
        int seconds = 0;

        while(true)
        {
            seconds++;

            //Add a minute.
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }

            //Print the time
            string output = "Minutes: " + minutes + " seconds: " + seconds;
            //print(output);
            this.gameObject.GetComponent<UnityEngine.UI.Text>().text = output;
            yield return new WaitForSeconds(1);
        }
        
    }

}
