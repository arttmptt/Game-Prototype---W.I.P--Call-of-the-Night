using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private bool Sprint => Input.GetKey(KeyCode.LeftShift);
	private bool ShouldJumpl => Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded;
	
	[Header("Move Parameters")]
	[SerializeField] private float walkSpeed = 8.0f;
	[SerializeField] private float sprintSpeed = 15.0f;
	[SerializeField] private float crouchSpeed = 3.0f;
	
	private float walkValue;
	private float sprintValue;
	
	[Header("Mouse Parameters")]
	[SerializeField, Range(1, 10)] private float lookSpeed = 2.0f;
	
	[Header("Jumping Parameters")]
	[SerializeField] private float jumpHeight = 5.0f;
 
    [Header("Audio")]
	[SerializeField] private AudioClip[] concreteSurface;
	[SerializeField] private AudioClip[] metalSurface;
	[SerializeField] private AudioClip jumpSound;
	[SerializeField] private AudioClip crouchSound;
	
    [HideInInspector]
	public bool isCrouching, isJumping, isZooming;

	private CharacterController characterController;
	private AudioSource mainSound;
	private Vector3 moveDirection;
	private Vector2 currentInput;
	private float gravity = -9.81f;
	private float rotationX = 0;
    private Camera mainCamera; 
	private Vector3 cameraStartPos;
	private Vector3 cameraEndPos;
	private float cameraMaxPosY = -2f;
	private float lerpTime = 0.2f;
	private float currentLerpTime1;
	private float currentLerpTime2;
	
private void Start()
    {
       characterController = GetComponent<CharacterController>();
	   mainCamera = GetComponentInChildren<Camera>();
	   cameraEndPos = mainCamera.transform.localPosition + Vector3.up * cameraMaxPosY;
	   mainSound = GetComponent<AudioSource>();
	   Cursor.lockState = CursorLockMode.Locked;
	   Cursor.visible = false;
    }


private void Update()
    {	
			HandleMovementInput();
			HandleMouseLook();
			
			if(!isCrouching)
			{
				HandleJump();
			}
			
			ApplyFinalMovement();
            Crouch();
		
		if (Input.GetMouseButtonDown(1))
		{
			isZooming = true;
		}
		else if (Input.GetMouseButtonUp(1) )
		{
			isZooming = false;
		}
		
    }
	
	private void HandleMovementInput()
	{
		if(!isCrouching)
		{
		currentInput = new Vector2((Sprint ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (Sprint ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
		}
		else
		{
		currentInput = new Vector2((Sprint ? crouchSpeed : crouchSpeed) * Input.GetAxis("Vertical"), (Sprint ? crouchSpeed : crouchSpeed) * Input.GetAxis("Horizontal"));
		}
		if(isZooming)
		{
		currentInput = new Vector2((Sprint ? crouchSpeed*2 : crouchSpeed*2) * Input.GetAxis("Vertical"), (Sprint ? crouchSpeed*2 : crouchSpeed*2) * Input.GetAxis("Horizontal"));	
		}
		float moveDirectionY = moveDirection.y;
		moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
		moveDirection.y = moveDirectionY;
	}
	
	private void HandleMouseLook()
	{
		rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
		rotationX = Mathf.Clamp(rotationX, -80, 80);
		mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
		transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
	}
	
	private void HandleJump()
	{
		if (ShouldJumpl)
		{
			moveDirection.y = jumpHeight;
			isJumping = true;
			}
			else
			{
				isJumping = false;
			}
	}
	
	private void ApplyFinalMovement()
	{
	    if (!characterController.isGrounded)
			moveDirection.y += gravity * Time.deltaTime;
		characterController.Move(moveDirection * Time.deltaTime);
	}
	
private	void Crouch()
	{
	float Perc1 = currentLerpTime1/lerpTime;
    float Perc2 = currentLerpTime2/lerpTime;	
    if (Input.GetKeyDown(KeyCode.C))
	{
		if (isCrouching)
		{
			isCrouching = false;
		}
		else
		{
			isCrouching = true;
		}
	}
	if (isCrouching && mainCamera.transform.localPosition.y > -2f)
	{
		currentLerpTime2 = 0;
		currentLerpTime1 += Time.deltaTime;
		mainCamera.transform.localPosition = Vector3.Lerp(cameraStartPos, cameraEndPos, Perc1);
	}
	
	if (!isCrouching && mainCamera.transform.localPosition.y < 0f)
	
	{
		currentLerpTime1 = 0;
		currentLerpTime2 += Time.deltaTime;
		mainCamera.transform.localPosition = Vector3.Lerp(cameraEndPos,cameraStartPos, Perc2);
	}
   }
	
	public void Footsteps()
     {
		 RaycastHit hit = new RaycastHit();
		 string floortag;
		if(characterController.isGrounded)
		{
		if(Physics.Raycast(transform.position, Vector3.down,out hit ))
        {
		floortag = hit.collider.gameObject.tag;
		if (floortag == "Concrete")
		{
         mainSound.clip = concreteSurface[Random.Range(0, concreteSurface.Length)];
		 mainSound.Play ();
		}
		else if (floortag == "Metal")
		{
         mainSound.clip = metalSurface[Random.Range(0, metalSurface.Length)];
		 mainSound.Play ();
		}
		}
		}
     }
public void JumpAudio()
{
	mainSound.clip = jumpSound;
	mainSound.Play ();
	
}
public void CrouchAudio()
{
	mainSound.clip = crouchSound;
	mainSound.Play ();
	
}
}