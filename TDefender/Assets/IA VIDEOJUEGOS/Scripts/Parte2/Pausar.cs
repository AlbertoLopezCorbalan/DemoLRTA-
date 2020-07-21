using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pausar : MonoBehaviour
{


    public Text Pause;
    // Start is called before the first frame update
    void Start()
    {
        Pause.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {    //si la velocidad es 1
                Pause.enabled = true;
                Time.timeScale = 0;     //que la velocidad del juego sea 0
            }
            else if (Time.timeScale == 0)
            {   // si la velocidad es 0
                Pause.enabled = false;
                Time.timeScale = 1;     // que la velocidad del juego regrese a 1
            }
        }


    }
}
