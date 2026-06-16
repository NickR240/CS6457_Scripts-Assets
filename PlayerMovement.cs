using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

  [SerializeField] private float playerMoveSpeed = 1f;
  [SerializeField] private float sprintMultiplier = 2f;
  [SerializeField] private float playerRotationSpeed = 150f;
  public Transform cameraTransform;

  private PlayerInputReader playerInput;
  private Rigidbody rbody; 

  void Awake()
  {
      playerInput = GetComponent<PlayerInputReader>();
      rbody = GetComponent<Rigidbody>();
      Time.timeScale = 1f;
    }

  void FixedUpdate()
  {
      HandleRotation();
      HandleMovement();
  }

  private void HandleRotation()
  {
      float yRotation = playerInput.Turn.x * playerRotationSpeed * Time.fixedDeltaTime;

      if (Mathf.Abs(yRotation) > 0.001f)
      {
        Quaternion deltaRotation = Quaternion.Euler(0f, yRotation, 0f);
        rbody.MoveRotation(rbody.rotation * deltaRotation);
      }
      else if (cameraTransform != null)
      {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude > 0.001f)
        {
          Quaternion targetRotation = Quaternion.LookRotation(forward.normalized);
          Quaternion yOnly = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
          rbody.MoveRotation(Quaternion.Slerp(rbody.rotation, yOnly, 5f * Time.fixedDeltaTime));
        }
      }
  }

  private void HandleMovement()
  {
    Vector2 moveInput = playerInput.Move;

    Vector3 moveDirection = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
    moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

    float speed = playerMoveSpeed;
    if (playerInput.SprintHeld)
    {
      speed *= sprintMultiplier;
    }
    rbody.MovePosition(rbody.position + moveDirection * speed * Time.fixedDeltaTime);
  }
}
