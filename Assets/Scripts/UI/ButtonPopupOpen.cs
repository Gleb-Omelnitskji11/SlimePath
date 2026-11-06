using Core.PopupShower;
using Core.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonPopupOpen : MonoBehaviour
    {
        [SerializeField] PopupID _popupID;
        private Button _button;
        private IPopupsProvider _popupsProvider;

        private void Start()
        {
            _button = GetComponent<Button>();
            _popupsProvider = Services.Get<IPopupsProvider>();
            _button.onClick.AddListener(OpenPopup);
        }

        private void OpenPopup()
        {
            _popupsProvider.TryOpen(_popupID);
        }
    }
}