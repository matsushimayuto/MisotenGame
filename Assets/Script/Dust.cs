using UnityEngine;

public class Dust : MonoBehaviour
{
    [SerializeField, Tooltip("‰Œ‚ÌƒvƒŒƒnƒu")] public GameObject dustPrefab;
    private GameObject dustEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dustEffect = Instantiate(dustPrefab, gameObject.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        dustEffect.transform.position = gameObject.transform.position; 
    }
}
