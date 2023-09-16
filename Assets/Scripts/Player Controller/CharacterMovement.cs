using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


namespace CharacterControls{

    public class CharacterMovement : NetworkBehaviour{
        public float speed = 6.0F;
        public float jumpSpeed = 8.0F;
        public float gravity = 20.0F;
        public Camera playerCamera;

        private Vector3 moveDirection = Vector3.zero;

        CharacterController controller;
        [SerializeField] private Animator animator = null;

        public GameObject light;
        public bool open= false;
        void Start(){
            controller = GetComponent<CharacterController>();
            if(!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }
        }
        void Update()
        {
            if(!isLocalPlayer)
            {
                return;
            }

            if (controller.isGrounded){
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                if (Input.GetButton("Jump"))
                    moveDirection.y = jumpSpeed;
            }

            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);

                // Set animation parameters
            //bool isMoving = Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f;
            //animator.SetBool("Walking", isMoving);
            bool isAttacking = false;
            bool isMoving= false;
            bool isJumping= false;
            if (Input.GetKey(KeyCode.W))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            animator.SetBool("Walking", isMoving);
            if (Input.GetButtonDown("Fire1"))
            {
                isAttacking = true;
            }
            else
            {
                isAttacking = false;
            }
            animator.SetBool("Attack", isAttacking);
            if (Input.GetButton("Jump"))
            {
                isJumping = true;
            }
            else
            {
                isJumping = false;
            }
            animator.SetBool("Jump", isJumping);

            if(Input.GetKeyDown(KeyCode.H))
            {
                open = !open;
                light.SetActive(open);
            }
            
        }

        void FixedUpdate(){
            
        }
    }
}
