using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WindowsMonoSingleton : MonoSingleton<WindowsMonoSingleton>
{
    #region Fields for mesh generation
    public static Mesh mesh;
    private static Vector3[] vertices;
    private static int[] triangles;
    private MeshRenderer mr;
    #endregion

    #region Fields for windows count and position config
    private static int yNum = 8;
    private static int xMargin = 1;
    private static float bottom = 0.2f;
    private static float top = 0.9f;
    private static float winHeight = (top - bottom) / (2 * yNum - 1);
    #endregion

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mr = GetComponent<MeshRenderer>();
    }

    public void setMesh(Building building, Building matBuilding)
    {
        vertices = new Vector3[yNum * 4 * (1 + (Building.getRes() - 2 * xMargin) / 2)];
        triangles = new int[3 * vertices.Length / 2];

        Vector3[] bv = building.vertices;

        for (int x = xMargin; x < Building.getRes() - xMargin; x += 2)
        {
            Vector3[] bvs = new Vector3[4];
            bvs[0] = new Vector3(bv[2 * x].x, 0f, bv[2 * x].z);
            bvs[1] = bv[2 * x + 1];
            bvs[2] = new Vector3(bv[2 * x + 2].x, 0f, bv[2 * x + 2].z);
            bvs[3] = bv[2 * x + 3];
            for (int i = 0; i < bvs.Length; i++)
            {
                bvs[i] = Vector3.Lerp(bvs[i], Vector3.zero, 0.001f);
            }

            for (int z = 0; z < yNum; z++)
            {
                float winBottom = bottom + 2 * z * winHeight;
                float winTop = winBottom + winHeight;

                int currentWindow = yNum * (x - xMargin) / 2 + z;

                int currentVertexIndex = 4 * currentWindow;

                vertices[currentVertexIndex] = Vector3.Lerp(bvs[0], bvs[1], winBottom);
                vertices[currentVertexIndex + 1] = Vector3.Lerp(bvs[0], bvs[1], winTop);
                vertices[currentVertexIndex + 2] = Vector3.Lerp(bvs[2], bvs[3], winBottom);
                vertices[currentVertexIndex + 3] = Vector3.Lerp(bvs[2], bvs[3], winTop);

                int currentTriangleIndex = 6 * currentWindow;
                triangles[currentTriangleIndex] = currentVertexIndex;
                triangles[currentTriangleIndex + 1] = currentVertexIndex + 2;
                triangles[currentTriangleIndex + 2] = currentVertexIndex + 1;
                triangles[currentTriangleIndex + 3] = currentVertexIndex + 1;
                triangles[currentTriangleIndex + 4] = currentVertexIndex + 2;
                triangles[currentTriangleIndex + 5] = currentVertexIndex + 3;
            }
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}