using Unity.VisualScripting;
using UnityEngine;

public class SelectCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;   // 追従ターゲット
    [SerializeField] private Renderer targetRender;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 2f, -5f);  // ターゲット相対位置
    [SerializeField] private float positionSmooth = 6f;    // 位置補間速度
    [SerializeField] private float rotationSmooth   = 6f;    // 角度補間速度

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool inside = IsVisibleFrom(mainCamera, targetRender);

        if (inside == true)
        {
            // 位置を補間して追従
            Vector3 desiredPos = target.position + targetOffset;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPos,
                positionSmooth * Time.deltaTime
            );
        }




        //// 角度を補間して回転
        //Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
        //transform.rotation = Quaternion.Slerp(
        //    transform.rotation,
        //    targetRot,
        //    rotationSmooth * Time.deltaTime
        //    );


    }

    // 画面内 → true、画面外 → false
    public static bool IsVisibleFrom(Camera cam, Renderer renderer)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Bounds bounds = renderer.bounds;

        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
}



