using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private float speed = 1;
    [SerializeField] private float xOffset = 9;

    private int materialsCount;
    private Material[] materials;
    private Vector2 offset = Vector2.zero;

    private readonly int _Offset = Shader.PropertyToID("_Offset");

    private void Awake()
    {
        materialsCount = transform.childCount;
        materials = new Material[materialsCount];

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = transform.GetChild(i).GetComponent<SpriteRenderer>().material = Instantiate(mat);
            transform.GetChild(i).transform.localPosition = new Vector2(Random.Range(-xOffset, xOffset), 0);
        }
    }

    private void Update()
    {
        for (int i = 0; i < materialsCount; i++)
        {
            offset.y = materials[i].GetVector(_Offset).y - (speed * (i + 1) * Time.deltaTime);
            materials[i].SetVector(_Offset, offset);
        }
    }
}