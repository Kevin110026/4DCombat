using UnityEngine;

[ExecuteAlways] // 讓腳本在 Edit 模式下也能即時預覽
public class SyncCameraProperties : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private Camera uiCamera;

    private void Awake()
    {
        uiCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        // 同步 FOV 與視角屬性
        uiCamera.fieldOfView = mainCamera.fieldOfView;
        uiCamera.nearClipPlane = mainCamera.nearClipPlane;
        uiCamera.farClipPlane = mainCamera.farClipPlane;

        // 若包含正交/透視切換亦可一併同步
        uiCamera.orthographic = mainCamera.orthographic;
        uiCamera.orthographicSize = mainCamera.orthographicSize;

        // 同步位置與旋轉
        transform.position = mainCamera.transform.position;
        transform.rotation = mainCamera.transform.rotation;
    }
}