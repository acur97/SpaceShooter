using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private float speed = 1;
    [SerializeField] private float xOffset = 9;

    private int materialsCount;
    private Material[] materials;

    private void Awake()
    {
        materialsCount = transform.childCount;
        materials = new Material[materialsCount];

        for (int i = 0; i < materialsCount; i++)
        {
            transform.GetChild(i).transform.localPosition = new Vector2(Random.Range(-xOffset, xOffset), 0);

            materials[i] = transform.GetChild(i).GetComponent<SpriteRenderer>().material = Instantiate(mat);
            materials[i].SetFloat(MaterialProperties.Speed, speed * (i + 1));
        }
    }
}