using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : Seek
{
    [SerializeField]
    protected List<Vector3> path = null;


    Vector3 LastcloserPoint = Vector3.zero;
    

    override
    public Movement getSteering(GameObject target, GameObject character, float maxVelocity, float maxAceleration, float maxAngularVelocity, float maxAcelerationVelocity, float orientation, Vector3 characterVelocity)
    {

        // We define a noMove Movement
        Movement noMove = new Movement
        {
            Angularsteering = orientation,
            Linealsteering = Vector3.zero,
        };

        if (this.path == null) return noMove;


        // We extract each point in the path
        int numPoints = path.Count; //

        if (numPoints == 0) return noMove;


        GameObject pseudoObject = new GameObject();
        pseudoObject.name = "PathPoint";
        pseudoObject.SetActive(false);
        pseudoObject.hideFlags = HideFlags.HideInHierarchy;
        pseudoObject.transform.position = new Vector3(path[0].x, character.transform.position.y, path[0].z);

        if (numPoints == 1) return base.getSteering(pseudoObject, character, maxVelocity, maxAceleration, maxAngularVelocity, maxAcelerationVelocity, orientation, characterVelocity);

        
        Vector3 closerPoints = path[0];
        Vector3 targetPoint = path[1];
        float maxDistance = (path[0] - character.transform.position).magnitude;
        // Search for the closer point
        for (int i = 0; i < numPoints; i++)
        {
            float dist = (path[i] - character.transform.position).magnitude;
            if (dist < maxDistance)
            {
                maxDistance = dist;
                closerPoints = path[i];
                if (i + 1 < numPoints) targetPoint = path[(i + 1)];
                else targetPoint = closerPoints;
            }

        }

        // This remove the points that we have already achieve
        if (LastcloserPoint == Vector3.zero ) LastcloserPoint = closerPoints; // Inicialize
        else if (LastcloserPoint != closerPoints) { path.Remove(path[0]); LastcloserPoint = path[0]; }  // Remove path[0] because could skipp one point, and its from 0 to path.count.


        pseudoObject.transform.position = new Vector3(targetPoint.x, character.transform.position.y, targetPoint.z);

        //Seek to targetPoint
        Movement steering = base.getSteering(pseudoObject, character, maxVelocity, maxAceleration, maxAngularVelocity, maxAcelerationVelocity, orientation, characterVelocity);
        return
         new Movement
         {
             Linealsteering = steering.Linealsteering,
             Angularsteering = Vector3.SignedAngle(this.transform.forward, characterVelocity.normalized, Vector3.up),
             // To look where we are moving into
         };
    }

}
