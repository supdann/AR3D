# AR3D
Die Dokumentation beinhaltet nur Ausschnitte des Codes die zum Verständnis relevant sind!

**Meshes erstellen am Beispiel eines Sterns:**

Die grundlegenden Elemente beim Erstellen eines Meshes sind die Eckpunkte (vertices), Dreiecke (triangles) und die Oberflächennormalen (normals). Um ein Grundmuster zu definieren haben wir Vektoren definiert, die 10 Eckpunkte eines Sternes repräsentieren:
 

```cs
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
```

Der Wert der Variablen „offset“ dient uns hierbei als Skalierungsfaktor um die Größe des Grundmusters schnell anzupassen. Bei unseren anderen Formen sind wir natürlich genauso vorgegangen. Um unsere Grundform nun an der aktuellen Position des HTC-Vive Controllers anzeigen zu lassen, mussten wir die initiale Position, beim ersten „Malimpuls“, zu unserem Grundmuster addieren und diese Werte anschließend in ein Vector3 Array kopieren, welches alle Eckpunkte unseres Meshes beinhaltet:

```cs
//Star-Pattern with initial controller position
Vector3[] initStarPattern = new Vector3[] {

  starPattern[0] + initPos,
  starPattern[1] + initPos,
  starPattern[2] + initPos,
  starPattern[3] + initPos,
  starPattern[4] + initPos,
  starPattern[5] + initPos,
  starPattern[6] + initPos,
  starPattern[7] + initPos,
  starPattern[8] + initPos,
  starPattern[9] + initPos
};
System.Array.Copy(initStarPattern, vertices, initStarPattern.Length);
```

Nachdem wir unsere ersten Eckpunkte hinzugefügt haben können wir damit anfangen Dreiecke zu definieren. Dies tun wir mittels eines Integer Arrays, dessen werte Referenzen, zu Indexen, des vorher genannten vertices Vector3 Arrays sind. Drei aufeinanderfolgende Referenzen bilden dabei jeweils ein sichtbares Dreieck. An dieser Stelle haben wir bereits ein 2-dimensionales Mesh erstellt. 


```cs
//First visible surface
triangles = new int[] {

//front 
	0,9,1,
  1,3,2,
  3,5,4,
  5,7,6,
  7,9,8, // outer star
	3,7,5, // inner star
	3,1,7,
  9,7,1,
                   
  0,1,9, //other side
  9,7,8,
  7,5,6,
  5,3,4,
  3,1,2,
  7,3,5,
  7,9,3,
  1,3,9
};
```



Der nächste logische Schritt ist es also eine dritte Dimension hinzuzufügen und das natürlich an der Stelle, wo der Nutzer entschieden hat den Controller hinzubewegen. In der folgenden Methode Nutzen wir ein interims-Array, um neue Eckpunkte hinzufügen zu können. Ähnlich wie beim initStarPattern addieren wir nun unsere neue Controller Position zu unserem Grundmuster hinzu, um auch am neuen Punkt wieder unser Sternenmuster generieren zu können. Nach diesem Schritt wäre es nun eigentlich nötig auch die Rotation des Controllers zu berücksichtigen, jedoch sind wir auf zu viele Probleme mit dem Rotationswert des Controllers gestoßen und konnten diese leider nicht mehr rechtzeitig lösen. 
Aber auch ohne die Rotation zu berücksichtigen müssen natürlich nun die Dreiecke des Meshes definiert werden, um nicht nur eine Fläche zu sehen, sondern einen 3-dimensionalen Körper. Dies können wir natürlich nicht so wie bei unseren ersten 10 Eckpunkten von Hand erledigen, da es deutlich mehr Referenzen sind, die bei jeder neuen Position des Controllers erneut berechnet werden müssen. 


```cs
    public override void addNewVertexPosition(Vector3 newPosition)
    {
        int numberOFAddedVertices = 10
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

        //copy the new vertices into the vertices Array
        System.Array.Copy(NewVertices, vertices, vertices.Length);
        //write back the added vertices
        mesh.vertices = vertices;
     
    }
```

Nun kommen wir also zur Kernmethode unseres Mesh-Erzeugungsscriptes, nämlich der Dreiecksberechnung beim „malen“. Die Berechnung der Index-Positionen war hierbei das Hauptproblem, aber auch die Reihenfolge der Referenzierung war natürlich wichtig, weil das Mesh nur von der Außenseite sichtbar sein sollte. Die Berechnung nehmen wir durch Zuhilfenahme der Variablen m, welche die Anzahl aller Eckpunkte repräsentiert und der Variablen n, welche die Anzahl der neu Hinzugefügten Eckpunkte repräsentiert, vor. So werden nach und nach die Außenflächen und auch die abschließende Vorderseite definiert. 
Diese Methode muss nach jedem Aufruf der addNewVertexPosition Methode aufgerufen werden, um ein durchgängiges Mesh garantieren zu können. 


```cs
public override void DefineTriangles(){

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
```

Dies schließt unsere grundlegende Mesherzeugung ab und erklärt hoffentlich unsere Gedanken, beim programmieren unserer Grundstruktur. Damit geht es dann weiter mit der Menüführung:
