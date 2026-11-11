using UnityEngine;
using Unity.Cinemachine;

namespace Muryotaisu
{
    public class FirstPersonToggle : MonoBehaviour
    {
        [Header("Referencias")]
        public MuryotaisuController controller;   // Tu controlador del personaje
        public CinemachineCamera fpCamera;        // Cámara de primera persona (hija del personaje, a la altura de los ojos)

        [Header("Ajustes")]
        public KeyCode toggleKey = KeyCode.V;
        public float sensitivity = 2f;
        public float minPitch = -80f;
        public float maxPitch = 80f;
        public bool hideBodyInFirstPerson = true;

        private bool isFirstPerson;
        private float pitch;
        private Renderer[] renderers;

        void Start()
        {
            if (controller == null) controller = GetComponent<MuryotaisuController>();
            renderers = GetComponentsInChildren<Renderer>();
            SetFirstPerson(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
                SetFirstPerson(!isFirstPerson);

            if (!isFirstPerson) return;

            // Girar el cuerpo con el mouse (izquierda/derecha)
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            transform.Rotate(0f, mouseX, 0f);

            // Mirar arriba/abajo solo con la cámara
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
            fpCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void SetFirstPerson(bool value)
        {
            isFirstPerson = value;
            controller.isFirstPerson = value;
            fpCamera.gameObject.SetActive(value);
            pitch = 0f;

            if (hideBodyInFirstPerson)
            {
                // Oculta el cuerpo pero conserva su sombra
                var mode = value ? UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly
                                 : UnityEngine.Rendering.ShadowCastingMode.On;
                foreach (var r in renderers)
                    r.shadowCastingMode = mode;
            }
        }
    }
}