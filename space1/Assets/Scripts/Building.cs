using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Building : MonoBehaviour // !!!ALL ANGLES IN RADIANS!!!
{
    #region fields (+ getters) for mesh generation
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private static int res = 17; // 홀수로 해줘잉
    public Vector3[] GetVertices()
    {
        return vertices;
    }
    public static int GetRes()
    {
        return res;
    }
    private Material material;
    public Material GetMaterial()
    {
        return material;
    }
    public void SetMaterialColor(Color c)
    {
        material.color = c;
    }
    public void SetRenderQueue(int order)
    {
        material.renderQueue = 3000 + order;
    }
    #endregion

    #region the one definitive field to determine building size & its setter
    private float angWid;
    public void SetAngWid(float newValue)
    {
        angWid = Mathf.Clamp(newValue, BuildingSystem.presetAngWids[BuildingSystem.presetAngWids.Length - 1], Mathf.PI * 2f /*BuildingSystem.presetAngWids[0]*/);
    }
    #endregion

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.MarkDynamic();

        material = GetComponent<MeshRenderer>().material;
    }

    public void UpdateMesh(float factor, int roleIndex) // 0 <= factor < 1
    {
        float angWid = Mathf.PI * 2f;
        if (roleIndex != 0) // not sky
        {
            angWid *= Mathf.Pow(BuildingSystem.angWidStepRatio, -(float)roleIndex + factor);
        }

        float radius = EnvSpecs.landRadius * Mathf.Pow(BuildingSystem.raduisStepRatio, 3 - roleIndex + factor);

        SetAngWid(angWid);
        UpdateMesh(this.angWid, radius, true);

        if (roleIndex == 3) // zeroToDoor Building
        {
            transform.position = Vector3.up * (radius * angWid * BuildingSystem.angWidToHeight * 4f * (factor - 1f));
        }
    }
    private void UpdateMesh(float angWid, float radius, bool coverTop) // 논리 기반의 베이스
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
}