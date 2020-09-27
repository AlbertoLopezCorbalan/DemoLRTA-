using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{

    [SerializeField]
    GameObject obstacles = null;
    [SerializeField]
    GameObject safe = null;

    public const int SAFE = 1;

    [SerializeField]
    GameObject startPoint = null;
    [SerializeField]
    GameObject endPoint = null;

    public float[,] map = null;
    public float L = 1.0f; // Size of grid

    private float width = 0, height = 0;

    private int sizeArrayX = 0, sizeArrayZ = 0;

    private Vector3 offset = new Vector3(0, 0, 0);

    public struct coordinate
    {
        public float x;
        public float y;
    }

    private void Awake()
    {

        if (startPoint == null || endPoint == null) return;

        width = Math.Abs(startPoint.transform.position.x - endPoint.transform.position.x) + 1;
        height = Math.Abs(startPoint.transform.position.z - endPoint.transform.position.z) + 1;

        sizeArrayX = (int)(width / L);
        sizeArrayZ = (int)(height / L);

        offset = startPoint.transform.position;



        map = new float[sizeArrayX, sizeArrayZ];

        for (int i = 0; i < sizeArrayX; i++)
            for (int j = 0; j < sizeArrayZ; j++)
                map[i, j] = 0;




        if (obstacles != null)
        {
            int size = obstacles.transform.childCount;
            for (int i = 0; i < size; i++)
            {
                GameObject g = obstacles.transform.GetChild(i).gameObject;
                coordinate c = map2grid(g.transform.position.x, g.transform.position.z);
                if (0 <= c.x && c.x < sizeArrayX && 0 <= c.y && c.y < sizeArrayZ)
                    map[(int)c.x, (int)c.y] = Mathf.Infinity;
            }
        }

        if (safe != null)
        {
            int size = safe.transform.childCount;
            for (int i = 0; i < size; i++)
            {
                GameObject g = safe.transform.GetChild(i).gameObject;
                coordinate c = map2grid(g.transform.position.x, g.transform.position.z);
                if (0 <= c.x && c.x < sizeArrayX && 0 <= c.y && c.y < sizeArrayZ)
                    map[(int)c.x, (int)c.y] = SAFE;
            }
        }



    }

    public float[,] getMapClone()
    {

        float[,] mapAux = new float[sizeArrayX, sizeArrayZ];
        for (int i = 0; i < sizeArrayX; i++)
            for (int j = 0; j < sizeArrayZ; j++)
                mapAux[i, j] = map[i, j];
        return mapAux;

    }

    public float getSizeMapX()
    {

        return sizeArrayX;

    }

    public float getSizeMapZ()
    {
        return sizeArrayZ;
    }



    public coordinate grid2map(float i, float j)
    {
        return new coordinate
        {
            x = i * L + offset.x,
            y = j * L + offset.z
        };
    }

    public coordinate map2grid(float i, float j)
    {
        return new coordinate
        {
            x = Mathf.Round(i - offset.x) / L,
            y = Mathf.Round(j - offset.z) / L
        };

    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < sizeArrayX; i++) // ***********
        {
            for (int j = 0; j < sizeArrayZ; j++)
            {

                Vector3 from = transform.position;
                coordinate cor = grid2map(i, j);

                // obstruction or safe
                if (map[i, j] == Mathf.Infinity) Gizmos.color = Color.red;
                if (map[i, j] == SAFE) Gizmos.color = Color.yellow;
                if (map[i, j] != Mathf.Infinity && map[i, j] != SAFE) Gizmos.color = Color.blue;

                Gizmos.DrawWireSphere(new Vector3(cor.x, this.transform.position.y + 0.01f, cor.y), 0.3f);
            }
        }

        for (int i = 0; i < sizeArrayX; i++) // ||||||||||||
        {
            coordinate cor = grid2map(i, 0);
            Vector3 from = new Vector3(cor.x, transform.position.y + 0.1f, cor.y);
            coordinate cor2 = grid2map(i, sizeArrayZ);
            Vector3 to = new Vector3(cor2.x, transform.position.y + 0.1f, cor2.y);
            Gizmos.DrawLine(from, to);

        }


        for (int i = 0; i < sizeArrayZ; i++) // -----------
        {
            coordinate cor = grid2map(0, i);
            Vector3 from = new Vector3(cor.x, transform.position.y + 0.1f, cor.y);
            coordinate cor2 = grid2map(sizeArrayX, i);
            Vector3 to = new Vector3(cor2.x, transform.position.y + 0.1f, cor2.y);
            Gizmos.DrawLine(from, to);
        }

    }

}
