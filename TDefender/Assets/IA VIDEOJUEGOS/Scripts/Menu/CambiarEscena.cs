using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void cambiarEscena(int i)
    {
        SceneManager.LoadScene(i, LoadSceneMode.Single);
        Time.timeScale = 1; 	//que la velocidad del juego sea 0
    }


    public void volverMenu()
    {

        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Time.timeScale = 1; 	//que la velocidad del juego sea 0
    }

    public void salir()
    {
        Application.Quit();
    }

}
