using System.Collections;
using UnityEngine;

public class LandPath : MonoBehaviour
{
    [SerializeField] private Material material;
    int curr = 0;

    private void Awake()
    {
        transform.localScale = Vector3.one * 2f * EnvSpecs.landRadius;

        material = GetComponent<MeshRenderer>().material;

        setAlpha(2, 0f);
    }

    public void SwapColors()
    {
        Debug.Log(material.GetColor("_Color0").a);
        Debug.Log(material.GetColor("_Color1").a);
        Debug.Log(material.GetColor("_Color2").a);
        Color temp = material.GetColor("_Color0");
        material.SetColor("_Color0", material.GetColor("_Color2"));
        material.SetColor("_Color2", material.GetColor("_Color1"));
        material.SetColor("_Color1", temp);
        Debug.Log(material.GetColor("_Color0").a);
        Debug.Log(material.GetColor("_Color1").a);
        Debug.Log(material.GetColor("_Color2").a);

    }

    public void SetAngWid(float angWidB2S, float angWidD2B)
    {
        material.SetFloat("_AngWidB2S", angWidB2S);
        material.SetFloat("_AngWidD2B", angWidD2B);
    }

    private void setAlpha(int index, float alpha)
    {
        Color c = material.GetColor("_Color" + index);
        c.a = alpha;
        material.SetColor("_Color" + index, c);
    }
    private IEnumerator fade(bool on)
    {
        for (float alpha = material.GetColor("_Color0").a; 0 < alpha && alpha < 1; alpha += on ? 0.02f : -0.02f)
        {
            setAlpha(0, alpha);
            yield return new WaitForSeconds(0.01f);
        }
        setAlpha(0, on ? 1f : 0f);
    }
    public void StartFade(bool on)
    {
        StopAllCoroutines();
        StartCoroutine(fade(on));
    }
}