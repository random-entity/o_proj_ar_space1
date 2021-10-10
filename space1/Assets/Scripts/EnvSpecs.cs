using UnityEngine;

public class EnvSpecs : MonoSingleton<EnvSpecs>
{
    public static float landRadius = 50f;
    [SerializeField] private Transform groundCylinder;

    private void Start()
    {
        groundCylinder.localScale = new Vector3(2 * landRadius, 0.1f, 2 * landRadius);
    }
}