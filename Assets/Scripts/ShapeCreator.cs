using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShapeCreator : MonoBehaviour {

    public static GameObject GetCircle(Color c, float scale = 1f) {
        return CreateCircle(0.75f*scale, c);
    }
        
    public static GameObject GetSphere(Color c, float scale = 1f) {
        return CreateSphere(0.75f*scale, c);
    }

    public static GameObject GetRectangle(Color c, float scale = 1f) {
        return CreateRectangle(new Vector3(2f*scale,1f*scale,0.1f), c);
    }

    public static GameObject GetSquare(Color c, float scale = 1f) {
        return CreateSquare(0.5f*scale,0.1f, c);
    }

    public static GameObject GetPyramid(Color c, float scale = 1f) {
        return CreatePyramid(new Vector3(0.5f,0.75f,0.5f)*scale, c);
    }

    static GameObject CreateCircle(float radius, Color color) {
        GameObject circle = CreateSphere(radius, 20, 50,color,"Circle ");
        circle.transform.localScale = new Vector3(circle.transform.localScale.x, circle.transform.localScale.y, 0.1f);
        return circle;
    }

    static GameObject CreateSphere(float radius, Color color) {
        return CreateSphere(radius, 20, 50, color);
    }

    static GameObject CreateSphere(float radius, int slices, int stacks,Color color, string name = "Sphere ") {
        GameObject sphere = new GameObject(name + radius);
        MeshFilter mf = sphere.AddComponent<MeshFilter>();
        MeshRenderer mr = sphere.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Diffuse"));
        mat.color = color;
        mr.material = mat;
        Mesh mesh = new Mesh();

        float sliceStep = (float)Math.PI * 2.0f / slices;
        float stackStep = (float)Math.PI / stacks;
        int vertexCount = slices * (stacks - 1) + 2;
        int triangleCount = slices * (stacks - 1) * 2;
        int indexCount = triangleCount * 3;

        Vector3[] sphereVertices = new Vector3[vertexCount];

        int currentVertex = 0;
        sphereVertices[currentVertex] = new Vector3(0, -radius, 0);
        currentVertex++;
        float stackAngle = (float)Math.PI - stackStep;
        for (int i = 0; i < stacks - 1; i++) {
            float sliceAngle = 0;
            for (int j = 0; j < slices; j++) {
                //NOTE: y and z were switched from normal spherical coordinates because the sphere is "oriented" along the Y axis as opposed to the Z axis
                float x = (float)(radius * Math.Sin(stackAngle) * Math.Cos(sliceAngle));
                float y = (float)(radius * Math.Cos(stackAngle));
                float z = (float)(radius * Math.Sin(stackAngle) * Math.Sin(sliceAngle));

                Vector3 position = new Vector3(x, y, z);
                sphereVertices[currentVertex] = position;

                currentVertex++;

                sliceAngle += sliceStep;
            }
            stackAngle -= stackStep;
        }
        sphereVertices[currentVertex] = new Vector3(0, radius, 0);

        mesh.vertices = sphereVertices;
        mesh.triangles = CreateIndexBuffer(vertexCount, indexCount, slices);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        mf.mesh = (mesh);
        return sphere;

        int[] CreateIndexBuffer(int vertexCount, int indexCount, int slices) {
            int[] indices = new int[indexCount];
            int currentIndex = 0;

            // Bottom circle/cone of shape
            for (int i = 1; i <= slices; i++) {
                indices[currentIndex++] = i;
                indices[currentIndex++] = 0;
                if (i - 1 == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
            }

            // Middle sides of shape
            for (int i = 1; i < vertexCount - slices - 1; i++) {
                indices[currentIndex++] = i + slices;
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;

                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;
            }

            // Top circle/cone of shape
            for (int i = vertexCount - slices - 1; i < vertexCount - 1; i++) {
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                indices[currentIndex++] = vertexCount - 1;
            }

            return indices;
        }
    }

    static GameObject CreateRectangle(Vector3 size, Color color) {
        return CreateCube(size, color,"Rectangle ");
    }

    static GameObject CreateSquare(float width,float zLength,Color color) {
        return CreateCube(new Vector3(width, width, zLength), color,"Square ");
    }

    static GameObject CreateCube(Vector3 size, Color color,string name = "Cube ") {
        GameObject cube = new GameObject(name+size);
        MeshFilter mf = cube.AddComponent<MeshFilter>();
        MeshRenderer mr = cube.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Diffuse"));
        mat.color = color;
        mr.material = mat;

        var mesh = new Mesh();//2) Define the cube's dimensions

        float w = size.x;
        float h = size.y;
        float l = size.z;


        //3) Define the co-ordinates of each Corner of the cube 
        Vector3[] c = new Vector3[8];

        c[0] = new Vector3(-w , -h , l );
        c[1] = new Vector3(w , -h , l );
        c[2] = new Vector3(w , -h , -l );
        c[3] = new Vector3(-w , -h , -l );

        c[4] = new Vector3(-w , h , l );
        c[5] = new Vector3(w , h , l );
        c[6] = new Vector3(w , h , -l );
        c[7] = new Vector3(-w , h , -l );


        //4) Define the vertices that the cube is composed of:
        //I have used 16 vertices (4 vertices per side). 
        //This is because I want the vertices of each side to have separate normals.
        //(so the object renders light/shade correctly) 
        mesh.vertices = new Vector3[]
        {
            c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top
        };


        //5) Define each vertex's Normal
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 forward = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;


        Vector3[] normals = new Vector3[]
        {
            down, down, down, down,             // Bottom
	        left, left, left, left,             // Left
	        forward, forward, forward, forward,	// Front
	        back, back, back, back,             // Back
	        right, right, right, right,         // Right
	        up, up, up, up	                    // Top
        };


        //6) Define each vertex's UV co-ordinates
        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv10 = new Vector2(1f, 0f);
        Vector2 uv01 = new Vector2(0f, 1f);
        Vector2 uv11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
            uv11, uv01, uv00, uv10, // Bottom
	        uv11, uv01, uv00, uv10, // Left
	        uv11, uv01, uv00, uv10, // Front
	        uv11, uv01, uv00, uv10, // Back	        
	        uv11, uv01, uv00, uv10, // Right 
	        uv11, uv01, uv00, uv10  // Top
        };


        //7) Define the Polygons (triangles) that make up the our Mesh (cube)
        //IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
        //This means that a polygon's vertices must be defined in 
        //a clockwise order (relative to the camera) in order to be rendered/visible.
        mesh.triangles = new int[]
        {
            3, 1, 0,        3, 2, 1,        // Bottom	
	        7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,	    // Top
        };


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        mf.mesh = (mesh);

        return cube;
    }

    static GameObject CreatePyramid(Vector3 size,Color color) {
        GameObject pyramid = new GameObject("Pyramid "+size);
        MeshFilter mf = pyramid.AddComponent<MeshFilter>();
        MeshRenderer mr = pyramid.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Diffuse"));
        mat.color = color;
        mr.material = mat;

        var m = new Mesh();

        float w = size.x;
        float h = size.y;
        float l = size.z;

        Vector3[] pyramidPoints = new Vector3[] {
            new Vector3(-w, 0, -l),
            new Vector3(w, 0, -l),
            new Vector3(w, 0, l),
            new Vector3(-w, 0, l),
            new Vector3(0, h, 0)
        };

        m.vertices = new Vector3[] {
            pyramidPoints[0], pyramidPoints[1], pyramidPoints[2],
            pyramidPoints[0], pyramidPoints[2], pyramidPoints[3],
            pyramidPoints[0], pyramidPoints[1], pyramidPoints[4],
            pyramidPoints[1], pyramidPoints[2], pyramidPoints[4],
            pyramidPoints[2], pyramidPoints[3], pyramidPoints[4],
            pyramidPoints[3], pyramidPoints[0], pyramidPoints[4]
        };

        m.triangles = new int[] {
            0, 1, 2,
            3, 4, 5,
            8, 7, 6,
            11, 10, 9,
            14, 13, 12,
            17, 16, 15
        };

        m.RecalculateNormals();
        m.RecalculateBounds();
        m.Optimize();
        mf.mesh = (m);
        return pyramid;
    }

}