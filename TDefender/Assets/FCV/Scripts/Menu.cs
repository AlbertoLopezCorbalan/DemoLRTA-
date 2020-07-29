using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Manejo de escenas unity

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField]
    GameObject panelPrincipal, panelAyuda;



    public void jugar(int i)
    {

        SceneManager.LoadScene(i, LoadSceneMode.Single);



    }

    public void ayuda()
    {
        panelPrincipal.SetActive(false);
        panelAyuda.SetActive(true);


    }

    public void salir()
    {

        Application.Quit();

    }

    public void salirAyuda()
    {
        panelPrincipal.SetActive(true);
        panelAyuda.SetActive(false);


    }




}
