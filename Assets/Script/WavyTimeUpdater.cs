using UnityEngine;

public class WavyTimeUpdater : MonoBehaviour
{
    [SerializeField] Material wavyMaterial;

    void Update()
    {
        if (wavyMaterial == null) return;

        // timeScale ‚Ì‰e‹¿‚ðŽó‚¯‚È‚¢ŽžŠÔ
        wavyMaterial.SetFloat("_TimeOffset", Time.unscaledTime);
    }
}
