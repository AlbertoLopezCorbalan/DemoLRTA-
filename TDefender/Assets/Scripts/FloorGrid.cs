using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGrid : MonoBehaviour
{

    [SerializeField]
    GameObject obstruction = null;

    public float[,] map = null;
    public float L = 1.0f; // Size of grid
    private int sizeMapX, sizeMapZ;

    private float offsetMapX, offsetMapZ; //Desplazamiento de coordenadas de unity(centro del objeto)  respecto al grid (esquina inferior)
    private float offsetCoordinateMapX, offsetCoordinateMapZ; // Coordenadas mapa respecto el punto 00 de unity (transform.position)
    private float offsetGridX, offsetGridZ;
    private float offsetCoordinateGridX, offsetCoordinateGridZ; // Coordenadas mapa respecto el punto 00 de unity (transform.position)

    public struct coordinate
    {
        public float x;
        public float y;
    }

    private void Awake()
    {

        sizeMapX = (int)Mathf.Round(this.transform.localScale.x / L);
        sizeMapZ = (int)Mathf.Round(this.transform.localScale.z / L);

        // Para map2grid
        offsetGridX = sizeMapX / 2; // Offset para situar las casillas del mundo al grid y que coindan
        offsetGridZ = sizeMapZ / 2;

        // Para grid2map
        offsetMapX = (this.transform.localScale.x / 2); // Offset para el eje de coordenadas esté entre -N y N, mientras que nuestro array sea 0-N*2
        offsetMapZ = (this.transform.localScale.z / 2);

        offsetCoordinateMapX = this.transform.position.x; // Offset para acoplar el grid al suelo dentro del mundo
        offsetCoordinateMapZ = this.transform.position.z;


        offsetCoordinateGridX = this.transform.position.x / L;
        offsetCoordinateGridZ = this.transform.position.z / L;



        map = new float[sizeMapX, sizeMapZ];

        for (int i = 0; i < sizeMapX; i++)
            for (int j = 0; j < sizeMapZ; j++)
                map[i, j] = 0; //Inicializamos todas las casillas a este valor



        /*Recorremos la lista de obstáculos para ponerlos en grid
        if (obstruction != null)
        {
            int numPuntos = obstruction.transform.childCount;
            for (int i = 0; i < numPuntos; i++)
            {
                GameObject g = obstruction.transform.GetChild(i).gameObject;
                coordinate c = map2grid(g.transform.position.x, g.transform.position.z);
                if (0 <= c.x && c.x < sizeMapX && 0 <= c.y && c.y < sizeMapZ)
                    //Esta dentro de los limites del array
                    map[(int)c.x, (int)c.y] = Mathf.Infinity; //Ponemos -1 a las casillas en las que haya un obstáculo


            }
        }*/

    }

    public float[,] getMapClone() //Devolvemos una copia por valor del mapa
    {

        float[,] mapAux = new float[sizeMapX, sizeMapZ];
        for (int i = 0; i < sizeMapX; i++)
            for (int j = 0; j < sizeMapZ; j++)
                mapAux[i, j] = map[i,j]; //Inicializamos todas las casillas al valor de la original (que tiene en cuenta los obstruction)
        return mapAux;

    }

    public float getTamMapX()
    {

        return sizeMapX;

    }

    public float getTamMapZ()
    {
        return sizeMapZ;
    }



    public coordinate grid2map(float i, float j) //tranforma la casilla del array en valores del mapa de unity
    {
        return new coordinate
        {
            x = i * L - offsetMapX  + offsetCoordinateMapX, //El offset del mapa es para transformar teniendo en cuenta que en unity el objeto se empieza a dibujar por el centro del mismo, en lugar de la esquina inferior izquierda
            y = j * L - offsetMapZ + offsetCoordinateMapZ
        };
    }

    public coordinate map2grid(float i, float j) // ¿ A qué casilla del array corresponde una posición del mapa?
    {
        return new coordinate
        {
            x = Mathf.Round(i/L + offsetGridX - offsetCoordinateGridX),   //Al sumarle el offset nos situamos en la casilla correspondiente
            y = Mathf.Round(j/L + offsetGridZ - offsetCoordinateGridZ)
        };

    }





    private void OnDrawGizmos() // Gizmo: una línea en la dirección del target
    {
        for (int i = 0; i < sizeMapX; i++)
        {
            for (int j = 0; j < sizeMapZ; j++)
            {
                Vector3 from = transform.position; // Origen de la línea
                coordinate cor = grid2map(i, j);
                Gizmos.color = Color.blue;        // Target Radius
                Gizmos.DrawWireSphere(new Vector3(cor.x, this.transform.position.y + 0.1f , cor.y), 0.3f);
            }
        }

        for (int i = 0; i < sizeMapX; i++)
        {
            coordinate cor = grid2map(i, 0);
            Vector3 from = new Vector3(cor.x, transform.position.y+ 0.1f, cor.y); // Origen de la línea
            coordinate cor2 = grid2map(i, sizeMapX);
            Vector3 to = new Vector3(cor2.x, transform.position.y + 0.1f, cor2.y); // Origen de la línea
            Gizmos.DrawLine(from,to);

        }


        for (int i = 0; i < sizeMapZ; i++)
        {
            coordinate cor = grid2map(0, i);
            Vector3 from = new Vector3(cor.x, transform.position.y + 0.1f, cor.y); // Origen de la línea
            coordinate cor2 = grid2map(sizeMapZ, i);
            Vector3 to = new Vector3(cor2.x, transform.position.y + 0.1f,  cor2.y); // Origen de la línea
            Gizmos.DrawLine(from, to);
        }


        /*Recorremos la lista de obstáculos para ponerlos en grid
        if (obstruction != null)
        {
            int numPuntos = obstruction.transform.childCount;
            for (int i = 0; i < numPuntos; i++)
            {
                GameObject g = obstruction.transform.GetChild(i).gameObject;
                coordinate c = map2grid(g.transform.position.x, g.transform.position.z);

                c = grid2map(c.x, c.y);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(new Vector3(c.x, transform.position.y + 0.1f, c.y), 1);
            }
        }*/



    }

}




