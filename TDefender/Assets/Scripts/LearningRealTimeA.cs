using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningRealTimeA : PathFollow
{
    [SerializeField] GameObject floor = null;
    private float sizeMapX;
    private float sizeMapZ;
    private float[,] map;
    private CustomGrid grid;

    public enum Heuristic { Euclidean, Manhattan, Chebychev }
    public Heuristic heuristic;


    CustomGrid.coordinate gridTarget = new CustomGrid.coordinate { x = -1, y = -1}; // Start value
    CustomGrid.coordinate gridCharacter;

    private bool firstTime = true;



    private void Start()
    {

        grid = floor.GetComponent<CustomGrid>();
        map = grid.getMapClone();

        sizeMapX = grid.getSizeMapX();
        sizeMapZ = grid.getSizeMapZ();



    }

    public void inicialize()
    {
        for (int i = 0; i < sizeMapX; i++)
            for (int j = 0; j < sizeMapZ; j++)
                if (map[i, j] != Mathf.Infinity) 
                {
                    Grid.coordenada gridOrigin = new Grid.coordenada { x = i, y = j };                 

                    switch (heuristic)
                    {
                        case Heuristic.Manhattan: { map[i, j] = 1 * ((Mathf.Abs(gridTarget.x - gridOrigin.x) + Mathf.Abs(gridTarget.y - gridOrigin.y))); break; }
                        case Heuristic.Chebychev: { map[i, j] = 1 * Mathf.Max((Mathf.Abs(gridTarget.x - gridOrigin.x) + Mathf.Abs(gridTarget.y - gridOrigin.y))); break; }
                        case Heuristic.Euclidean: { map[i, j] = 1 * Vector2.Distance(new Vector2(gridTarget.x, gridTarget.y), new Vector2(gridOrigin.x, gridOrigin.y)); break; }
                    }

                }
        firstTime = false;
    }




    public void targetMayChange(GameObject target) // Check if the target has been changed, in that case reset the distances in the map.
    {
        if (gridTarget.x == -1) return; // First time, already inicialize

        CustomGrid.coordinate newgridTarget = grid.map2grid(target.transform.position.x, target.transform.position.z); // possible new position

        if (gridTarget.x != newgridTarget.x || gridTarget.y != newgridTarget.y) inicialize(); // Target change, reset ditance

        // Target is the same as before, do nothing
    }


    override
    public Movement getSteering(GameObject target, GameObject character, float maxVelocity, float maxAceleration, float maxAngularVelocity, float maxAcelerationVelocity, float orientation, Vector3 characterVelocity)
    {


        /* 1. Exit condition -> Be in the same grid as the target */

        Movement doNotMove = new Movement
        {
            Angularsteering = orientation,
            Linealsteering = Vector3.zero,
        };
        
        if (target == null || character == null) return doNotMove;

        targetMayChange(target); // In case the target change we should restart de distances

        gridTarget = grid.map2grid(target.transform.position.x, target.transform.position.z); // Target in our grid
        gridCharacter = grid.map2grid(character.transform.position.x, character.transform.position.z); // Character in our grid

        if (gridTarget.x == gridCharacter.x && gridTarget.y == gridCharacter.y) return doNotMove; // Target has been achieved

        if (firstTime) inicialize(); // Inicialize the map with the heuristics distances


        /* 2. Local Space of Search -> Minimum in our case */

        CustomGrid.coordinate gridActual;

        if (path.Count == 0) gridActual = gridCharacter;
        else
        {
            gridActual = new CustomGrid.coordinate { x = path[path.Count - 1].x, y = path[path.Count - 1].z };
            gridActual = grid.map2grid(gridActual.x, gridActual.y);
        }



        /* 3  Do Action if it is calculated  */

        if (gridActual.x == gridTarget.x && gridActual.y == gridTarget.y) // Target is one of the path calculated, just move throgh the points
        {
            return base.getSteering(target, character, maxVelocity, maxAceleration, maxAngularVelocity, maxAcelerationVelocity, orientation, characterVelocity);
        }
                    

        // Continue calculating the path.... if it is not yet

        LinkedList<CustomGrid.coordinate> posibleStep = new LinkedList<CustomGrid.coordinate>();
        if (esValida(gridActual.x + 1, gridActual.y)) posibleStep.AddLast(new CustomGrid.coordinate { x = gridActual.x + 1, y = gridActual.y });
        if (esValida(gridActual.x, gridActual.y + 1)) posibleStep.AddLast(new CustomGrid.coordinate { x = gridActual.x, y = gridActual.y + 1 });
        if (esValida(gridActual.x - 1, gridActual.y)) posibleStep.AddLast(new CustomGrid.coordinate { x = gridActual.x - 1, y = gridActual.y });
        if (esValida(gridActual.x, gridActual.y - 1)) posibleStep.AddLast(new CustomGrid.coordinate { x = gridActual.x, y = gridActual.y - 1 });

        if (posibleStep.Count == 0) return doNotMove; // No posible path

        CustomGrid.coordinate  gridNextStep = posibleStep.First.Value;

        foreach (CustomGrid.coordinate casilla in posibleStep) // min of the 4 directions
        {
            if (map[(int)Mathf.Round(casilla.x), (int)Mathf.Round(casilla.y)] < map[(int)Mathf.Round(gridNextStep.x), (int)Mathf.Round(gridNextStep.y)])
                gridNextStep = casilla;
        }

        if (map[(int)gridNextStep.x, (int)gridNextStep.y] == Mathf.Infinity) return doNotMove;

        gridNextStep = grid.grid2map(gridNextStep.x, gridNextStep.y);
        path.Add(new Vector3(gridNextStep.x, character.transform.position.y ,gridNextStep.y));


        /* 3  Do Action while calculating the path  */

        Movement action = base.getSteering(target, character, maxVelocity, maxAceleration, maxAngularVelocity, maxAcelerationVelocity, orientation, characterVelocity);


        /* 4  Update cost  */

        map[(int)gridActual.x, (int)gridActual.y] = Mathf.Max(1 + map[(int)gridActual.x, (int)gridActual.y], map[(int)gridActual.x, (int)gridActual.y]);

        return action;

    }


    private bool esValida(float cx, float cy)
    {
        if (0 <= cx && cx < sizeMapX && 0 <= cy && cy < sizeMapZ) return true;
        return false;

    }

    
    private void OnDrawGizmos()
    {
        if (path.Count > 0) // To show the path
        {
            for (int i = 0; i + 1 < path.Count; i++)
            {
                //CustomGrid.coordinate cor = new CustomGrid.coordinate { x = path[i].x, y = path[i].z };
                //Gizmos.color = Color.green;

                //Gizmos.DrawWireSphere(new Vector3(cor.x, this.transform.position.y + 0.01f, cor.y), 0.3f);


                Gizmos.color = Color.green;
                CustomGrid.coordinate cor = new CustomGrid.coordinate { x = path[i].x, y = path[i].z };
                Vector3 from = new Vector3(cor.x, transform.position.y + 0.1f, cor.y);
                CustomGrid.coordinate cor2 = new CustomGrid.coordinate { x = path[i+1].x, y = path[i+1].z };
                Vector3 to = new Vector3(cor2.x, transform.position.y + 0.1f, cor2.y);
                Gizmos.DrawLine(from, to);


            }
        }
    }
}
