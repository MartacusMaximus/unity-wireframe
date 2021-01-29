using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorKey : MonoBehaviour
{
    public GameObject door;
    public GameObject thePlayer;
    private GameObject key;
    private bool keyPickup = false;

    private void Update()
    {
        key = GameObject.Find("Key");
        if (key == null)
        {
            keyPickup = true;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (keyPickup == true)
        {
            door.SetActive(false);
        }

        if (keyPickup == false || key != null)
        {
            key.SetActive(false);
        }

       
    }
}
