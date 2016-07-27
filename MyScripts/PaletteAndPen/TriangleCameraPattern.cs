using UnityEngine;
using System.Collections;

public class TriangleCameraPattern : MonoBehaviour {

	MeshFilter mf;
	Mesh mesh;
	// Use this for initialization
	void Start () {

		mf = GetComponent<MeshFilter>();
		mesh = mf.mesh;

		//Vertices
		Vector3[] vertices = new Vector3[]
		{
			//front
			new Vector3(0,1,1),  //top
			new Vector3(-1,-1,1), //left bottom front
			new Vector3(1,-1,1)   //right bottom front
		};

		//Triangles
		int[] triangles = new int[]
		{
			//front 
			0,1,2//first triangle

		};

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.Optimize();
		mesh.RecalculateNormals();


	}
}
