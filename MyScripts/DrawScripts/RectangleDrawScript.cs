using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// This class creates a rectangle-shaped mesh
/// </summary>
public class RectangleDrawScript: DrawScript {

    MeshFilter mf;                                      // MeshFilter "mf" to create a mesh 
    Mesh mesh;                                          // Mesh "mesh" stores information of our currently created Mesh 
    Vector3[] vertices;                                 // Vector3 Array "vertices" which contains coordinates of each vertex
    int[] triangles;                                    // Int Array "triangles" which contains references to the vertices Array 
    Vector3[] rectanglePattern;                         // Vector3 Array "starPattern" contains the pattern of our star
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

        rectanglePattern = new Vector3[]
        {
			//front
    		new Vector3(-offset,offset,offset),
            new Vector3(offset,offset,offset),
            new Vector3(-offset,-offset,offset),
            new Vector3(offset,-offset,offset)
        };

        Vector3[] initPattern = new Vector3[] {

            rectanglePattern[0] + actualInitPos,
            rectanglePattern[1] + actualInitPos,
            rectanglePattern[2] + actualInitPos,
            rectanglePattern[3] + actualInitPos,
        };

        vertices = new Vector3[initPattern.Length];
        
        System.Array.Copy(initPattern, vertices, initPattern.Length);

        //Triangles
        triangles = new int[]
        {
			//Grundfläche Rechteck
			0,2,3,//first triangle
			3,1,0,//second triangle
            1,3,2,
            2,0,1

		};

        mesh.Clear();
        mesh.vertices = vertices;           //write back added vertices
        mesh.triangles = triangles;         //write back added triangle definitions
        mesh.Optimize();
        mesh.RecalculateNormals();
    }


        public override void addNewVertexPosition(Vector3 newPosition) {

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

        int numberOFAddedVertices = 4;
        //create newVertices Array with the length of the vertices Array plus the value of the added vertices
        Vector3[] NewVertices = new Vector3[vertices.Length + numberOFAddedVertices];
        //resize the vertices array to make it big enough for the new vertices
        System.Array.Resize(ref vertices, NewVertices.Length);
        //copy the already added vertices into the NewVertices Array to change the values 
        System.Array.Copy(vertices, NewVertices, NewVertices.Length);

        NewVertices[vertices.Length - 4] = rectanglePattern[0] + actualNewPosition;
        NewVertices[vertices.Length - 3] = rectanglePattern[1] + actualNewPosition;
        NewVertices[vertices.Length - 2] = rectanglePattern[2] + actualNewPosition;
        NewVertices[vertices.Length - 1] = rectanglePattern[3] + actualNewPosition;

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

        int numberOfAddedReferences = 30;
        //create newTriangles Array with the length of the triangles Array plus space for 30 new references to define a rectangle.
        int[] newTriangles = new int[triangles.Length + numberOfAddedReferences];
        //resize the triangles Array to hold all new entries
        System.Array.Resize(ref triangles, newTriangles.Length);
        //copy the references from the triangles Array into the newTriangles Array 
        System.Array.Copy(triangles, newTriangles, newTriangles.Length);

        //number of all vertices
        int m = vertices.Length;
        //number of added vertices
        int n = 4;
        //front
        newTriangles[triangles.Length - 30] = m - n;        
        newTriangles[triangles.Length - 29] = m - 2;           
        newTriangles[triangles.Length - 28] = m - 1;        
        newTriangles[triangles.Length - 27] = m - 1;        
        newTriangles[triangles.Length - 26] = m - 3;        
        newTriangles[triangles.Length - 25] = m - n;            

        newTriangles[triangles.Length - 24] = m - 2 * n;          
        newTriangles[triangles.Length - 23] = m - n;            
        newTriangles[triangles.Length - 22] = m - n + 1;        
        newTriangles[triangles.Length - 21] = m - 2 * n + 1;        
        newTriangles[triangles.Length - 20] = m - n + 1;        
        newTriangles[triangles.Length - 19] = m - 1;            
        newTriangles[triangles.Length - 18] = m - 2 * n + 2;        
        newTriangles[triangles.Length - 17] = m - n + 2;            
        newTriangles[triangles.Length - 16] = m - n;            
        newTriangles[triangles.Length - 15] = m - n - 1;            
        newTriangles[triangles.Length - 14] = m - 1;            
        newTriangles[triangles.Length - 13] = m - n + 2;            
        newTriangles[triangles.Length - 12] = m - n;            
        newTriangles[triangles.Length - 11] = m - 2 * n;            
        newTriangles[triangles.Length - 10] = m - 2 * n + 2;       
        newTriangles[triangles.Length - 9] = m - n + 1;        
        newTriangles[triangles.Length - 8] = m - 2 * n + 1; 
        newTriangles[triangles.Length - 7] = m - 2 * n;         
        newTriangles[triangles.Length - 6] = m - n + 2;     
        newTriangles[triangles.Length - 5] = m - 2 * n + 2;     
        newTriangles[triangles.Length - 4] = m - n - 1;     
        newTriangles[triangles.Length - 3] = m - 1;             
        newTriangles[triangles.Length - 2] = m - n - 1;         
        newTriangles[triangles.Length - 1] = m - 2 * n + 1;     


        System.Array.Copy(newTriangles, triangles, triangles.Length);
        mesh.triangles = triangles;
        mesh.Optimize();

    }

}