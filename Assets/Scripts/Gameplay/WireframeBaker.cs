using UnityEngine;

public class WireframeBaker : MonoBehaviour
{
    void Awake()
    {
        // 1. Try to find the MeshFilter on this object OR any child object
        MeshFilter mf = GetComponentInChildren<MeshFilter>();

        // 2. Safety Check: Did we find it?
        if (mf == null)
        {
            Debug.LogError($"[WireframeBaker] No MeshFilter found on {gameObject.name} or its children! Disabling script.");
            enabled = false;
            return;
        }

        Mesh originalMesh = mf.sharedMesh; // Use sharedMesh to access the asset safely
        if (originalMesh == null)
        {
            Debug.LogError($"[WireframeBaker] MeshFilter found on {mf.gameObject.name}, but the Mesh data is missing.");
            return;
        }

        // 3. Prepare data arrays
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;
        
        // Setup new arrays for the "unshared" vertices technique
        Vector3[] newVertices = new Vector3[triangles.Length];
        Vector3[] newBarycentric = new Vector3[triangles.Length];
        int[] newTriangles = new int[triangles.Length];

        // 4. The Baking Loop
        for (int i = 0; i < triangles.Length; i++)
        {
            // Uncouple the vertex so it's unique to this triangle
            newVertices[i] = vertices[triangles[i]];
            newTriangles[i] = i;

            // Assign Barycentric Coordinates (1,0,0), (0,1,0), (0,0,1)
            if (i % 3 == 0) newBarycentric[i] = new Vector3(1, 0, 0);
            else if (i % 3 == 1) newBarycentric[i] = new Vector3(0, 1, 0);
            else newBarycentric[i] = new Vector3(0, 0, 1);
        }

        // 5. Create and assign the new mesh
        Mesh wireframeMesh = new Mesh();
        wireframeMesh.name = originalMesh.name + "_Wireframe";
        wireframeMesh.vertices = newVertices;
        wireframeMesh.triangles = newTriangles;
        wireframeMesh.SetUVs(1, newBarycentric); // Store in UV2
        wireframeMesh.RecalculateNormals();
        
        // Assign the new mesh to the Filter we found
        mf.mesh = wireframeMesh; 
    }
}