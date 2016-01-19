using Parkitect.UI;
using UnityEngine;

namespace GuestCam
{
    public class GuestCamCamera : MonoBehaviour
    {
        private GameObject _camera; 
        private bool _isInGuest;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!_isInGuest && Input.GetKeyUp(KeyCode.O))
            {
                var guest = GuestUnderMouse();

                if (guest != null)
                {
                    EnterGuest(guest);
                }
            }
            else if (_isInGuest && (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.Escape)))
            {
                LeaveGuest();
            }
        }

        private void OnDestroy()
        {
            LeaveGuest();
        }

        private static Guest GuestUnderMouse()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;

            var obj = Collisions.Instance.checkSelectables(ray, out distance);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance, LayerMasks.MOUSECOLLIDERS))
                obj = hit.collider.gameObject;
            return obj != null ? obj.GetComponentInParent<Guest>() : null;
        }

        private void EnterGuest(Guest guest)
        {
            if (_isInGuest)
                return;
            
            UIWorldOverlayController.Instance.gameObject.SetActive(false);
            Camera.main.GetComponent<CameraController>().enabled = false;

            _camera = new GameObject();
            _camera.AddComponent<Camera>().nearClipPlane = 0.05f;
            _camera.AddComponent<AudioListener>();
            _camera.transform.parent = guest.head.transform;
            _camera.transform.localPosition = new Vector3(-0.09f, -0.13f, 0);
            _camera.transform.localRotation = Quaternion.Euler(90, 0, 90);
            
            _isInGuest = true;

            ApplySettings();
        }

        private void LeaveGuest()
        {
            if (!_isInGuest)
                return;

            UIWorldOverlayController.Instance.gameObject.SetActive(true);
            Camera.main.GetComponent<CameraController>().enabled = true;

            Destroy(_camera);
            
            _isInGuest = false;
        }

        public void ApplySettings()
        {
            if (!_isInGuest)
                return;

            var settings = GetComponent<GuestCamSettings>();

            _camera.transform.localPosition = new Vector3(settings.CameraHeight, settings.CameraDistance, _camera.transform.localPosition.z);
        }
    }
}