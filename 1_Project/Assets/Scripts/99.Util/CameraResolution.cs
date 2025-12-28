using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [Header("Settings")]
    // 우리가 기준으로 잡은 해상도 비율 (1080 : 1920) -> 9 : 16
    public float targetAspect = 9.0f / 16.0f;

    // 기준이 되는 몬스터 영역의 넉넉한 가로 사이즈 (World Unit)
    // 예: 몬스터가 뚱뚱해서 가로 5 Unit을 차지하면, 여백 포함 6 정도로 설정
    public float targetWidthSize = 6.0f;

    void Awake()
    {
        AdjustCamera();
    }

    void Update()
    {
        // 에디터에서 해상도 바꿔가며 테스트할 땐 Update에 두세요.
        // 실제 빌드 시엔 Awake나 Start에만 둬도 됩니다.
#if UNITY_EDITOR
        AdjustCamera();
#endif
    }

    void AdjustCamera()
    {
        Camera cam = GetComponent<Camera>();

        // 현재 기기의 화면 비율 (가로 / 세로)
        float currentAspect = (float)Screen.width / (float)Screen.height;

        // [핵심 로직]
        // 우리가 원하는 가로 너비(targetWidthSize)를 
        // 현재 기기의 비율로 나눠서 세로 사이즈(orthographicSize)를 역산합니다.

        // 공식: Size = (목표 가로길이 / 현재 비율) / 2
        cam.orthographicSize = targetWidthSize / currentAspect / 2f;
    }
}