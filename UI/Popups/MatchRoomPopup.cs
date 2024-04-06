using System.Threading;
using Cysharp.Threading.Tasks;
using Logic.Interfaces.Sync;
using Logic.Sync.Gameplay;
using SharedLibrary;
using SharedLibrary.Match.Room;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace UI.Popups
{
    public class MatchRoomPopup : BasePopup
    {
        [SerializeField] private Button _cancelMatchButton;
        [SerializeField] private TMP_Text _roomIdText;

        private readonly CancellationTokenSource _disposeTokenSource = new();

        private IRoomRegistrar _roomRegistrar;
        private MatchController _matchController;

        [Inject]
        private void Construct(IRoomRegistrar roomRegistrar, MatchController matchController)
        {
            _roomRegistrar = roomRegistrar;
            _matchController = matchController;
        }

        public async UniTaskVoid Initialize(RoomData roomData)
        {
            _roomIdText.text = roomData.RoomId.ToString();

            while (_disposeTokenSource.IsCancellationRequested == false
                   && await _roomRegistrar.IsSecondPlayerConnected() == false)
            {
                const int waitDuration = 1;
                await UniTask.WaitForSeconds(waitDuration);
            }

            _matchController.StartMatch(roomData.ConvertToMatchData());
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _cancelMatchButton.CheckNotNull();
        }

        protected override void Awake()
        {
            OnDestroyed += _ => CancelMatch();

            _cancelMatchButton
                .OnClickAsObservable()
                .First()
                .Subscribe(_ => Close())
                .AddTo(this);

            base.Awake();
        }

        private void CancelMatch()
        {
            _disposeTokenSource?.Dispose();
            _roomRegistrar.Unregister();
        }
    }
}