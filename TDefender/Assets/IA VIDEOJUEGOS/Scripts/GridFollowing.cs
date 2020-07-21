using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFollowing : MonoBehaviour
{
    [SerializeField]
    GameObject target = null;

    public float[,] map;
    public int L = 1; // Tamaño de las casillas
    private int tamMapX, tamMapZ; // 

    private float offsetMapaX, offsetMapaZ; //Desplazamiento de coordenadas de unity(centro del objeto)  respecto al grid (esquina inferior)
    private float offsetCoordenadasMapaX, offsetCoordenadasMapaZ; // Coordenadas mapa respecto el punto 00 de unity (transform.position)
    private float offsetCasillasX, offsetCasillasZ;
    private float offsetCoordenadasCasillasX, offsetCoordenadasCasillasZ; // Coordenadas mapa respecto el punto 00 de unity (transform.position)

    public struct coordenada
    {
        public float x;
        public float y;
    }

    private void Start()
    {
        tamMapX = (int)Mathf.Round(this.transform.localScale.x / L);
        tamMapZ = (int)Mathf.Round(this.transform.localScale.z / L);


        // Para grid2map
        offsetMapaX = (this.transform.localScale.x / 2); // Offset para el eje de coordenadas esté entre -N y N, mientras que nuestro array sea 0-N*2
        offsetMapaZ = (this.transform.localScale.z / 2);

        offsetCoordenadasMapaX = this.transform.position.x; // Offset para acoplar el grid al suelo dentro del mundo
        offsetCoordenadasMapaZ = this.transform.position.z;


        // Para map2grid
        offsetCasillasX = tamMapX / 2; // Offset para situar las casillas del mundo al grid y que coindan
        offsetCasillasZ = tamMapZ / 2;

        offsetCoordenadasCasillasX = this.transform.position.x / L;
        offsetCoordenadasCasillasZ = this.transform.position.z / L;



        map = new float[tamMapX, tamMapZ];

        for (int i = 0; i < tamMapX; i++)
            for (int j = 0; j < tamMapZ; j++)
                map[i, j] = -1; //Inicializamos todas las casillas a este valor

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

    private void OnDrawGizmos() // Gizmo: una línea en la dirección del objetivo
    {


        for (int i = 0; i < tamMapX; i++)
        {
            for (int j = 0; j < tamMapZ; j++)
            {
                Vector3 from = transform.position; // Origen de la línea
                coordenada cor = grid2map(i, j);
                Gizmos.color = Color.blue;        // Target Radius
                Gizmos.DrawWireSphere(new Vector3(cor.x, this.transform.position.y + 0.1f , cor.y), 0.3f);
            }
        }

        for (int i = 0; i < tamMapX; i++)
        {
            coordenada cor = grid2map(i, 0);
            Vector3 from = new Vector3(cor.x, transform.position.y+ 0.1f, cor.y); // Origen de la línea
            coordenada cor2 = grid2map(i, tamMapX);
            Vector3 to = new Vector3(cor2.x, transform.position.y + 0.1f, cor2.y); // Origen de la línea
            Gizmos.DrawLine(from,to);

        }


        for (int i = 0; i < tamMapZ; i++)
        {
            coordenada cor = grid2map(0, i);
            Vector3 from = new Vector3(cor.x, transform.position.y + 0.1f, cor.y); // Origen de la línea
            coordenada cor2 = grid2map(tamMapZ, i);
            Vector3 to = new Vector3(cor2.x, transform.position.y + 0.1f,  cor2.y); // Origen de la línea
            Gizmos.DrawLine(from, to);
        }

        /* Punto para Debug */
        coordenada c = map2grid(target.transform.position.x, target.transform.position.z);
        c = grid2map(c.x, c.y);
        c = map2grid(c.x, c.y);
        c = grid2map(c.x, c.y);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(c.x, this.transform.position.y + 0.1f, c.y), 1);
    }



}




