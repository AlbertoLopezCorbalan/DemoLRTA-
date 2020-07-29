using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    GameObject enemigo;


    [SerializeField]
    int nEnemigosMax;

    [SerializeField]
    int t_aparicion;

    [SerializeField]
    int enemigosDerrotadosParaEmpezar; //A partir de que enemigo han de empezar a salir (oleada)

    static public int enemigosGlobales; //numero de enemigos que han muerto entre todos los scripts

    bool crearEnemigo = true;
    int nEnemigos; //numero de enemigos que este objeto ha creado (locales a este spawn)

    void Start()
    {
        //enemigo.transform.position = this.transform.position;
        enemigosGlobales = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemigosGlobales > enemigosDerrotadosParaEmpezar) {
            if (crearEnemigo && nEnemigos < nEnemigosMax) Invoke("spawnEnemigo", t_aparicion);
            crearEnemigo = false;
        }
    }


    void spawnEnemigo()
    {

        GameObject pepe = Instantiate(enemigo);
        pepe.transform.position = this.transform.position;
        nEnemigos++;
        crearEnemigo = true;


    }



}
