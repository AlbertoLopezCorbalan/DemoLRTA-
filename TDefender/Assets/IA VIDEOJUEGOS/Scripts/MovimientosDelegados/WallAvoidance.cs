using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidance : SeekAcelerado
{
    //GameObject que contiene todos los puntos de la ruta a seguir
    [SerializeField]
    float avoidDistance = 7; // Distancia mínima al muro (radio del personaje y un poco de margen)

    [SerializeField]
    float lookahead = 7; // Distancia máxima donde "mira"

    GameObject pseudoObjetivo;

    Vector3 posicion = Vector3.zero;

    Vector3 nuevoObjetivo = Vector3.zero;

    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {
        Vector3 rayVector = velocidad;

        RaycastHit hit, hitI, hitD; // HitI y HitD son los bigotes

        bool delante = Physics.Raycast(transform.position + velocidad.normalized, rayVector.normalized, out hit, lookahead); // Se separa un poco para que no se choque el raycast con él mismo
        bool izquierda = Physics.Raycast(transform.position + velocidad.normalized, Quaternion.Euler(0, -90, 0) * rayVector.normalized, out hitI, lookahead / 2);
        bool derecha = Physics.Raycast(transform.position + velocidad.normalized, Quaternion.Euler(0, +90, 0) * rayVector.normalized, out hitD, lookahead / 2);

        if (delante && hit.transform.tag != "muro") delante = false; // Para que solo colisione con los objetos "muro"
        if (izquierda && hitI.transform.tag != "muro") izquierda = false;
        if (derecha && hitD.transform.tag != "muro") derecha = false;

        MovimientoAcelerado movimientoLineal; // Seek nos dará el movimiento

        if ( delante || izquierda || derecha )
        {
            Vector3 normal = Vector3.zero;
            if (delante) { 
                posicion = hit.point;
                normal = hit.normal;
            } else if (derecha) {
                posicion = hitD.point;
                normal = hitD.normal;
            } else if (izquierda) {
                posicion = hitI.point;
                normal = hitI.normal;
            }

            nuevoObjetivo = posicion + normal * avoidDistance;
            pseudoObjetivo.transform.position = new Vector3(nuevoObjetivo.x, personaje.transform.position.y , nuevoObjetivo.z);

            if (delante) Debug.DrawRay(transform.position + velocidad.normalized, velocidad.normalized * lookahead, Color.red);
            else Debug.DrawRay(transform.position + velocidad.normalized, velocidad.normalized * lookahead, Color.blue);

            if (izquierda) Debug.DrawRay(transform.position + velocidad.normalized, Quaternion.Euler(0, -90, 0) * velocidad.normalized * lookahead / 2, Color.red);
            else Debug.DrawRay(transform.position + velocidad.normalized, Quaternion.Euler(0, -90, 0) * velocidad.normalized * lookahead / 2, Color.blue);

            if (derecha) Debug.DrawRay(transform.position + velocidad.normalized, Quaternion.Euler(0, +90, 0) * velocidad.normalized * lookahead / 2, Color.red);
            else Debug.DrawRay(transform.position + velocidad.normalized, Quaternion.Euler(0, +90, 0) * velocidad.normalized * lookahead / 2, Color.blue);

            movimientoLineal = base.getSteering(pseudoObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);
        }
        else
        {
            Debug.DrawRay(transform.position + velocidad.normalized, velocidad.normalized * lookahead, Color.blue);
            Debug.DrawRay(transform.position + velocidad.normalized, Quaternion.Euler(0, -90, 0) * velocidad.normalized * lookahead / 2, Color.blue);
            Debug.DrawRay(transform.position + velocidad.normalized, Quaternion.Euler(0, +90, 0) * velocidad.normalized * lookahead / 2, Color.blue);
            movimientoLineal =  base.getSteering(target, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);
            
        }


        return
             new MovimientoAcelerado
             {
                 steeringLineal = movimientoLineal.steeringLineal, // Seek
                 steeringAngular = Vector3.SignedAngle(getVectorOrientacion(personaje, orientacion), velocidad.normalized, Vector3.up)
                 // Para mirar en la dirección velocidad, calculamos la diferencia entre nuestra orientación y el vector velocidad
             };

    }

    private void Start()
    {
        pseudoObjetivo = new GameObject();
        pseudoObjetivo.name = "Wall Avoidance Imaginario";
        pseudoObjetivo.SetActive(true);
        pseudoObjetivo.hideFlags = HideFlags.HideInHierarchy; // Para que no se muestre en la jerarquía
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        if (posicion != Vector3.zero) Gizmos.DrawWireSphere(posicion, 1);

        Gizmos.color = Color.blue;
        if (nuevoObjetivo != Vector3.zero) Gizmos.DrawWireSphere(nuevoObjetivo, 1);
    }

}
