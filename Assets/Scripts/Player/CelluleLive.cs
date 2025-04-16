using UnityEngine;

public class CelluleLive : MonoBehaviour
{

    public float life;
    [Header("Range of life")]
    public float max;
    public float min;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(life, life, life);
    }
}
