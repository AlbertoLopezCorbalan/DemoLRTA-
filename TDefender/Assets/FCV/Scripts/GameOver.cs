using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{



    public Text gameOver;
    public Button repetir;

    // Start is called before the first frame update
    void Start()
    {
        gameOver.enabled = false;
        repetir.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {

        if (movimientoPersonaje.vida <= 0 || castillo.vidaCastillo <= 0)
        {

            gameOver.enabled = true;
            repetir.gameObject.SetActive(true);

        }
        else
        {
            gameOver.enabled = false;
            repetir.gameObject.SetActive(false);

        }
        
    }
}
