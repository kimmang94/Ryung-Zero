using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxDistance = 15f;     // 사슬 최대 사거리
    public LayerMask grappleLayer;     // 사슬이 붙을 수 있는 레이어 (Ground 등)

    [Header("Physics")]
    public float grappleSpeed = 20f;   // 당겨지는 속도

    private PlayerController pc;
    private Rigidbody2D rb;
    private LineRenderer lr;
    private DistanceJoint2D dj;
    private Vector2 targetPoint;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        dj = GetComponent<DistanceJoint2D>();

        dj.enabled = false; // 시작할 땐 조인트를 꺼둡니다.
        lr.enabled = false; // 시작할 땐 사슬을 그리지 않습니다.
    }

    void Update()
    {
        // 1. 마우스 왼쪽 클릭 시 발사
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        // 2. 마우스 버튼 떼면 해제
        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        // 사슬이 연결되어 있다면 실시간으로 그리기
        if (lr.enabled)
        {
            lr.SetPosition(0, transform.position); // 시작점: 캐릭터
            lr.SetPosition(1, targetPoint);       // 도착점: 고정된 벽
        }
    }

    void StartGrapple()
    {
        // 마우스 방향 계산
        Vector2 direction = (pc.mousePos - (Vector2)transform.position).normalized;

        // 레이캐스트(보이지 않는 광선)를 쏴서 벽이 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            targetPoint = hit.point;

            // 물리적인 끈(Joint) 활성화
            dj.connectedAnchor = targetPoint;
            dj.distance = Vector2.Distance(transform.position, targetPoint) * 0.8f; // 약간 당겨지도록 설정
            dj.enabled = true;

            // 시각적인 줄(Line) 활성화
            lr.enabled = true;
            lr.positionCount = 2;
        }
    }

    void StopGrapple()
    {
        dj.enabled = false;
        lr.enabled = false;
    }
}