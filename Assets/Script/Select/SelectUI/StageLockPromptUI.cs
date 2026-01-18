using UnityEngine;

public class StageLockPromptUI : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0, 2f, 0);

    Transform target;

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        transform.forward = Camera.main.transform.forward;
    }

    public void Show(Transform player)
    {
        target = player;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }

}
