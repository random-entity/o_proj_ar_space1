using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Quad : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private MeshRenderer mr;

    public void SetMesh(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        mr = GetComponent<MeshRenderer>();

        mesh.Clear();

        vertices = new Vector3[4] { v0, v1, v2, v3 };
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        mesh.RecalculateNormals();
    }

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        SetMesh(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
    }

    private IEnumerator Fade(float wait, bool on)
    {
        yield return new WaitForSeconds(wait);

        for (float fadeAlpha = on ? 0f : 1f; 0 <= fadeAlpha && fadeAlpha <= 1; fadeAlpha += on ? 0.04f : -0.02f)
        {
            Color c = mr.material.color;
            c.a = fadeAlpha;
            mr.material.color = c;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void StartFade(float wait, bool on)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(wait, on));
    }
}