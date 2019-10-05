﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTutorialTrigger : MonoBehaviour
{
    public GameObject gunTutorial;
    public bool messageActive;
    private PlayerController tvPlayer;
    public bool triggered =false;

    public GameObject entirePlayer;
    // Start is called before the first frame update
    void Start()
    {
        tvPlayer = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (messageActive)
        {
            entirePlayer.gameObject.SetActive(false);
            gunTutorial.gameObject.SetActive(true);

             if (Input.GetButtonDown("Fire"))
            {
                gunTutorial.gameObject.SetActive(false);
                messageActive = false;
                entirePlayer.gameObject.SetActive(true);

            }
        }

        //if(tvPlayer.hasGun ==1 && !triggered)
        //{
        //    gunTutorial.gameObject.SetActive(true);
        //    messageActive = true;
        //    triggered = true;
        //}

    }

}