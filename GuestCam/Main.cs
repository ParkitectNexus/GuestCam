using UnityEngine;

namespace GuestCam
{
    public class Main : IMod, IModSettings
    {
        private GameObject _gameObject;

        public GuestCamCamera Camera { get; private set; }
        public GuestCamSettings Settings { get; private set; }

        #region Implementation of IMod

        public void onEnabled()
        {
            // Check for double loading.
            if (_gameObject != null)
                return;

            // Create game object.
            _gameObject = new GameObject();
            Camera = _gameObject.AddComponent<GuestCamCamera>();

            Settings = _gameObject.AddComponent<GuestCamSettings>();
            Settings.Main = this;
            Settings.Load();
        }

        public void onDisabled()
        {
            Object.Destroy(_gameObject);
            _gameObject = null;
        }

        public string Name
        {
            get { return "GuestCam"; }
        }

        public string Description
        {
            get { return "Press O to start watching from a guest's perspective"; }
        }

        public string Identifier { get; set; }

        public string Path { get; set; }

        #endregion

        #region Implementation of IModSettings

        public void onDrawSettingsUI()
        {
            Settings.onDrawSettingsUI();
        }

        public void onSettingsOpened()
        {
            Settings.onSettingsOpened();
        }

        public void onSettingsClosed()
        {
            Settings.onSettingsClosed();
        }

        #endregion
    }
}