using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : SeekAcelerado
{

    //GameObject que contiene todos los puntos de la ruta a seguir
    [SerializeField]
    GameObject ruta = null;


    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {

        //Definimos un steering en el caso de que no haya que mover el personaje
        MovimientoAcelerado noMover = new MovimientoAcelerado
        {
            steeringAngular = orientacion,
            steeringLineal = Vector3.zero,
        };

        //si no hay gameObject que seguir
        if (ruta == null) return noMover;


        //Extraemos del GameObject Ruta, todos los waypoints (gameObjects)
        List<GameObject> listaPuntos = new List<GameObject>(ruta.transform.childCount);
        int numPuntos = ruta.transform.childCount;
        for (int i = 0; i < numPuntos; i++)
        {
            GameObject g = ruta.transform.GetChild(i).gameObject;
            listaPuntos.Insert(i, g);
        }

        if (numPuntos == 0) return noMover;
        if (numPuntos == 1) return base.getSteering(listaPuntos[0], personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);

        
        //inicializamos el objetivo
        GameObject masCercano = listaPuntos[0];
        GameObject objetivo = listaPuntos[1];
        float distMaxima = (listaPuntos[0].transform.position - personaje.transform.position).magnitude;

        //Buscamos el punto mas cercano
        for (int i = 0; i < numPuntos; i++)
        {
            float dist = (listaPuntos[i].transform.position - personaje.transform.position).magnitude;
            if (dist < distMaxima)
            {
                distMaxima = dist;
                masCercano = listaPuntos[i];
                objetivo = listaPuntos[(i + 1) % numPuntos];
            }

        }

        //Realizamos un seek al puntoObjetivo
        MovimientoAcelerado steering = base.getSteering(objetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);
        return
         new MovimientoAcelerado
         {
             steeringLineal = steering.steeringLineal,
             steeringAngular = Vector3.SignedAngle(getVectorOrientacion(personaje, orientacion), velocidad.normalized, Vector3.up)
             // Para mirar en la dirección velocidad, calculamos la diferencia entre nuestra orientación y el vector velocidad
         };


    }

}
