using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapaInfluencia : MonoBehaviour
{

    [SerializeField]
    GameObject obstaculos = null;

    public float[,] map = null;
    public int L = 1; // Tamaño de las casillas
    private int tamMapX, tamMapZ; // 

    private float offsetMapaX, offsetMapaZ; //Desplazamiento de coordenadas de unity(centro del objeto)  respecto al grid (esquina inferior)
    private float offsetCoordenadasMapaX, offsetCoordenadasMapaZ; // Coordenadas mapa respecto el punto 00 de unity (transform.position)
    private float offsetCasillasX, offsetCasillasZ;
    private float offsetCoordenadasCasillasX, offsetCoordenadasCasillasZ; // Coordenadas mapa respecto el punto 00 de unity (transform.position)

    private float TiempoRecalculo;
    public float TIEMPO_ACTUALIZACION = 5.0f;

    GameObject[,] planos = null;
    Vector3[,] vertices = null;

    Color ColorRojo = Color.red;
    Color ColorAzul = Color.blue;

    bool inicializar = false;


    public struct coordenada
    {
        public float x;
        public float y;
    }

    private void Start()
    {

        TiempoRecalculo = Time.time - TIEMPO_ACTUALIZACION; // para que la primera vez se calcule

        tamMapX = (int)Mathf.Round(this.transform.localScale.x / L);
        tamMapZ = (int)Mathf.Round(this.transform.localScale.z / L);

        // Para map2grid
        offsetCasillasX = tamMapX / 2; // Offset para situar las casillas del mundo al grid y que coindan
        offsetCasillasZ = tamMapZ / 2;

        // Para grid2map
        offsetMapaX = (this.transform.localScale.x / 2); // Offset para el eje de coordenadas esté entre -N y N, mientras que nuestro array sea 0-N*2
        offsetMapaZ = (this.transform.localScale.z / 2);

        offsetCoordenadasMapaX = this.transform.position.x; // Offset para acoplar el grid al floor dentro del mundo
        offsetCoordenadasMapaZ = this.transform.position.z;


        offsetCoordenadasCasillasX = this.transform.position.x / L;
        offsetCoordenadasCasillasZ = this.transform.position.z / L;



        map = new float[tamMapX, tamMapZ];

        for (int i = 0; i < tamMapX; i++)
            for (int j = 0; j < tamMapZ; j++)
                map[i, j] = 0; //Inicializamos todas las casillas a este valor



        //Recorremos la lista de obstáculos para ponerlos en grid
        if (obstaculos != null)
        {
            int numPuntos = obstaculos.transform.childCount;
            for (int i = 0; i < numPuntos; i++)
            {
                GameObject g = obstaculos.transform.GetChild(i).gameObject;
                coordenada c = map2grid(g.transform.position.x, g.transform.position.z);
                if (0 <= c.x && c.x < tamMapX && 0 <= c.y && c.y < tamMapZ)
                    //Esta dentro de los limites del array
                    map[(int)c.x, (int)c.y] = Mathf.Infinity; //Ponemos -1 a las casillas en las que haya un obstáculo


            }
        }


        vertices = new Vector3 [tamMapX , tamMapZ];
        for (int i = 0; i < tamMapX; i++)
            for (int j = 0; j < tamMapZ; j++)
            {
                coordenada c = grid2map(i, j);
                vertices[i, j] = new Vector3(c.x, this.transform.position.y, c.y);
            }

        planos = new GameObject[tamMapX, tamMapZ]; 
        for (int i = 0; i < tamMapX; i++)
            for (int j = 0; j < tamMapZ; j++)
            {
                if (map[i, j] != Mathf.Infinity) {
                    coordenada c = grid2map(i, j);
                    planos[i, j] = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    planos[i, j].transform.position = new Vector3 (c.x + (L/2), transform.position.y + 0.5f , c.y + (L / 2));
                    planos[i, j].transform.localScale = new Vector3( planos[i, j].transform.localScale.x/2 , planos[i, j].transform.localScale.y / 2, planos[i, j].transform.localScale.z / 2);
                    planos[i, j].hideFlags = HideFlags.HideInHierarchy; // ocultar en la jerarquía
                }
            }


        inicializar = true;
    }


    public float getTamMapX()
    {

        return tamMapX;

    }

    public float getTamMapZ()
    {
        return tamMapZ;
    }



    public coordenada grid2map(float i, float j) //tranforma la casilla del array en valores del mapa de unity
    {
        return new coordenada
        {
            x = i * L - offsetMapaX  + offsetCoordenadasMapaX, //El offset del mapa es para transformar teniendo en cuenta que en unity el objeto se empieza a dibujar por el centro del mismo, en lugar de la esquina inferior izquierda
            y = j * L - offsetMapaZ + offsetCoordenadasMapaZ
        };
    }

    public coordenada map2grid(float i, float j) // ¿ A qué casilla del array corresponde una posición del mapa?
    {
        return new coordenada
        {
            x = Mathf.Round(i/L + offsetCasillasX - offsetCoordenadasCasillasX),   //Al sumarle el offset nos situamos en la casilla correspondiente
            y = Mathf.Round(j/L + offsetCasillasZ - offsetCoordenadasCasillasZ)
        };

    }



    void Update()
    {   
        //float t = Time.time;
        if ((Time.time - TiempoRecalculo) > TIEMPO_ACTUALIZACION)
        {
            TiempoRecalculo = Time.time;
            // Se toman todas las unidades
            GameObject[] rojo, azul;
            rojo = GameObject.FindGameObjectsWithTag("TeamRojo");
            azul = GameObject.FindGameObjectsWithTag("TeamAzul");
            CalcularMapaInfluencia(rojo, azul);
        
        }
        
    }

    private void CalcularMapaInfluencia(GameObject[] EquipoRojo, GameObject[] EquipoAzul)
    {


        for (int i = 0; i < tamMapX; i++)
            for (int j = 0; j < tamMapZ; j++)
            {
                if (map[i, j] != Mathf.Infinity)
                {
                    coordenada c = grid2map(i, j);

                    Color color = minimaDistancia(EquipoRojo, EquipoAzul, new Vector3(c.x, transform.position.y, c.y )  );

                    Renderer render = planos[i, j].GetComponent<Renderer>();
                    render.material.color = color;

                }
            }


    }






    private Color minimaDistancia (GameObject[] EquipoRojo, GameObject[] EquipoAzul, Vector3 punto)
    {
        //Calculamos el más cercano rojo

        float minAzul = Mathf.Infinity;
        foreach (GameObject g in EquipoAzul)
        {
            Vector3 distancia = new Vector3(g.transform.position.x, 0, g.transform.position.z) - new Vector3(punto.x, 0, punto.z);
            if (distancia.magnitude < minAzul) minAzul = distancia.magnitude;
        }
        //Calculamos el más cercano azul
        float minRojo = Mathf.Infinity;
        foreach (GameObject g in EquipoRojo)
        {
            Vector3 distancia = new Vector3(g.transform.position.x, 0, g.transform.position.z) - new Vector3(punto.x, 0, punto.z);
            if (distancia.magnitude < minRojo) minRojo = distancia.magnitude;
        }
        if (minRojo <= minAzul) return ColorRojo;
        return ColorAzul;
    }

    public float costeTactico(float i, float j, Color color ) // ¿ A qué casilla del array corresponde una posición del mapa?
    {
        if (!inicializar) return 0.0f;

        if ( 0 <= i && i < tamMapX && 0 <= j && j < tamMapZ && planos[(int)i, (int)j] != null) // Si planos == null sería porque es Mathf.infinity y es un obstaculo
        {
            Renderer render = planos[(int)i, (int)j].GetComponent<Renderer>();
            if (render.material.color == color) return 1.0f;
        }
        return 0.0f;
    }


}




