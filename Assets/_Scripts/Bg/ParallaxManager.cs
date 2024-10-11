using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private float speed = 1;

    private int materialsCount;
    private Material[] materials;

    private readonly int _Offset = Shader.PropertyToID("_Offset");

    private void Awake()
    {
        materialsCount = transform.childCount;
        materials = new Material[materialsCount];

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = transform.GetChild(i).GetComponent<SpriteRenderer>().material = Instantiate(mat);
        }
    }

    private void Update()
    {
        for (int i = 0; i < materialsCount; i++)
        {
            materials[i].SetVector(_Offset, new Vector2(0, materials[i].GetVector(_Offset).y - (speed * (i + 1) * Time.deltaTime)));
        }
    }
}