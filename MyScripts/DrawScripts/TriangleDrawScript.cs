using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// This class creates a triangle-shaped mesh
/// </summary>
public class TriangleDrawScript : DrawScript
{

    MeshFilter mf;                                      // MeshFilter "mf" to create a mesh 
    Mesh mesh;                                          // Mesh "mesh" stores information of our currently created Mesh 
    Vector3[] vertices;                                 // Vector3 Array "vertices" which contains coordinates of each vertex
    int[] triangles;                                    // Int Array "triangles" which contains references to the vertices Array 
    Vector3[] trianglePattern;                          // Vector3 Array "starPattern" contains the pattern of our star
    private float offset = 0.015f;                      // float "offset" used to scale the pattern into proportion 
    //private Vector3 initPos = new Vector3(0, 0, 0);     // Vector3 "initPos stores the first position of the HTC-Vive controller

    public override void initPattern(Vector3 initPos)
    {
        Vector3 actualInitPos = new Vector3();
        if (isServer)
        {
            actualInitPos = initPos;
        }
        else if (isClient)
        {
            print("Is client");
            actualInitPos = initPos - cubePosition;
        }

        mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;

        trianglePattern = new Vector3[] {

            new Vector3(0,offset,offset),
            new Vector3(-offset,-offset,offset),
            new Vector3(offset,-offset,offset)
        };

        Vector3[] initTrianglePattern = new Vector3[] {

            trianglePattern[0] + actualInitPos,
            trianglePattern[1] + actualInitPos,
            trianglePattern[2] + actualInitPos,
        };

        vertices = new Vector3[initTrianglePattern.Length];
        System.Array.Copy(initTrianglePattern, vertices, initTrianglePattern.Length);

        triangles = new int[]
        {
			//front 
			0,1,2,  //first triangle
            0,2,1
        };

        mesh.Clear();
        mesh.vertices = vertices;           //write back added vertices
        mesh.triangles = triangles;         //write back added triangle definitions
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    public override void addNewVertexPosition(Vector3 newPosition)
    {
        Vector3 actualNewPosition = new Vector3();

        //Check if this is either the server or the client
        if (isServer)
        {
            actualNewPosition = newPosition;
        }
        else if (isClient)
        {
            print("Is client");
            actualNewPosition = newPosition - cubePosition;
        }

        int numberOFAddedVertices = 3;
        //create newVertices Array with the length of the vertices Array plus the value of the added vertices
        Vector3[] NewVertices = new Vector3[vertices.Length + numberOFAddedVertices];
        //resize the vertices array to make it big enough for the new vertices
        System.Array.Resize(ref vertices, NewVertices.Length);
        //copy the already added vertices into the NewVertices Array to change the values 
        System.Array.Copy(vertices, NewVertices, NewVertices.Length);

        NewVertices[vertices.Length - 3] = trianglePattern[0] + actualNewPosition;
        NewVertices[vertices.Length - 2] = trianglePattern[1] + actualNewPosition;
        NewVertices[vertices.Length - 1] = trianglePattern[2] + actualNewPosition;

        //copy the new vertices into the vertices Array
        System.Array.Copy(NewVertices, vertices, vertices.Length);
        //write back the added vertices
        mesh.vertices = vertices;
        //calculate normals
        mesh.RecalculateNormals();
        //call DefineTriangles to make the change visible
        DefineTriangles();
    }

    public override void DefineTriangles()
    {

        int numberOfAddedReferences = 21;
        //create newTriangles Array with the length of the triangles Array plus space for 21 new references to define a triangle.
        int[] newTriangles = new int[triangles.Length + numberOfAddedReferences];
        //resize the triangles Array to hold all new entries
        System.Array.Resize(ref triangles, newTriangles.Length);
        //copy the references from the triangles Array into the newTriangles Array 
        System.Array.Copy(triangles, newTriangles, newTriangles.Length);

        //number of all vertices
        int m = vertices.Length;
        //number of added vertices
        int n = 3;

        newTriangles[triangles.Length - 21] = m - n;        //referenziert auf einen Index im Vertices Array
        newTriangles[triangles.Length - 20] = m - 2;
        newTriangles[triangles.Length - 19] = m - 1;

        newTriangles[triangles.Length - 18] = m - 2 * n;        //referenziert auf einen Index im Vertices Array
        newTriangles[triangles.Length - 17] = m - n;
        newTriangles[triangles.Length - 16] = m - 1;
        newTriangles[triangles.Length - 15] = m - 1;
        newTriangles[triangles.Length - 14] = m - 2 * n + 2;
        newTriangles[triangles.Length - 13] = m - 2 * n;
        newTriangles[triangles.Length - 12] = m - 2 * n + 2;
        newTriangles[triangles.Length - 11] = m - 1;
        newTriangles[triangles.Length - 10] = m - 2;
        newTriangles[triangles.Length - 9] = m - 2;
        newTriangles[triangles.Length - 8] = m - 2 * n + 1;
        newTriangles[triangles.Length - 7] = m - 2 * n + 2;
        newTriangles[triangles.Length - 6] = m - 2 * n + 1;
        newTriangles[triangles.Length - 5] = m - 2;
        newTriangles[triangles.Length - 4] = m - n;
        newTriangles[triangles.Length - 3] = m - n;
        newTriangles[triangles.Length - 2] = m - 2 * n;
        newTriangles[triangles.Length - 1] = m - 2 * n + 1;


        //copy the added references into the triangles Array
        System.Array.Copy(newTriangles, triangles, triangles.Length);
        //write back the triangles into the mesh
        mesh.triangles = triangles;
        mesh.Optimize();


    }

}
