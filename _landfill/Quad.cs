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
        // mesh.RecalculateBounds();
    }

    public void setMaterialPreserveAlpha(Material newMaterial) // Quad instance의 material 새로 지정할 때는 꼭 이걸 써주세요~~!!!!!!!!
    {
        var saveAlpha = mr.material.color.a;

        mr.material = newMaterial;

        Color c = mr.material.color;
        c.a = saveAlpha;
        mr.material.color = c;
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

        for (float fadeAlpha = mr.material.color.a; 0 <= fadeAlpha && fadeAlpha <= 1; fadeAlpha += on ? 0.04f : -0.02f)
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