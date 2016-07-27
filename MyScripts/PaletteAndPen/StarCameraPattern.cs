using UnityEngine;
using System.Collections;

public class StarCameraPattern : MonoBehaviour {

	MeshFilter mf;
	Mesh mesh;
	Vector3[] starPattern;
	Vector3[] vertices;	
	int[] triangles;


	void Start () {

		mf = GetComponent<MeshFilter>();
		mesh = mf.mesh;

		//Vertices
		starPattern = new Vector3[] {

			//Grundmuster
			new Vector3(0,1,1), 
			new Vector3(0.5f,0,1),   
			new Vector3(1.5f,0,1),
			new Vector3(0.5f,-1,1),  
			new Vector3(1,-2.2f,1),   
			new Vector3(0,-1.5f,1),
			new Vector3(-1,-2.2f,1),  
			new Vector3(-0.5f,-1,1),   
			new Vector3(-1.5f,0,1), 
			new Vector3(-0.5f,0,1)   
		};

		//zu extrudierendes Pattern nach vertices kopieren
		vertices = new Vector3[starPattern.Length];
		System.Array.Copy (starPattern, vertices, starPattern.Length);

		//Triangles
		triangles = new int[]
		{
			//front 
			0,9,1,
			1,3,2,
			3,5,4,
			5,7,6,
			7,9,8, // äußerer stern
			3,7,5,  // innerer stern
			3,1,7,
			9,7,1

		};



		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.Optimize();
		mesh.RecalculateNormals();


	}
}
