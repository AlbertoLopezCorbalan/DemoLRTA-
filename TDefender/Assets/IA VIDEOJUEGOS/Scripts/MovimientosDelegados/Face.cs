using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align
{

    protected Vector3 direccion; // Vector direccion entre el personaje y el target
    protected float distancia;

    private GameObject pseudoObjetivo; //Nuevo target imaginario, que describe la posición donde predecimos que estará el target original.
    private bool inicializa = true;

    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {
        if (inicializa)
        {
            pseudoObjetivo = new GameObject();
            pseudoObjetivo.name = "Face Imaginario";
            pseudoObjetivo.SetActive(false);
            pseudoObjetivo.hideFlags = HideFlags.HideInHierarchy;
            inicializa = false;
        }

        // Se calcula el vector posicion destino
        direccion = target.transform.position - personaje.transform.position;
        distancia = direccion.magnitude; //Modulo de la direccion entre el personaje y el target

        if(distancia == 0) return base.getSteering(target, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);


        Quaternion nuevaRotacion = target.transform.rotation;

        pseudoObjetivo.transform.position = personaje.transform.position;


        //Calculamos el nuevo angulo
        float rotacionObjetivo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;


        //Ponemos la orientacion del target al calculado anteriormente
        pseudoObjetivo.transform.rotation = Quaternion.Euler(0,rotacionObjetivo,0);

        return base.getSteering(pseudoObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);



    }


}
