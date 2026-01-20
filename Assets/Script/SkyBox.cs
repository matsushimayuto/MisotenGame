using UnityEngine;

[RequireComponent(typeof(Camera))]
public class URPSkyboxRotator : MonoBehaviour
{
    public Texture2D skyboxTexture;
    public float rotationSpeed = 10f;

    private Material skyboxMaterial;
    private float rotation;

    void Start()
    {
        Shader shader = Shader.Find("Skybox/Panoramic");
        if (shader == null)
        {
            Debug.LogError("Skybox/Panoramic が見つかりません");
            return;
        }

        skyboxMaterial = new Material(shader);
        skyboxMaterial.SetTexture("_MainTex", skyboxTexture);

        // Camera の Skybox コンポーネントを取得
        var skybox = GetComponent<Skybox>();
        if (skybox == null)
        {
            skybox = gameObject.AddComponent<Skybox>();
        }

        // ここが重要：Cameraに直接渡す
        skybox.material = skyboxMaterial;

        DynamicGI.UpdateEnvironment();
    }

    void Update()
    {
        if (skyboxMaterial == null) return;

        rotation += rotationSpeed * Time.unscaledDeltaTime;
        skyboxMaterial.SetFloat("_Rotation", rotation);
    }
}
