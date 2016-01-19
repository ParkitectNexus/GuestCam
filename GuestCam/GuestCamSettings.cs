using System;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

namespace GuestCam
{
    public class GuestCamSettings : MonoBehaviour, IModSettings
    {
        public GuestCamSettings()
        {
            // Set defaults
            CameraDistance = -0.13f;
            CameraHeight = -0.09f;
        }

        public float CameraDistance { get; set; }
        public float CameraHeight { get; set; }

        public Main Main { get; set; }

        public void Save()
        {
            AssertMainIsSet();

            var dict = new Dictionary<string, float>();
            dict["CameraDistance"] = CameraDistance;
            dict["CameraHeight"] = CameraHeight;

            var str = Json.Serialize(dict);

            try
            {
                File.WriteAllText(GetSettingsPath(), str);
            }
            catch (Exception e)
            {
                Debug.LogError("Could not save GuestCam settings. " + e.Message);
            }
        }

        public void Load()
        {
            AssertMainIsSet();

            if (File.Exists(GetSettingsPath()))
            {
                try
                {
                    var str = File.ReadAllText(GetSettingsPath());
                    var dict = Json.Deserialize(str) as IDictionary<string, object>;
                    
                    TryGet<float>(dict, "CameraDistance", v => CameraDistance = v);
                    TryGet<float>(dict, "CameraHeight", v => CameraHeight = v);
                }
                catch (Exception e)
                {
                    Debug.LogError("Could not save GuestCam settings. " + e.Message);
                }
            }
        }

        private void AssertMainIsSet()
        {
            if (Main == null)
                throw new Exception("Main is not set");
        }

        private static void TryGet<T>(IDictionary<string, object> dict, string key, Action<T> store)
        {
            object obj;
            if (dict != null && key != null && store != null && dict.TryGetValue(key, out obj))
            {
                if (obj is T)
                    store((T) obj);
                else if (obj == null)
                    store(default(T));
                else if (typeof (T) == typeof (float) && obj is double)
                    store((T) (object) (float) (double) obj);
            }
        }
        
        private string GetSettingsPath()
        {
            return System.IO.Path.Combine(Main.Path, "settings.json");
        }

        #region Implementation of IModSettings

        public void onDrawSettingsUI()
        {
            GUILayout.Label("Camera Distance: " + CameraDistance.ToString("0.00"));
            CameraDistance = GUILayout.HorizontalSlider(CameraDistance, -0.5f, 2.5f);

            GUILayout.Label("Camera Height: " + (-CameraHeight).ToString("0.00"));
            CameraHeight = -GUILayout.HorizontalSlider(-CameraHeight, -0.5f, 0.5f);
        }

        public void onSettingsOpened()
        {
        }

        public void onSettingsClosed()
        {
            Main.Camera.ApplySettings();
            Save();
        }

        #endregion
    }
}