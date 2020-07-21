using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formacion : SeekAcelerado
{


    [SerializeField]
    public GameObject leader = null;

    [SerializeField]
    public float offsetX = 1;

    [SerializeField]
    public float offsetZ = 1; //k para la función Inverse square law


    private GameObject pseudoObjetivo; //Nuevo target imaginario, que describe la posición donde predecimos que estará el target original.
    bool inicializa = true;

    public override MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidadJugador)
    {

        if (inicializa)
        {
            pseudoObjetivo = new GameObject();
            pseudoObjetivo.name = "Face Imaginario";
            pseudoObjetivo.SetActive(false);
            pseudoObjetivo.hideFlags = HideFlags.HideInHierarchy;
            inicializa = false;
        }

        MovimientoAcelerado noMover = new MovimientoAcelerado
        {
            steeringAngular = orientacion,
            steeringLineal = Vector3.zero,
        };


        if (leader == null) return noMover;

        Vector3 offset = new Vector3(offsetX, 0, offsetZ);
        offset = Quaternion.Euler(0, getOrientacion(leader), 0) * offset;


        pseudoObjetivo.transform.position = new Vector3(leader.transform.position.x + offset.x, leader.transform.position.y, leader.transform.position.z + offset.z);

        Vector3 distanciaFormacion = this.transform.position - pseudoObjetivo.transform.position;
        if (distanciaFormacion.magnitude > 50) return noMover; // Si está muy lejos no pueden retomar formación


        //Definimos un steering en el caso de que no haya que mover el personaje
        MovimientoAcelerado resultado = base.getSteering(pseudoObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidadJugador);

        return new MovimientoAcelerado
        {
            steeringAngular = (getOrientacion(leader) - orientacion) % 360,
            steeringLineal = resultado.steeringLineal,
        };




    }



}
