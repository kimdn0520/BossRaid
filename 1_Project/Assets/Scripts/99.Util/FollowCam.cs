using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField]
    private Transform target; // 따라갈 대상 (플레이어)

    [SerializeField]
    private float smoothSpeed = 0.125f; // 카메라가 따라가는 속도 (값이 낮을수록 부드러움)

    [SerializeField]
    private Vector3 offset; // 대상으로부터 유지할 거리

    // 모든 Update 로직이 끝난 후에 호출됩니다.
    private void LateUpdate()
    {
        // 따라갈 대상이 없으면 아무것도 하지 않습니다.
        if (target == null) return;

        // 목표 위치 = 대상의 위치 + 오프셋
        Vector3 desiredPosition = target.position + offset;

        // Lerp(선형 보간)를 사용하여 현재 위치에서 목표 위치로 부드럽게 이동합니다.
        // Time.deltaTime을 곱해 프레임 속도에 관계없이 일정한 속도를 유지합니다.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 계산된 위치로 카메라를 이동시킵니다.
        transform.position = smoothedPosition;
    }
}