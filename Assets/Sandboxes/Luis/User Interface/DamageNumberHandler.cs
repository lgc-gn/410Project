using TMPro;
using UnityEngine;

public class DamageNumberHandler : MonoBehaviour
{
    public float destroyTime = 1.5f;
    public float floatSpeed = .1f;
    public Vector3 offset;
    public Vector3 randomize = new Vector3(2,0,0);

    private TextMeshPro textMesh;
    private Color originalColor;
    private float timeElapsed;

    void Start()
    {
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
        transform.localPosition += new Vector3(
            Random.Range(-randomize.x, randomize.x),
            Random.Range(0, randomize.y),
            Random.Range(-randomize.z, randomize.z)
        );

        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            originalColor = textMesh.color;
        }
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timeElapsed / destroyTime);

        if (textMesh != null)
        {
            Color fadedColor = originalColor;
            fadedColor.a = alpha;
            textMesh.color = fadedColor;
        }
    }

    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 direction = transform.position - cam.transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }
    }

}
