using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] Material mat;
    [SerializeField] float speed = 1;

    private Material[] materials;

    private void Awake()
    {
        materials = new Material[transform.childCount];

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = transform.GetChild(i).GetComponent<SpriteRenderer>().material = Instantiate(mat);
        }
    }

    private void Update()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetVector("_Offset", new Vector2(0, materials[i].GetVector("_Offset").y - ((speed * (i + 1)) * Time.deltaTime)));
        }
    }
}