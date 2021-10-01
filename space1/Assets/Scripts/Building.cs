using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Building : MonoBehaviour // !!!ALL ANGLES IN RADIANS!!!
{
    BuildingSystem bs;
    #region Fields for Mesh generation
    private Mesh mesh;
    [HideInInspector] public Vector3[] vertices;
    private int[] triangles;
    private static int res = 17; // 홀수로 해줘잉
    public int getRes()
    {
        return res;
    }
    #endregion

    #region Fields for building position and size
    private float angWid;
    #endregion

    #region Getters and Setters for current angWid
    public void setAngWid(float newValue)
    {
        angWid = Mathf.Clamp(newValue, BuildingSystem.presetAngWids[BuildingSystem.presetAngWids.Length - 1], BuildingSystem.presetAngWids[0]);
    }
    #endregion

    private void Start()
    {
        bs = BuildingSystem.instance;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void UpdateMesh(float factor, int localIndex)
    { // 0 <= factor < 1
        float angWid = BuildingSystem.presetAngWids[0] * Mathf.Pow(BuildingSystem.angWidStepRatio, -(float)localIndex + factor);
        float radius = EnvSpecs.landRadius * Mathf.Pow(BuildingSystem.raduisRatio, 3 - localIndex + factor);

        setAngWid(angWid);
        UpdateMesh(this.angWid, radius, false);

        if (localIndex == 3)
        { // zeroToDoor Building
            transform.position = Vector3.up * (radius * angWid * BuildingSystem.angWidToHeight * 4f * (factor - 1f));
        }
    }

    public void UpdateMesh(float angWid, float radius, bool coverTop)
    {
        vertices = new Vector3[2 * res + 2];
        for (int i = 0; i <= res; i++)
        {
            float arg = BuildingSystem.centerDir + angWid * (-0.5f + (float)i / res);

            float x = radius * Mathf.Cos(arg);
            float y = -1f;
            float z = radius * Mathf.Sin(arg);

            vertices[2 * i] = new Vector3(x, y, z);

            y = radius * BuildingSystem.angWidToHeight * angWid;

            vertices[2 * i + 1] = new Vector3(x, y, z);
        }

        if (coverTop)
        {
            triangles = new int[3 * 2 * res + 3 * (res - 1)];
        }
        else
        {
            triangles = new int[3 * 2 * res];
        }
        for (int i = 0; i <= res - 1; i++)
        {
            int v0 = 2 * i;
            int v1 = 2 * i + 1;
            int v2 = 2 * i + 2;
            int v3 = 2 * i + 3;

            triangles[6 * i] = v0;
            triangles[6 * i + 1] = v2;
            triangles[6 * i + 2] = v1;
            triangles[6 * i + 3] = v1;
            triangles[6 * i + 4] = v2;
            triangles[6 * i + 5] = v3;
        }
        if (coverTop)
        {
            for (int i = 0; i <= res - 2; i++)
            {
                triangles[6 * res + 3 * i] = 1;
                triangles[6 * res + 3 * i + 1] = 2 * i + 3;
                triangles[6 * res + 3 * i + 2] = 2 * i + 5;
            }
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}