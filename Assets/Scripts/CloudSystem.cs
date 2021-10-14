using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CloudSystem : MonoBehaviour
{
    #region fields
    [SerializeField] private CloudPivot cloudPivotPrefab;
    [SerializeField] private int numberOfClouds;
    private CloudPivot[] cloudPivots;
    private Material material;
    public static float minRotationSpeed = 0.25f, maxRotationSpeed = 4f;
    #endregion
    
    public static float getRandomSpeed()
    {
        return Random.Range(minRotationSpeed, maxRotationSpeed) * (Random.Range(0f, 1f) < 0.5f ? 1f : -1f);
    }

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material; // material을 instancing 하면서 동시에 각가의 CloudPivot 들에게 그 하나의 CloudSystem 인스턴스 내에서만 공유되는 material 인스턴스를 공유시키려고.
        Extensions.InitializeArray<CloudPivot>(out cloudPivots, cloudPivotPrefab, numberOfClouds, transform, (cp) => { cp.SetMaterial(material); });
    }

    private IEnumerator fade(bool on)
    {
        for (float fadeAlpha = material.color.a; 0 <= fadeAlpha && fadeAlpha <= 1; fadeAlpha += on ? 0.02f : -0.02f)
        {
            SetAlpha(fadeAlpha);
            yield return new WaitForSeconds(0.04f);
        }
        SetAlpha(on ? 1f : 0f);
    }
    public void StartFade(bool on)
    {
        StopAllCoroutines();
        StartCoroutine(fade(on));
    }
    public void SetAlpha(float alpha)
    {
        Color color = material.GetColor("_Color");
        color.a = alpha;
        material.SetColor("_Color", color);
    }
    public void SetRenderQueue(int renderQueue)
    {
        material.renderQueue = renderQueue;
    }
}