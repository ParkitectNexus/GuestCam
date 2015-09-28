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
            else if(_isInGuest && (Input.GetKeyUp(KeyCode.O) || Input.GetKeyUp(KeyCode.Escape)))
            {
                LeaveGuest();
            }
        }

        private void OnDestroy()
        {
            LeaveGuest();
        }

        private Guest GuestUnderMouse()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            return Physics.Raycast(ray, out hit, Mathf.Infinity)
                ? hit.transform.gameObject.GetComponentInParent<Guest>()
                : null;
        }

        private void EnterGuest(Guest guest)
        {
            if (_isInGuest)
                return;

            var message = string.Format("You are now following {0} {1}.", guest.forename, guest.surname);
            NotificationBar.Instance.addNotification(message).openInfoWindowOf = guest;
            
            _guestCam = new GameObject();
            _guestCam.AddComponent<Camera>().nearClipPlane = 0.0005f;
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
            
            Destroy(_guestCam);
            
            _isInGuest = false;
        }
    }
}