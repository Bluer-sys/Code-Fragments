using System;

namespace UI.Interfaces
{
    public interface IBasePopup
    {
        event Action<IBasePopup> OnDestroyed;
    }
}