using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderDelegado : Face
{
    [SerializeField]
    float wanderOffset = 2; //Distancia hasta el circulo del wander

    [SerializeField]
    float wanderRadius = 1; //Circulo del wander

    [SerializeField]
    float wanderRate = 50; //Maximo giro permitido

    float wanderOrientation = 0; //Mantiene la orientacion del targetWander

    GameObject pseudoTarget = null;

     private void Start()
    {
        pseudoTarget = new GameObject();
        pseudoTarget.name = "Wander Imaginario";
        pseudoTarget.SetActive(false);
        pseudoTarget.hideFlags = HideFlags.HideInHierarchy; // Para que no se muestre en la jerarquía
    }

    override
        public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {
        //Actualiza la orientacion del wanderTarget
        wanderOrientation += (Random.value - Random.value) * wanderRate;


        //Calculamos la orientacion del target 
        float targetOrientacion = wanderOrientation + orientacion;


        //Situamos el wanderTarget en el centro del circulo
        pseudoTarget.transform.position = personaje.transform.position + wanderOffset * getVectorOrientacion(personaje, orientacion);



        //Situamos el wanderTarget en la circunferencia
        pseudoTarget.transform.rotation = Quaternion.Euler(0, targetOrientacion,0);
        pseudoTarget.transform.position +=  wanderRadius * getVectorOrientacion(pseudoTarget, targetOrientacion);


        MovimientoAcelerado wander =  base.getSteering(pseudoTarget, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);

        wander.steeringLineal = maxAceleracion * getVectorOrientacion(personaje, orientacion);
        return wander;

    }

    private void OnDrawGizmos() // Gizmo: una línea en la dirección del objetivo
    {
        Vector3 from = transform.position; // Origen de la línea

        Gizmos.color = Color.yellow;        // Mirando en la dirección de la orientación.
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(from, direction);

        Gizmos.color = Color.red;        // Target Radius
        Gizmos.DrawWireSphere(transform.position + wanderOffset * transform.forward, wanderRadius);


        Gizmos.color = Color.blue;        // Target Radius
        if (pseudoTarget != null) Gizmos.DrawWireSphere(pseudoTarget.transform.position, 0.1f);

    }


}
