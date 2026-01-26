using UnityEngine;

public class SelectCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;   // 追従ターゲット
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 2f, -5f);  // ターゲット相対位置
    [SerializeField] private float positionSmooth = 6f;    // 位置補間速度
    //[SerializeField] private float rotationSmooth = 6f;    // 角度補間速度

    [Tooltip("ステージ遷移時のカメラ設定")]
    [SerializeField] private float distance = 3f;
    [SerializeField] private float height   = 2f;

    private bool IsEnterCamera = false; // ステージに移行する処理が行われたかどうか
    private bool IsSnapping = false;    // Playerの背後に回り切ったか

    void Start()
    {

    }

    void LateUpdate()
    {
        if (target == null) return;

        if (IsEnterCamera == false)
        {
            // 位置を補間して追従
            Vector3 desiredPos = new Vector3(-1.5f, target.position.y + targetOffset.y, target.position.z + targetOffset.z);

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPos,
                positionSmooth * Time.deltaTime
            );

            // 角度を補間して回転
            //Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
            //transform.rotation = Quaternion.Slerp(
            //    transform.rotation,
            //    targetRot,
            //    rotationSmooth * Time.deltaTime
            //);
        }
        else if (IsSnapping == true)
        {
            // カメラの補間速度を下げる
            positionSmooth = 2f;

            Vector3 behind =
            target.position
            - target.forward * distance
            + Vector3.up * height;

            // Playerの背後に回るように位置を補間して追従
            Vector3 desiredPos = behind;

            transform.position = Vector3.Lerp(
                 transform.position,
                 desiredPos,
                 positionSmooth * Time.deltaTime
            );

            transform.LookAt(target.position + Vector3.up * height * 0.8f);

            // 十分近づいたら終了
            if (Vector3.SqrMagnitude(transform.position - desiredPos) < 0.01f)
            {
                IsSnapping = false;
            }

        }

    }

    // ステージ移動するときのカメラの移動処理
    public void EnterCamera()
    {
        IsEnterCamera = true;
        IsSnapping = true;
    }




}



