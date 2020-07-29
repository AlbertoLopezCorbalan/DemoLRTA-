using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiguienteNivel : MonoBehaviour
{

    public Text siguienteNivel;
    public Button continuar;

    [SerializeField]
    int numEnemigos;

    // Start is called before the first frame update
    void Start()
    {
        siguienteNivel.enabled = false;
        continuar.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if (enemySpawn.enemigosGlobales >= numEnemigos && movimientoPersonaje.vida > 0 && castillo.vidaCastillo > 0)
        {

            siguienteNivel.enabled = true;
            continuar.gameObject.SetActive(true);

        }
        else
        {
            siguienteNivel.enabled = false;
            continuar.gameObject.SetActive(false);

        }

    }
}
