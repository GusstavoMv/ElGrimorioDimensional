using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Muryotaisu
{
    public class MuryotaisuController : MonoBehaviour
    {
        private Animator animator;

        public float speed = 2; // Walking speed
        public float runSpeed = 5; // Running speed
        public KeyCode runKey = KeyCode.LeftShift; // Tecla para correr
        public bool hasRunAnimation = true; // Desactívalo si no tienes el parámetro "runFlag" en el Animator

        public float jumpSpeed = 2; // Jump speed
        public float gravity = 1; // gravity

        public float rotationSpeed = 720f; // Grados por segundo con los que el personaje gira hacia donde camina

        public float startKocchi = 2; // Distance to camera for Kocchiflag firing <●><●>

        public bool isFirstPerson; // Lo controla FirstPersonToggle

        [Header("Sonido")]
        public AudioClip sonidoSalto;
        public float volumenSalto = 1f;
        private AudioSource audioSource;

        float second; // Time Measurement

        private CharacterController controller;
        private Vector3 moveDirection = Vector3.zero;
        private Transform cam;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            cam = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            // Por si la cámara se creó/cambió después del Start
            if (cam == null && Camera.main != null) cam = Camera.main.transform;

            // Smile
            animator.SetBool("smileFlag", Input.GetKey("q"));

            // Kocchiminna <●><●>
            float dist = Vector3.Distance(transform.position, cam.position);
            animator.SetBool("kocchiFlag", dist < startKocchi);

            // ---------- INPUT (relativo a la cámara) ----------
            float h = Input.GetAxisRaw("Horizontal"); // A/D y flechas
            float v = Input.GetAxisRaw("Vertical");   // W/S y flechas
            Vector3 input = new Vector3(h, 0f, v);
            bool hasInput = input.sqrMagnitude > 0.01f;
            input.Normalize();

            // ---------- CORRER ----------
            bool isRunning = hasInput && Input.GetKey(runKey);
            float currentSpeed = isRunning ? runSpeed : speed;

            // Direcciones de la cámara aplanadas en el plano horizontal
            Vector3 camForward = cam.forward;
            camForward.y = 0f;
            camForward.Normalize();
            Vector3 camRight = cam.right;
            camRight.y = 0f;
            camRight.Normalize();

            Vector3 desiredDir = camForward * input.z + camRight * input.x;

            // ---------- ROTACIÓN + MOVIMIENTO HORIZONTAL ----------
            Vector3 horizontalMove = Vector3.zero;
            if (hasInput && desiredDir.sqrMagnitude > 0.0001f)
            {
                // En primera persona el cuerpo gira con el mouse, no hacia donde camina
                if (!isFirstPerson)
                {
                    Quaternion targetRot = Quaternion.LookRotation(desiredDir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }

                horizontalMove = desiredDir.normalized * currentSpeed;
            }

            // ---------- SUELO / ANIMACIONES / SALTO ----------
            if (controller.isGrounded)
            {
                // Evita que la gravedad se acumule mientras está en el piso
                if (moveDirection.y < 0f) moveDirection.y = -1f;

                // Switching idle motions
                second += Time.deltaTime;

                if (Input.GetKeyDown("space"))
                {
                    animator.SetBool("jumpFlag", true);
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("idleFlag", false);
                    moveDirection.y = jumpSpeed;

                    // Sonido de salto
                    if (audioSource != null && sonidoSalto != null)
                        audioSource.PlayOneShot(sonidoSalto, volumenSalto);
                }
                else if (hasInput)
                {
                    animator.SetBool("jumpFlag", false);
                    animator.SetBool("walkFlag", true);
                    animator.SetBool("idleFlag", false);
                    second = 0;
                }
                else if (second >= 15)
                {
                    animator.SetBool("jumpFlag", false);
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("idleFlag", false);
                    animator.SetTrigger("idleBFlag");
                    second = 0;
                }
                else
                {
                    animator.SetBool("jumpFlag", false);
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("idleFlag", true);
                }
            }

            // Animación de correr (solo en el suelo)
            if (hasRunAnimation)
                animator.SetBool("runFlag", isRunning && controller.isGrounded);

            // ---------- GRAVEDAD + MOVER ----------
            moveDirection.y -= gravity * Time.deltaTime;

            Vector3 finalMove = horizontalMove;
            finalMove.y = moveDirection.y;
            controller.Move(finalMove * Time.deltaTime);
        }
    }

}