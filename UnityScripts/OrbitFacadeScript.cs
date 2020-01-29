using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Orbit;

public class OrbitFacadeScript : MonoBehaviour
{
    private SystemFacade Facade;

    // Start is called before the first frame update
    void Start()
    {
        print("Orbit started...");
        StartCoroutine(Timer());

        Facade = new SystemFacade();
        Facade.StartService();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Timer()
    {
        int minutes = 0;
        int seconds = 0;

        int interval = 5;
        int int_counter = 0;

        while (true)
        {
            seconds++;

            //Add a minute.
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }
            //Manage the interval
            int_counter++;
            if (interval == int_counter)
            {
                //Insert Code here to do regularly...

                print("Checking...");
                print("Urine Tank Level: " + Facade.UrineTank.UrineTankLevel);
                //print("Waste Tank Level: " + Facade.WasteWater.Level);
                //print("Potable Tank Level: " + Facade.PotableWaterTank.ProductTankLevel);

                //Reset the counter...
                int_counter = 0;
            }

                //Wait 1 second...
                yield return new WaitForSeconds(1);
        }
    }
}
