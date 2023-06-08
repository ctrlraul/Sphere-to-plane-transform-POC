using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Script : MonoBehaviour
{
    private Mesh mesh;
    
    private Vector3[] sphereVertices;
    private Vector3[] plainVertices;
    private int[] triangles;
    private Vector2[] uv;

    private float radius = 10;
    private int detail = 40;
    private float aspectRatio = 1.9f;
    
    private int vertexCount => (int)Math.Pow(detail + 1, 2);


    void Start()
    {
        mesh = new Mesh();
        
        GetComponent<MeshFilter>().mesh = mesh;

        plainVertices = MakePlainVertices();
        sphereVertices = MakeSphereVertices();
        triangles = MakeTriangles();
        uv = MakeUV();
    }


    private void Update()
    {
        float weight = BezierInOut(Time.frameCount % 180 / 180f);
        Vector3[] vertices = new Vector3[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = Vector3.Lerp(sphereVertices[i], plainVertices[i], weight);
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        
        mesh.RecalculateNormals();
    }

    
    private Vector3[] MakeSphereVertices()
    {
        Vector3[] result = new Vector3[vertexCount];

        for (int i = 0, zCoord = 0; zCoord <= detail; zCoord++)
        {
            for (int xCoord = 0; xCoord <= detail; xCoord++)
            {
                float xScale = (xCoord - detail * 0.5f) / detail;
                float zScale = (zCoord - detail * 0.5f) / detail;
                
                float zRadii = Mathf.PI * 2 * zScale;

                float x = Mathf.Cos(Mathf.PI * xScale) * Mathf.Cos(zRadii) * radius * -1;
                float y = Mathf.Sin(Mathf.PI * xScale) * radius;
                float z = Mathf.Sin(zRadii) * Mathf.Cos(Mathf.PI * xScale) * radius;
                
                result[i] = new Vector3(x, y, z);
                
                i++;
            }
        }

        return result;
    }
    
    
    private Vector3[] MakePlainVertices()
    {
        Vector3[] result = new Vector3[vertexCount];

        for (int i = 0, zBase = 0; zBase <= detail; zBase++)
        {
            for (int yBase = 0; yBase <= detail; yBase++)
            {
                float y = (yBase - detail * 0.5f) / detail * radius * 2;
                float z = (zBase - detail * 0.5f) / detail * radius * 2 * aspectRatio;
                result[i] = new Vector3(-radius, y, z);
                i++;
            }
        }

        return result;
    }

    
    private int[] MakeTriangles()
    {
        int[] result = new int[detail * detail * 6];

        int vert = 0;
        int tris = 0;
        
        for (int z = 0; z < detail; z++)
        {
            for (int x = 0; x < detail; x++)
            {
                result[tris + 0] = vert + 0;
                result[tris + 1] = vert + detail + 1;
                result[tris + 2] = vert + 1;
                result[tris + 3] = vert + 1;
                result[tris + 4] = vert + detail + 1;
                result[tris + 5] = vert + detail + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        return result;
    }

    
    private Vector2[] MakeUV()
    {
        Vector2[] result = new Vector2[vertexCount];
        
        for (int i = 0, z = 0; z <= detail; z++)
        {
            for (int x = 0; x <= detail; x++)
            {
                result[i] = new Vector2(1 - (float)z / detail, (float)x / detail);
                i++;
            }
        }

        return result;
    }

    
    private float BezierInOut(float t)
    {
        return t * t * (3.0f - 2.0f * t);
    }

    
    // private void OnDrawGizmos()
    // {
    //     foreach (Vector3 v in mesh.vertices)
    //     {
    //         Gizmos.DrawSphere(v, 0.1f);
    //     }
    // }
}
