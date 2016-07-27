using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// This class creates a star-shaped mesh
/// </summary>
public class StarDrawScript: DrawScript {

    MeshFilter mf;                                      // MeshFilter "mf" to create a mesh 
    Mesh mesh;                                          // Mesh "mesh" stores information of our currently created Mesh 
    Vector3[] vertices;                                 // Vector3 Array "vertices" which contains coordinates of each vertex
    int[] triangles;                                    // Int Array "triangles" which contains references to the vertices Array 
    Vector3[] starPattern;                              // Vector3 Array "starPattern" contains the pattern of our star
    private float offset = 0.015f;                      // float "offset" used to scale the pattern into proportion 
    //private Vector3 initPos = new Vector3(0, 0, 0);     // Vector3 "initPos stores the first position of the HTC-Vive controller


    /// <summary>
    /// initStarPattern
    /// Method that initiates a 2-dimensional star pattern at the initial
    /// position of the HTC-Vive Controller. It takes one argument.
    /// </summary>
    /// <param name="initPos">first position of the HTC-Vive Controller</param>
    public override void initPattern(Vector3 initPos) {
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

        starPattern = new Vector3[] {

			//create Star-Pattern 
			new Vector3(0,offset,offset),
            new Vector3(0.5f*offset,0,offset),
            new Vector3(1.5f*offset,0,offset),
            new Vector3(0.5f*offset,-offset,offset),
            new Vector3(offset,-2.2f*offset,offset),
            new Vector3(0,-1.5f*offset,offset),
            new Vector3(-offset,-2.2f*offset,offset),
            new Vector3(-0.5f*offset,-offset,offset),
            new Vector3(-1.5f*offset,0,offset),
            new Vector3(-0.5f*offset,0,offset)
        };


        //Star-Pattern with initial controller position
        Vector3[] initStarPattern = new Vector3[] {
			
			starPattern[0] + actualInitPos,
            starPattern[1] + actualInitPos,
            starPattern[2] + actualInitPos,
            starPattern[3] + actualInitPos,
            starPattern[4] + actualInitPos,
            starPattern[5] + actualInitPos,
            starPattern[6] + actualInitPos,
            starPattern[7] + actualInitPos,
            starPattern[8] + actualInitPos,
            starPattern[9] + actualInitPos
        };

        //adjust the length of the vertices Array
        vertices = new Vector3[initStarPattern.Length];
        //copy the star-pattern with the initial position into the vertices Array
        System.Array.Copy(initStarPattern, vertices, initStarPattern.Length);

        //First visible surface
        triangles = new int[]
        {
			//front 
			0,9,1,
            1,3,2,
            3,5,4,
            5,7,6,
            7,9,8,  // outer star
			3,7,5,  // inner star
			3,1,7,
            9,7,1,

                    //other side
            0,1,9,
            9,7,8,
            7,5,6,
            5,3,4,
            3,1,2,
            7,3,5,
            7,9,3,
            1,3,9

        };

        mesh.Clear();
        mesh.vertices = vertices;           //write back added vertices
        mesh.triangles = triangles;         //write back added triangle definitions
        mesh.Optimize();
        mesh.RecalculateNormals();

    }

    /// <summary>
    /// Adds the new vertex position.
    /// </summary>
    /// <param name="newPosition">New position of the HTC Vive controller</param>
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
            actualNewPosition = newPosition - cubePosition;
        }

        int numberOFAddedVertices = 10;
        //create newVertices Array with the length of the vertices Array plus the value of the added vertices
        Vector3[] NewVertices = new Vector3[vertices.Length + numberOFAddedVertices];
        //resize the vertices array to make it big enough for the new vertices
        System.Array.Resize(ref vertices, NewVertices.Length);
        //copy the already added vertices into the NewVertices Array to change the values 
        System.Array.Copy(vertices, NewVertices, NewVertices.Length);

        //create the pattern at the new position of the HTC-Vive controller
        NewVertices[vertices.Length - 10] = starPattern[0] + actualNewPosition;
        NewVertices[vertices.Length - 9] =  starPattern[1] + actualNewPosition;
        NewVertices[vertices.Length - 8] =  starPattern[2] + actualNewPosition;
        NewVertices[vertices.Length - 7] =  starPattern[3] + actualNewPosition;
        NewVertices[vertices.Length - 6] =  starPattern[4] + actualNewPosition;
        NewVertices[vertices.Length - 5] =  starPattern[5] + actualNewPosition;
        NewVertices[vertices.Length - 4] =  starPattern[6] + actualNewPosition;
        NewVertices[vertices.Length - 3] =  starPattern[7] + actualNewPosition;
        NewVertices[vertices.Length - 2] =  starPattern[8] + actualNewPosition;
        NewVertices[vertices.Length - 1] =  starPattern[9] + actualNewPosition;

        Debug.Log("prerotation vertices: " + NewVertices[vertices.Length - 10]);

        //copy the new vertices into the vertices Array
        System.Array.Copy(NewVertices, vertices, vertices.Length);
        //write back the added vertices
        mesh.vertices = vertices;
        //calculate normals
        mesh.RecalculateNormals();
        //call DefineTriangles to make the change visible
        DefineTriangles();

    }

    /// <summary>
    /// Defines the triangles by referencing indices of the vertices Array
    /// Triangles have to be defined counter-clock-wise to be visible
    /// </summary>
    public override void DefineTriangles()
    {
        int numberOfAddedReferences = 84;
        //create newTriangles Array with the length of the triangles Array plus space for 60 new references to define a star.
        int[] newTriangles = new int[triangles.Length + numberOfAddedReferences];
        //resize the triangles Array to hold all new entries
        System.Array.Resize(ref triangles, newTriangles.Length);
        //copy the references from the triangles Array into the newTriangles Array 
        System.Array.Copy(triangles, newTriangles, newTriangles.Length);

        //number of all vertices
        int m = vertices.Length;
        //number of added vertices
        int n = 10;

        //reference to a index of the vertices Array
        newTriangles[triangles.Length - 84] = m - n;
        newTriangles[triangles.Length - 83] = m - 1;
        newTriangles[triangles.Length - 82] = m - n + 1;

        newTriangles[triangles.Length - 81] = m - 9;
        newTriangles[triangles.Length - 80] = m - 7;
        newTriangles[triangles.Length - 79] = m - 8;

        newTriangles[triangles.Length - 78] = m - 7;
        newTriangles[triangles.Length - 77] = m - 5;
        newTriangles[triangles.Length - 76] = m - 6;

        newTriangles[triangles.Length - 75] = m - 5;
        newTriangles[triangles.Length - 74] = m - 3;
        newTriangles[triangles.Length - 73] = m - 4;

        newTriangles[triangles.Length - 72] = m - 3;
        newTriangles[triangles.Length - 71] = m - 1;
        newTriangles[triangles.Length - 70] = m - 2;

        newTriangles[triangles.Length - 69] = m - 7;
        newTriangles[triangles.Length - 68] = m - 3;
        newTriangles[triangles.Length - 67] = m - 5;

        newTriangles[triangles.Length - 66] = m - 7;
        newTriangles[triangles.Length - 65] = m - 9;
        newTriangles[triangles.Length - 64] = m - 3;

        newTriangles[triangles.Length - 63] = m - 1;
        newTriangles[triangles.Length - 62] = m - 3;
        newTriangles[triangles.Length - 61] = m - 9;

        newTriangles[triangles.Length - 60] = m - 2 * n;
        newTriangles[triangles.Length - 59] = m - n;
        newTriangles[triangles.Length - 58] = m - n + 1;
        newTriangles[triangles.Length - 57] = m - n + 1;
        newTriangles[triangles.Length - 56] = m - 2 * n + 1;
        newTriangles[triangles.Length - 55] = m - 2 * n;
        newTriangles[triangles.Length - 54] = m - 2 * n + 1;
        newTriangles[triangles.Length - 53] = m - n + 1;
        newTriangles[triangles.Length - 52] = m - n + 2;
        newTriangles[triangles.Length - 51] = m - n + 2;
        newTriangles[triangles.Length - 50] = m - 2 * n + 2;
        newTriangles[triangles.Length - 49] = m - 2 * n + 1;
        newTriangles[triangles.Length - 48] = m - 2 * n + 2;
        newTriangles[triangles.Length - 47] = m - n + 2;
        newTriangles[triangles.Length - 46] = m - n + 3;
        newTriangles[triangles.Length - 45] = m - n + 3;
        newTriangles[triangles.Length - 44] = m - 2 * n + 3;
        newTriangles[triangles.Length - 43] = m - 2 * n + 2;
        newTriangles[triangles.Length - 42] = m - 2 * n + 3;
        newTriangles[triangles.Length - 41] = m - n + 3;
        newTriangles[triangles.Length - 40] = m - n + 4;
        newTriangles[triangles.Length - 39] = m - n + 4;
        newTriangles[triangles.Length - 38] = m - 2 * n + 4;
        newTriangles[triangles.Length - 37] = m - 2 * n + 3;
        newTriangles[triangles.Length - 36] = m - 2 * n + 4;
        newTriangles[triangles.Length - 35] = m - n + 4;
        newTriangles[triangles.Length - 34] = m - n + 5;
        newTriangles[triangles.Length - 33] = m - n + 5;
        newTriangles[triangles.Length - 32] = m - 2 * n + 5;
        newTriangles[triangles.Length - 31] = m - 2 * n + 4;
        newTriangles[triangles.Length - 30] = m - 2 * n + 5;
        newTriangles[triangles.Length - 29] = m - n + 5;
        newTriangles[triangles.Length - 28] = m - n + 6;
        newTriangles[triangles.Length - 27] = m - n + 6;
        newTriangles[triangles.Length - 26] = m - 2 * n + 6;
        newTriangles[triangles.Length - 25] = m - 2 * n + 5;
        newTriangles[triangles.Length - 24] = m - 2 * n + 6;
        newTriangles[triangles.Length - 23] = m - n + 6;
        newTriangles[triangles.Length - 22] = m - n + 7;
        newTriangles[triangles.Length - 21] = m - n + 7;
        newTriangles[triangles.Length - 20] = m - 2 * n + 7;
        newTriangles[triangles.Length - 19] = m - 2 * n + 6;
        newTriangles[triangles.Length - 18] = m - 2 * n + 7;
        newTriangles[triangles.Length - 17] = m - n + 7;
        newTriangles[triangles.Length - 16] = m - n + 8;
        newTriangles[triangles.Length - 15] = m - n + 8;
        newTriangles[triangles.Length - 14] = m - 2 * n + 8;
        newTriangles[triangles.Length - 13] = m - 2 * n + 7;
        newTriangles[triangles.Length - 12] = m - 2 * n + 8;
        newTriangles[triangles.Length - 11] = m - n + 8;
        newTriangles[triangles.Length - 10] = m - n + 9;
        newTriangles[triangles.Length - 9] = m - n + 9;
        newTriangles[triangles.Length - 8] = m - 2 * n + 9;
        newTriangles[triangles.Length - 7] = m - 2 * n + 8;
        newTriangles[triangles.Length - 6] = m - 2 * n + 9;
        newTriangles[triangles.Length - 5] = m - n + 9;
        newTriangles[triangles.Length - 4] = m - n;
        newTriangles[triangles.Length - 3] = m - n;
        newTriangles[triangles.Length - 2] = m - 2 * n;
        newTriangles[triangles.Length - 1] = m - 2 * n + 9;

        //copy the added references into the triangles Array
        System.Array.Copy(newTriangles, triangles, triangles.Length);
        //write back the triangles into the mesh
        mesh.triangles = triangles;
        mesh.Optimize();

    }

}
