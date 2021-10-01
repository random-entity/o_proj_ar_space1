using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTTest : MonoBehaviour
{
    MeshRenderer rnd;

    void Start()
    {
        rnd = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            StartCoroutine("Fade");
        }
    }

    IEnumerator Fade()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.01f)
        {
            Color c = rnd.material.color;
            c.a = ft;
            rnd.material.color = c;
            yield return new WaitForSeconds(0.01f);
        }
    }

}
