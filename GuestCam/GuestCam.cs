using Parkitect.UI;
using UnityEngine;

namespace GuestCam
{
    public class GuestCam : MonoBehaviour
    {
        private GameObject _guestCam; 
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

            var message = string.Format("You are now following {0} {1}.", guest.forename, guest.surname);
            NotificationBar.Instance.addNotification(message).openInfoWindowOf = guest;
            UIWorldOverlayController.Instance.gameObject.SetActive(false);
            Camera.main.GetComponent < CameraController>().enabled = false;
            _guestCam = new GameObject();
            _guestCam.AddComponent<Camera>().nearClipPlane = 0.05f;
            _guestCam.AddComponent<AudioListener>();
            _guestCam.transform.parent = guest.head.transform;
            _guestCam.transform.localPosition = new Vector3(-0.09f, -0.13f, 0);
            _guestCam.transform.localRotation = Quaternion.Euler(90, 0, 90);
            
            _isInGuest = true;
        }

        private void LeaveGuest()
        {
            if (!_isInGuest)
                return;

            UIWorldOverlayController.Instance.gameObject.SetActive(true);
            Camera.main.GetComponent<CameraController>().enabled = true;

            Destroy(_guestCam);
            
            _isInGuest = false;
        }
    }
}