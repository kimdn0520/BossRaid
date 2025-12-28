using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);
    private Vector2 lastScreenSize = new Vector2(0, 0);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    void Update()
    {
        // 매 프레임 체크하지만, 실제로 값이 변했을 때만 적용하므로 성능 부하 없음
        Refresh();
    }

    void Refresh()
    {
        Rect safeArea = Screen.safeArea;

        // 화면 크기나 Safe Area가 변하지 않았다면 리턴 (최적화)
        if (safeArea == lastSafeArea && new Vector2(Screen.width, Screen.height) == lastScreenSize)
            return;

        // 변경 사항 저장
        lastSafeArea = safeArea;
        lastScreenSize.x = Screen.width;
        lastScreenSize.y = Screen.height;

        ApplySafeArea(safeArea);
    }

    void ApplySafeArea(Rect r)
    {
        // Safe Area 좌표를 0~1 사이의 앵커 값으로 정규화(Normalize)
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // RectTransform에 적용
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        // 여백 제거 (앵커를 바꿨으므로 오프셋은 0으로 초기화해야 딱 맞음)
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}