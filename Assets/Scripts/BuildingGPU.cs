using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BuildingGPU : MonoBehaviour // !!!ALL ANGLES IN RADIANS!!!
{
    #region fields (+ getters) for mesh generation
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private static readonly int res = 17; // 홀수로 해줘잉
    private Material material;
    public void SetMaterialColor(Color c)
    {
        material.color = c;
    }
    #endregion

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        material = GetComponent<MeshRenderer>().material;

        initializeMesh();
    }

    private void initializeMesh()
    {
        vertices = new Vector3[2 * res + 2];

        float radius = EnvSpecs.landRadius;

        for (int i = 0; i <= res; i++)
        {
            // 여기서 설정할 필요가 없으나 mesh의 bound가 좁으면 렌더링 시 시야에 없으면 무시해버림
            float arg = Mathf.PI * 2f * ((float)i / res);

            float x = radius * Mathf.Cos(arg);
            float y = -radius * BuildingSystem.angWidToHeight * Mathf.PI * 2f;
            float z = radius * Mathf.Sin(arg);

            vertices[2 * i] = new Vector3(x, y, z);

            y *= -1f;

            vertices[2 * i + 1] = new Vector3(x, y, z);

            vertices[i] = Vector3.zero;
        }

        uvs = new Vector2[vertices.Length];

        for (int i = 0; i <= res; i++)
        {
            uvs[2 * i] = new Vector2((float)i / (res - 1), 0f);
            uvs[2 * i + 1] = new Vector2((float)i / (res - 1), 1f);
        }

        triangles = new int[3 * 2 * res + 3 * (res - 1)];

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
        for (int i = 0; i <= res - 2; i++)
        {
            triangles[6 * res + 3 * i] = 1;
            triangles[6 * res + 3 * i + 1] = 2 * i + 3;
            triangles[6 * res + 3 * i + 2] = 2 * i + 5;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    public void UpdateMeshByWalkProgress(float walkerProgress, int roleIndex) // 0 <= walkerProgress < 1
    {
        float angWid = Mathf.PI * 2f;
        if (roleIndex != 0) // not sky
        {
            angWid *= Mathf.Pow(BuildingSystem.angWidStepRatio, -(float)roleIndex + walkerProgress);
        }

        int renderOrder = roleIndex <= 3 ? roleIndex : -1;

        UpdateMesh(angWid, renderOrder);
    }
    private void UpdateMesh(float angWid, int renderOrder) // 논리 기반의 베이스
    {
        material.SetFloat("_AngWid", angWid);
        material.renderQueue = 3000 + renderOrder;
    }
}