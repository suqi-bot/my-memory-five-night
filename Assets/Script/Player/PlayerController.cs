using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    public float moveSpeed;
    public float runSpeed;
    public float jumpSpeed;

    public float maxEndurance, nowEndurance;
    public float maxColdEndurance, nowColdEndurance;
    private float horizontalMove, verticalMove;
    private bool isSquat;
    private Vector3 dir;
    public float griority;
    private Vector3 velocity;
    public Transform playerCamera;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groundLayer;

    public bool isGround;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        nowEndurance = maxEndurance;
        nowColdEndurance = maxColdEndurance;
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        isGround = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
        playerCamera.GetComponent<PlayerMouse>().MouseMove();
        HandleMove();
        

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 画一个球体在物体位置
        Gizmos.DrawSphere(groundCheck.position, checkRadius);
    }

    private void HandleMove()
    {
        if (isGround && velocity.y < 0)
        {
            velocity.y = -2;
        }
        if (Input.GetButtonDown("Jump") && isGround && nowEndurance > 0 && !isSquat)
        {
            nowEndurance -= .5f;
            nowColdEndurance = maxColdEndurance;
            velocity.y = jumpSpeed;
        }
        if (Input.GetButton("Run") && isGround && nowEndurance > 0)
        {
            nowColdEndurance = maxColdEndurance;
            nowEndurance -= Time.deltaTime;
            UIManager.Instance.GetPanel<GameSencePanel>().ChangeEnduranceValue(maxEndurance, nowEndurance);
            horizontalMove = Input.GetAxis("Horizontal") * runSpeed * (isSquat ? 0.4f : 1);
            verticalMove = Input.GetAxis("Vertical") * runSpeed * (isSquat ? 0.4f : 1);
        }
        else
        {
            horizontalMove = Input.GetAxis("Horizontal") * moveSpeed * (isSquat? 0.4f : 1);
            verticalMove = Input.GetAxis("Vertical") * moveSpeed * (isSquat ? 0.4f : 1);
        }
   
        if(nowEndurance != maxEndurance)
        {
            nowColdEndurance -= Time.deltaTime;
            if (nowColdEndurance < 0)
            {
                nowEndurance += Time.deltaTime;
            }
            UIManager.Instance.GetPanel<GameSencePanel>().ChangeEnduranceValue(maxEndurance, nowEndurance);
            if (nowEndurance >= maxEndurance)
            {
                nowEndurance = maxEndurance;
            }
        }
        Vector3 currentRotation = playerCamera.localEulerAngles;
        if (Input.GetButton("LeftHand"))
        {
            
            //float lerp = Mathf.Lerp(0, 15f, Time.deltaTime);
            playerCamera.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 15f);

            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, Vector3.left * 0.5f + Vector3.up * 0.5f, Time.deltaTime);
        }
        if (Input.GetButtonUp("LeftHand"))
        {
            playerCamera.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
            playerCamera.localPosition = Vector3.zero + Vector3.up * 0.5f;

        }
        if (Input.GetButton("RightHand"))
        {

            //float lerp = Mathf.Lerp(0, 15f, Time.deltaTime);
            playerCamera.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, -15f);

            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, Vector3.right * 0.5f + Vector3.up * 0.5f, Time.deltaTime);
        }
        if (Input.GetButtonUp("RightHand"))
        {
            playerCamera.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
            playerCamera.localPosition = Vector3.zero + Vector3.up * 0.5f;

        }
        if (Input.GetButtonDown("Squat"))
        {
            isSquat = !isSquat;
            if (CheckHandRay())
            {
                isSquat = true;
            }
            if (isSquat)
                this.transform.localScale = new Vector3(1, .7f, 1);
            else
                this.transform.localScale = Vector3.one;
        }
        if (Input.GetButtonDown("Interaction"))
        {
            CheckFaceRay();
        }


        dir = transform.forward * verticalMove + transform.right * horizontalMove;
        cc.Move(dir * Time.deltaTime);
        velocity.y -= griority * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    public bool CheckHandRay()
    {
        Ray ray = new Ray(transform.position, Vector3.up * .5f);
        RaycastHit hitInfo;
        return Physics.Raycast(ray, out hitInfo);

    }

    public RaycastHit CheckFaceRay()
    {
        Ray ray = new Ray(transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log(hitInfo.collider);
            hitInfo.collider.GetComponentInParent<Door>()?.UseItem();
        }
        return hitInfo;
    }
}
