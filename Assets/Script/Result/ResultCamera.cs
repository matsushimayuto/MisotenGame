using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCamera : MonoBehaviour
{
    Transform target;

    Vector3 startPos;
    Quaternion startRot;

    [SerializeField] float moveTime = 1.0f;
    Vector3 offset = new Vector3(0, 0f, -5f);

    List<GameObject> hiddenObjects = new List<GameObject>();

    bool isResultMode = false;

    void Update()
    {
        if (!isResultMode) return;
        if (target == null) return;

        CheckObstacles();
    }

    public void SetTarget(Transform player)
    {
        target = player;
    }

    public void MoveToPlayer()
    {
        if (target == null)
        {
            Debug.LogWarning("ResultCamera : target が未設定");
            return;
        }

        startPos = transform.position;
        startRot = transform.rotation;

        StopAllCoroutines();
        StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {
        Vector3 endPos = target.position + offset;
        Quaternion lookRot = Quaternion.LookRotation(target.position - endPos);
        Quaternion tiltRot = Quaternion.Euler(-20f, 0f, 0f);
        Quaternion endRot = lookRot * tiltRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / moveTime;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }
    }

    void CheckObstacles()
    {
        // 前フレームで消したものを戻す
        foreach (var obj in hiddenObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        hiddenObjects.Clear();

        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude;

        Ray ray = new Ray(transform.position, dir.normalized);
        RaycastHit[] hits = Physics.RaycastAll(ray, dist);

        foreach (var hit in hits)
        {
            // Player自身 or 子は無視
            if (hit.transform == target || hit.transform.IsChildOf(target))
                continue;

            // Blockなどを非表示
            GameObject hitObj = hit.transform.gameObject;
            hitObj.SetActive(false);
            hiddenObjects.Add(hitObj);
        }
    }

    public void ClearHiddenObjects()
    {
        foreach (var obj in hiddenObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        hiddenObjects.Clear();
    }

    public void BeginResult()
    {
        isResultMode = true;
    }

    public void EndResult()
    {
        isResultMode = false;
        ClearHiddenObjects(); // 念のため全部戻す
    }
}

