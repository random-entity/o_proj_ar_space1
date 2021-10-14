using UnityEngine;

public class CloudPivot : MonoBehaviour
{
    [SerializeField] Transform cloud;
    private float rotationSpeed;
    private void Awake()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        float r = EnvSpecs.landRadius;
        float arg = Random.Range(0f, 2f);
        float argDegree = arg * 180f;
        arg *= Mathf.PI;

        cloud.localPosition = new Vector3(r * Mathf.Cos(arg), Random.Range(20f, 80f), r * Mathf.Sin(arg));
        cloud.localRotation = Quaternion.Euler(0f, 90f - argDegree, 90f /*이 z값은 캡슐이 세로로 서 있어서*/);
        cloud.localScale *= Random.Range(1f, 4f);

        rotationSpeed = CloudSystem.getRandomSpeed();
    }
    public void SetMaterial(Material material)
    {
        cloud.GetComponent<MeshRenderer>().material = material;
    }
    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }
}