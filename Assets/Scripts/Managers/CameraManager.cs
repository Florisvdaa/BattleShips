using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineCamera cmCamera;

    [SerializeField] private Vector3 playerBoardCameraPos;
    [SerializeField] private Vector3 enemyBoardCameraPos;

    [SerializeField] private float transitionSpeed = 2f;

    private Vector3 targetPosition;
    private bool lookingAtEnemy = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        targetPosition = playerBoardCameraPos;
        cmCamera.transform.position = targetPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lookingAtEnemy = !lookingAtEnemy;
            targetPosition = lookingAtEnemy ? enemyBoardCameraPos : playerBoardCameraPos;
        }

        cmCamera.transform.position = Vector3.Lerp(cmCamera.transform.position, targetPosition, transitionSpeed * Time.deltaTime);
    }
}
