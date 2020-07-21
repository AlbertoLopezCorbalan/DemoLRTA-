using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTA2 : SeekAcelerado
{
    [SerializeField] GameObject suelo = null;
    private float tamMapX;
    private float tamMapZ;
    private float[,] map;
    private Grid grid;
    private Grid.coordenada casillaActual;
    private Grid.coordenada casillaDestino  = new Grid.coordenada { x = Mathf.Infinity, y = Mathf.Infinity};
    private Grid.coordenada cTargetAnterior = new Grid.coordenada { x = Mathf.Infinity, y = Mathf.Infinity };

    private Grid.coordenada casillaActualAnterior = new Grid.coordenada { x = Mathf.Infinity, y = Mathf.Infinity };

    public enum Heuristica { Euclidea, Manhattan, Chebychev }
    public Heuristica heuristica;

    private bool inicializar = true;

    // Gizmos
    GameObject casillaObjetivo = null;
    GameObject Objetivo = null;
    GameObject Personaje = null;

    bool debug = false;

    private void Start()
    {

        grid = suelo.GetComponent<Grid>();
        map = grid.getMapClone();

        tamMapX = grid.getTamMapX();
        tamMapZ = grid.getTamMapZ();

        casillaObjetivo = new GameObject();
        casillaObjetivo.name = "Casilla Objetivo";
        casillaObjetivo.SetActive(true);
        casillaObjetivo.hideFlags = HideFlags.HideInHierarchy; // Para que no se muestre en la jerarquía

    }




    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {

        float[,] searchSpace = LocalSearchSpace();


        ValueUpdateStep(map);

        ActionSelectionStep(searchSpace);

        //ActionExecutionStep();

        MovimientoAcelerado noMover = new MovimientoAcelerado
        {
            steeringAngular = orientacion,
            steeringLineal = Vector3.zero,
        };

        return noMover;

    }


    private bool esValida(float cx, float cy)
    {
        if (0 <= cx && cx < tamMapX && 0 <= cy && cy < tamMapZ) return true;
        return false;

    }





    private float [,] LocalSearchSpace()
    {
        return null;
    }

    public void ValueUpdateStep (float [,] map)
    {

    }

    public void ActionSelectionStep (float [,] searchSpace)
    {

    }



}
