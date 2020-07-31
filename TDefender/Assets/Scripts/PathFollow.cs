using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : Seek
{
    [SerializeField]
    GameObject path = null;


    override
    public Movement getSteering(GameObject target, GameObject character, float maxVelocity, float maxAceleration, float maxAngularVelocity, float maxAcelerationVelocity, float orientation, Vector3 characterVelocity)
    {

        // We define a noMove Movement
        Movement noMove = new Movement
        {
            Angularsteering = orientation,
            Linealsteering = Vector3.zero,
        };

        if (path == null) return noMove;


        // We extract each point in the path
        List<GameObject> pointsList = new List<GameObject>(path.transform.childCount);
        int numPoints = path.transform.childCount;
        for (int i = 0; i < numPoints; i++)
        {
            GameObject g = path.transform.GetChild(i).gameObject;
            pointsList.Insert(i, g);
        }

        if (numPoints == 0) return noMove;
        if (numPoints == 1) return base.getSteering(pointsList[0], character, maxVelocity, maxAceleration, maxAngularVelocity, maxAcelerationVelocity, orientation, characterVelocity);

        
        GameObject closerPoints = pointsList[0];
        GameObject targetPoint = pointsList[1];
        float maxDistance = (pointsList[0].transform.position - character.transform.position).magnitude;
        // Search for the closer point
        for (int i = 0; i < numPoints; i++)
        {
            float dist = (pointsList[i].transform.position - character.transform.position).magnitude;
            if (dist < maxDistance)
            {
                maxDistance = dist;
                closerPoints = pointsList[i];
                targetPoint = pointsList[(i + 1) % numPoints];
            }

        }

        //Seek to targetPoint
        Movement steering = base.getSteering(targetPoint, character, maxVelocity, maxAceleration, maxAngularVelocity, maxAcelerationVelocity, orientation, characterVelocity);
        return
         new Movement
         {
             Linealsteering = steering.Linealsteering,
             Angularsteering = Vector3.SignedAngle(this.transform.forward, characterVelocity.normalized, Vector3.up),
             // To look where we are moving into
         };
    }

}
