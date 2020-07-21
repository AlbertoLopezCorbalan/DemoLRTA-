using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTA : SeekAcelerado
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

    GameObject esfera = null; // Para modo debug

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



        esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Establecemos tamaño
        esfera.SetActive(false);
        esfera.hideFlags = HideFlags.HideInHierarchy;

    }




    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {

        this.Objetivo = target; // Para tener el Targer en el gizmos
        this.Personaje = personaje;

        //Definimos un steering en el caso de que no haya que mover el personaje
        MovimientoAcelerado noMover = new MovimientoAcelerado
        {
            steeringAngular = orientacion,
            steeringLineal = Vector3.zero,
        };
        
        if (target == null) return noMover;

        Grid.coordenada cTarget = grid.map2grid(target.transform.position.x, target.transform.position.z); //Casilla en la que se encuentra el target

         if (cTargetAnterior.x != Mathf.Infinity && (cTargetAnterior.x != cTarget.x || cTargetAnterior.y != cTarget.y)) inicializar = true; // Ha cambiado el target
        cTargetAnterior = cTarget;

        if (!esValida(cTarget.x, cTarget.y)) return noMover;

        if (inicializar) // solo se ponene las heuristicas la primera vez
        {
            for (int i = 0; i < tamMapX; i++)
                for (int j = 0; j < tamMapZ; j++)
                    if (map[i, j] != Mathf.Infinity) //Las casillas que no tienen obstacilos
                    {
                        Grid.coordenada cOrigen = new Grid.coordenada { x = i, y = j };

                        //Inicializamos las casillas con la heuristica

                        switch (heuristica)
                        {
                            case Heuristica.Manhattan: { map[i, j] = 1 * ((Mathf.Abs(cTarget.x - cOrigen.x) + Mathf.Abs(cTarget.y - cOrigen.y))); break;}
                            case Heuristica.Chebychev: { map[i, j] = 1 * Mathf.Max((Mathf.Abs(cTarget.x - cOrigen.x) + Mathf.Abs(cTarget.y - cOrigen.y))); break; }
                            case Heuristica.Euclidea: { map[i, j] = 1 * Vector2.Distance(new Vector2(cTarget.x, cTarget.y), new Vector2(cOrigen.x, cOrigen.y)) ; break; }
                        }
                        
                    }
            inicializar = false;
        }

        casillaActual = grid.map2grid(personaje.transform.position.x, personaje.transform.position.z); // Calculamos la casilla del jugador

        //if (casillaDestino.x != Mathf.Infinity && (casillaDestino.x != casillaActual.x || casillaDestino.y != casillaActual.y)  ) // Hasta no llegar a la siguiente casilla no se vuelve a calcular todo
        //    return base.getSteering(casillaObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);

        if (casillaActualAnterior.x != Mathf.Infinity && (casillaActualAnterior.x == casillaActual.x && casillaActualAnterior.y == casillaActual.y)  ) 
            // Hasta no llegar a otra casilla no se vuelve a calcular todo
            return base.getSteering(casillaObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);
        casillaActualAnterior = casillaActual;

        if ( cTarget.x == casillaActual.x && cTarget.y == casillaActual.y)
            return noMover; // Se ha alcanzado el target

        LinkedList<Grid.coordenada> posiblesCasillas = new LinkedList<Grid.coordenada>();
        if (esValida(casillaActual.x + 1, casillaActual.y)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x + 1, y = casillaActual.y });
        if (esValida(casillaActual.x, casillaActual.y + 1)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x, y = casillaActual.y + 1 });
        if (esValida(casillaActual.x - 1, casillaActual.y)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x - 1, y = casillaActual.y });
        if (esValida(casillaActual.x, casillaActual.y - 1)) posiblesCasillas.AddLast(new Grid.coordenada { x = casillaActual.x, y = casillaActual.y - 1 });

        if (posiblesCasillas.Count == 0) return noMover;

        casillaDestino = posiblesCasillas.First.Value;

        foreach (Grid.coordenada casilla in posiblesCasillas)
        {
            if (map[(int)Mathf.Round(casilla.x), (int)Mathf.Round(casilla.y)] < map[(int)Mathf.Round(casillaDestino.x), (int)Mathf.Round(casillaDestino.y)])
                casillaDestino = casilla;
        }

        if (map[(int)casillaDestino.x, (int)casillaDestino.y] == Mathf.Infinity) return noMover;

        //Debug.Log("Casilla Destino: " + casillaDestino.x + "," + casillaDestino.y);
        //Debug.Log("Valor casilla destino: " + map[(int)casillaDestino.x ,(int)casillaDestino.y]);

        // El destino al mundo y hacemos su seek
        Grid.coordenada c = grid.grid2map(casillaDestino.x, casillaDestino.y);
        casillaObjetivo.transform.position = new Vector3(c.x, personaje.transform.position.y, c.y);


        // Una vez calculado el destino actualizamos los costes de nuestro mapa

        map[(int)casillaDestino.x, (int)casillaDestino.y] = Mathf.Max( 1 + map[(int)casillaDestino.x, (int)casillaDestino.y], map[(int)casillaDestino.x, (int)casillaDestino.y]);


        return base.getSteering(casillaObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);

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



    private void Update()
    {
        if (casillaObjetivo != null)
        {
            
            Grid.coordenada c = grid.map2grid(casillaObjetivo.transform.position.x, casillaObjetivo.transform.position.z);
            c = grid.grid2map(c.x, c.y);
            esfera.transform.position = new Vector3(c.x, this.transform.position.y + 0.1f, c.y);
        }
       

        if (Input.GetKeyDown("k"))
            debug = !debug;


        if (debug)
        {
            esfera.SetActive(true);
        }
        else
        {
            esfera.SetActive(false);
        }

    }

}
