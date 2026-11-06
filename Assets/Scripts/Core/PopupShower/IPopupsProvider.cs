using Core.ServiceLocator;
using UI;

namespace Core.PopupShower
{
    public interface IPopupsProvider : IService
    {
        bool TryOpen(PopupID popupID, bool force = false);
        BasicPopup GetCurrentPopup();
    }
}