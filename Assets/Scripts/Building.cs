using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Building : MonoBehaviour // !!!ALL ANGLES IN RADIANS!!!
{
    #region fields for mesh generation
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private static readonly int res = 150; // 홀수로 해줘잉
    private Material material;
    #endregion

    #region fields for windows
    private int[] permutation;
    private float[] windowAlphas;
    #endregion

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        material = GetComponent<MeshRenderer>().material;

        initializeMesh();

        permutation = Enumerable.Range(0, 100).ToArray<int>();
        windowAlphas = new float[100];

        InitializeWindowAlphas(0f);
    }

    public void InitializeWindowAlphas(float initAlpha)
    {
        for (int i = 0; i < windowAlphas.Length; i++)
        {
            windowAlphas[i] = initAlpha;
        }

        material.SetFloatArray("_WindowAlphas", windowAlphas);
    }
    private void initializeMesh()
    {
        vertices = new Vector3[2 * res + 2];

        float radius = EnvSpecs.landRadius;

        for (int i = 0; i <= res; i++)
        {
            // vertex shader가 위치 세팅 해줘서 여기서 설정할 필요가 없으나 mesh의 bound가 좁으면 렌더링 시 시야에 없으면 무시해버림. 원통형 방어벽을 쳐준다.
            float arg = Mathf.PI * 2f * ((float)i / res);

            float x = radius * Mathf.Cos(arg);
            float y = -radius * BuildingSystemCPU.angWidToHeight * Mathf.PI * 2f;
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
    public void UpdateMesh(float walkerProgress, int roleIndex) // 0 <= walkerProgress < 1
    {
        float angWid = Mathf.PI * 2f;
        if (roleIndex != 0) // not sky
        {
            angWid *= Mathf.Pow(BuildingSystem.angWidStepRatio, -(float)roleIndex + walkerProgress);
        }
        material.SetFloat("_AngWid", angWid);

        if (roleIndex == 3) // zeroToDoor Building
        {
            transform.position = Vector3.up * (angWid * 80/*Building.shader's AngWidToHeight, to get height of the building*/ * 4f * (walkerProgress - 1f));
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }
    public void SetRenderQueue(int renderQueue)
    {
        material.renderQueue = renderQueue;
    }
    public void SetColors(Color buildingColor, Color windowColor)
    {
        material.SetColor("_Color", buildingColor);
        material.SetColor("_WindowColor", windowColor);
    }
    private IEnumerator fade(bool on, float interval)
    {
        Extensions.Shuffle(ref permutation);

        for (int i = 0; i < 2 * permutation.Length; i++)
        {
            for (int j = 0; j < Math.Min(i, permutation.Length); j++)
            {
                windowAlphas[permutation[j]] += on ? 0.04f : -0.02f;
                windowAlphas[permutation[j]] = Mathf.Clamp(windowAlphas[permutation[j]], 0f, 1f);
            }
            material.SetFloatArray("_WindowAlphas", windowAlphas);
            yield return new WaitForSeconds(interval);
        }
    }
    public void StartFade(bool on)
    {
        StopAllCoroutines();
        StartCoroutine(fade(on, 0.02f));
    }
}