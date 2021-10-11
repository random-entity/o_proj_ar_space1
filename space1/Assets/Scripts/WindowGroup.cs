using UnityEngine;

public class WindowGroup : MonoBehaviour
{
    private Quad[,] quads;
    [SerializeField] private Quad quadPrefab;

    private static int yNum = 8;
    private static int xMargin = 1;
    private static float bottom = 0.2f;
    private static float top = 0.9f;
    private static float winHeight = (top - bottom) / (2 * yNum - 1);

    private void Awake()
    {
        quads = new Quad[1 + (Building.GetRes() - 2 * xMargin) / 2, yNum];

        for (int x = 0; x < quads.GetLength(0); x++)
        {
            for (int y = 0; y < quads.GetLength(1); y++)
            {
                Quad newQuad = Instantiate(quadPrefab);

                quads[x, y] = newQuad;
                newQuad.transform.SetParent(transform);
            }
        }
    }

    public void SetMeshes(Building building, Building matBuilding)
    {
        Vector3[] bv = building.GetVertices();

        for (int x = 0; x < quads.GetLength(0); x++)
        {
            int bvStart = (xMargin + 2 * x) * 2;

            Vector3[] bvs = new Vector3[4];
            bvs[0] = new Vector3(bv[bvStart].x, 0f, bv[bvStart].z);
            bvs[1] = bv[bvStart + 1];
            bvs[2] = new Vector3(bv[bvStart + 2].x, 0f, bv[bvStart + 2].z);
            bvs[3] = bv[bvStart + 3];
            // for (int i = 0; i < bvs.Length; i++)
            // {
            //     bvs[i] = Vector3.Lerp(bvs[i], Vector3.up * bvs[i].y, 0.05f);
            // } // Shader RenderQueue 세팅으로 해결

            for (int y = 0; y < quads.GetLength(1); y++)
            {
                float winBottom = bottom + 2 * y * winHeight;
                float winTop = winBottom + winHeight;

                Vector3 v0 = Vector3.Lerp(bvs[0], bvs[1], winBottom);
                Vector3 v1 = Vector3.Lerp(bvs[0], bvs[1], winTop);
                Vector3 v2 = Vector3.Lerp(bvs[2], bvs[3], winBottom);
                Vector3 v3 = Vector3.Lerp(bvs[2], bvs[3], winTop);

                quads[x, y].SetMesh(v0, v1, v2, v3);
                quads[x, y].setMaterialPreserveAlpha(matBuilding.GetMaterial());
            }
        }
    }

    public void StartFades(bool on)
    {
        foreach (Quad quad in quads)
        {
            quad.StartFade(Random.Range(0f, 2f), on);
        }
    }

    public void InitializeAllAlpha(float initAlpha)
    {
        foreach (Quad quad in quads)
        {
            Color c = quad.GetComponent<MeshRenderer>().material.color;
            c.a = initAlpha;
            quad.GetComponent<MeshRenderer>().material.color = c;
        }
    }
}