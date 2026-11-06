using System;
using System.Linq;
using UI;
using UnityEngine;

namespace Core.PopupShower
{
    public class PopupsProvider : MonoBehaviour, IPopupsProvider
    {
        [SerializeField] private PopupData[] _popups = new PopupData[0];

        private BasicPopup _currentPopup;

        private void Start()
        {
            foreach (var popup in _popups)
            {
                popup.popup.OnShow += OnPopupOpened;
                popup.popup.OnClose += OnPopupClosed;
            }
        }

        private void OnPopupOpened(BasicPopup popup)
        {
            _currentPopup = popup;
        }

        private void OnPopupClosed(BasicPopup popup)
        {
            _currentPopup = null;
        }

        public bool TryOpen(PopupID popupID, bool force = false)
        {
            var popup = _popups.FirstOrDefault(x=>x.id == popupID);
            if(popup == null)
            {
                Debug.LogError($"Could not find data for {popupID}");
                return false;
            }
            
            if (_currentPopup != null)
            {
                if (force)
                {
                    _currentPopup.Close();
                }
                else
                {
                    Debug.LogWarning($"Close {_currentPopup.gameObject} to open {popupID}");
                    return false;
                }
            }
            
            BasicPopup newPopup = Instantiate(popup.popup, transform);
            newPopup.OnShow += OnPopupOpened;
            newPopup.OnClose += OnPopupClosed;
            newPopup.Open();
            return true;
        }
        
        public BasicPopup GetCurrentPopup() => _currentPopup;

        [Serializable]
        private class PopupData
        {
            public PopupID id;
            public BasicPopup popup;
        }
    }
}