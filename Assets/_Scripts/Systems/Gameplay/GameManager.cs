using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int leftForNextGroup = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            leftForNextGroup = 0;
        }

        if (leftForNextGroup == 0)
        {
            RoundsController.Instance.StartGroup();
        }
    }

    public void EndLevel()
    {
        Debug.LogWarning("Acabo el nivel");
        leftForNextGroup = -1;
    }
}