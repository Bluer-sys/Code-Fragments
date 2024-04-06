using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Logic.Interfaces;
using Logic.Interfaces.Presenters;
using SharedLibrary.Match.Room;
using UI.Popups;

namespace Logic.Services.Presenters
{
    [UsedImplicitly]
    public class MatchRoomPopupPresenter : IMatchRoomPopupPresenter
    {
        private const string MatchRoomPopupKey = "MatchRoomPopup";

        private readonly IPopupSystem _popupSystem;

        public MatchRoomPopupPresenter(IPopupSystem popupSystem)
        {
            _popupSystem = popupSystem;
        }

        public async UniTask Show(RoomData roomData)
        {
            var popup = await _popupSystem.ShowPopup<MatchRoomPopup>(MatchRoomPopupKey);
            popup.Initialize(roomData).Forget();
        }
    }
}