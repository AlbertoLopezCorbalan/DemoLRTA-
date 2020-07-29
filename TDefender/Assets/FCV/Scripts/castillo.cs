using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class castillo : MonoBehaviour
{

    public static int vidaCastillo;

    public Slider barraVidaCastillo;

    // Start is called before the first frame update
    void Start()
    {
        vidaCastillo = 100;

    }

    // Update is called once per frame
    void Update()
    {

        barraVidaCastillo.value = vidaCastillo;
        if (vidaCastillo <= 0)
        {

            //TODO Game Over
        }

  

    }
}
