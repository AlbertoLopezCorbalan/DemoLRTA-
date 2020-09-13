using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTAObstaculos : FleeAcelerado
{
    [SerializeField] GameObject suelo = null;
    private float tamMapX;
    private float tamMapZ;
    private float[,] map;
    private Grid grid;
    private Grid.coordenada casillaActual;
    private Grid.coordenada casillaDestino  = new Grid.coordenada { x = Mathf.Infinity, y = Mathf.Infinity};
    private Grid.coordenada cTargetAnterior = new Grid.coordenada { x = Mathf.Infinity, y = Mathf.Infinity };

    public enum Heuristica { Euclidea, Manhattan, Chebychev }
    public Heuristica heuristica;

    // Gizmos
    GameObject casillaObjetivo = null;
    GameObject Objetivo = null;
    GameObject Personaje = null;

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
        this.Personaje = personaje;
        //Definimos un steering en el caso de que no haya que mover el personaje
        MovimientoAcelerado noMover = new MovimientoAcelerado
        {
            steeringAngular = orientacion,
            steeringLineal = Vector3.zero,
        };
     
        casillaActual = grid.map2grid(personaje.transform.position.x, personaje.transform.position.z); // Calculamos la casilla del jugador
        
        LinkedList<Grid.coordenada> posiblesCasillas = new LinkedList<Grid.coordenada>();
        if (esValida(casillaActual.x + 1, casillaActual.y)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x + 1, y = casillaActual.y });
        if (esValida(casillaActual.x, casillaActual.y + 1)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x, y = casillaActual.y + 1 });
        if (esValida(casillaActual.x - 1, casillaActual.y)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x - 1, y = casillaActual.y });
        if (esValida(casillaActual.x, casillaActual.y - 1)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x, y = casillaActual.y - 1 });

        if (posiblesCasillas.Count == 0) return noMover;

        casillaDestino = posiblesCasillas.First.Value;

        foreach (Grid.coordenada casilla in posiblesCasillas)
        {
            if (map[(int)Mathf.Round(casilla.x), (int)Mathf.Round(casilla.y)] > map[(int)Mathf.Round(casillaDestino.x), (int)Mathf.Round(casillaDestino.y)])
                casillaDestino = casilla;
        }

        if (map[(int)casillaDestino.x, (int)casillaDestino.y] != Mathf.Infinity) return noMover;

        // El destino al mundo y hacemos su seek
        Grid.coordenada c = grid.grid2map(casillaDestino.x, casillaDestino.y);
        casillaObjetivo.transform.position = new Vector3(c.x, personaje.transform.position.y, c.y);
        return base.getSteering(casillaObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);

        // En esta versión no actualizamos pesos, porque será usada junto a formaciones, es una manera de encontrar obstruction cercanos mediante pathfinding e ir en contra de ellos.

    }


    private bool esValida(float cx, float cy)
    {
        if (0 <= cx && cx < tamMapX && 0 <= cy && cy < tamMapZ) return true;
        return false;

    }

    private void OnDrawGizmos()
    {
        if (casillaObjetivo != null)
        {
            Grid.coordenada c = grid.map2grid(casillaObjetivo.transform.position.x, casillaObjetivo.transform.position.z);
            c = grid.grid2map(c.x, c.y);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(c.x, this.transform.position.y + 0.1f, c.y), 1);
        }
        if (Objetivo != null)
        {
            Grid.coordenada c = grid.map2grid(Objetivo.transform.position.x, Objetivo.transform.position.z);
            c = grid.grid2map(c.x, c.y);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(c.x, this.transform.position.y + 0.1f, c.y), 1);
        }
        if (Personaje != null)
        {
            Grid.coordenada c = grid.map2grid(Personaje.transform.position.x, Personaje.transform.position.z);
            c = grid.grid2map(c.x, c.y);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(c.x, this.transform.position.y + 0.1f, c.y), 1);
        }
    }
}
