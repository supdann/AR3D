using UnityEngine;
using System.Collections;

public class RectangleCameraPattern : MonoBehaviour {

	MeshFilter mf;
	Mesh mesh;


	// Use this for initialization
	void Start () {

		mf = GetComponent<MeshFilter>();
		mesh = mf.mesh;


		Vector3[] vertices = new Vector3[]
		{
			//front
			new Vector3(-1,1,1),  //left top front
			new Vector3(1,1,1),   //right top front
			new Vector3(-1,-1,1), //left bottom front
			new Vector3(1,-1,1)   //right bottom front
		};

		//Triangles
		int[] triangles = new int[]
		{
			//front 
			0,2,3,//first triangle
			3,1,0//second triangle
		};
			
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.Optimize();
		mesh.RecalculateNormals();
}
}
