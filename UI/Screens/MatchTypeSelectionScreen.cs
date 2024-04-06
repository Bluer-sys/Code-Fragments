using System.Net.Http;
using Cysharp.Threading.Tasks;
using Data.Configs;
using Logic.Interfaces.Presenters;
using Logic.Interfaces.Sync;
using Logic.Sync.Gameplay;
using SharedLibrary.Configurations;
using SharedLibrary.Match.Room;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;
#if !SKILLZ_ENABLED
using SharedLibrary.Match;
using SharedLibrary.Player;
#endif

namespace UI.Screens
{
    public class MatchTypeSelectionScreen : BaseScreen
    {
        [SerializeField] private Button _botMatchButton;

        [SerializeField] private TMP_InputField _enterMatchInputField;
        [SerializeField] private Button _enterMatchButton;

        [SerializeField] private Button _createMatchButton;

        private MatchController _matchController;
        private IRoomRegistrar _roomRegistrar;
        private GameConfig _gameConfig;
        private IMatchRoomPopupPresenter _matchRoomPopupPresenter;
        private UserData _userData;

        [Inject]
        private void Construct(
            MatchController matchController,
            IRoomRegistrar roomRegistrar,
            GameConfig gameConfig,
            IMatchRoomPopupPresenter matchRoomPopupPresenter,
            UserData userData)
        {
            _matchController = matchController;
            _roomRegistrar = roomRegistrar;
            _gameConfig = gameConfig;
            _matchRoomPopupPresenter = matchRoomPopupPresenter;
            _userData = userData;
        }

        private void OnValidate()
        {
            _botMatchButton.CheckNotNull();
            _enterMatchInputField.CheckNotNull();
            _enterMatchButton.CheckNotNull();
            _createMatchButton.CheckNotNull();
        }

        private void Awake()
        {
            _botMatchButton
                .OnClickAsObservable()
                .Subscribe(_ => PlayBotMatch().Forget())
                .AddTo(this);

            _enterMatchButton
                .OnClickAsObservable()
                .Subscribe(_ => EnterMatch().Forget())
                .AddTo(this);

            _createMatchButton
                .OnClickAsObservable()
                .Subscribe(_ => CreateMatch().Forget())
                .AddTo(this);
        }

        private async UniTaskVoid PlayBotMatch()
        {
#if SKILLZ_ENABLED
			SkillzCrossPlatform.LaunchSkillz( _matchDelegate );
#else
            CreateRoomData createRoomData = new(new PlayerData { Nickname = "Creator" }, _gameConfig.ConvertToConfiguration());

            RoomData roomData = await _roomRegistrar.Register(createRoomData);

            _userData.CurrentPlayerId = roomData.Creator.Id;

            PlayerData currentPlayer = new PlayerData { Id = _userData.CurrentPlayerId, Nickname = "CurrentPlayer" };

            MatchData matchData = new MatchData(currentPlayer, _gameConfig.ConvertToConfiguration())
            {
                Id = roomData.MatchId ?? 0
            };

            _matchController.StartMatch(matchData);
#endif
        }

        private async UniTaskVoid CreateMatch()
        {
            CreateRoomData createRoomData =
                new(new PlayerData { Nickname = "Creator" }, _gameConfig.ConvertToConfiguration());
            RoomData roomData = await _roomRegistrar.Register(createRoomData);

            _userData.CurrentPlayerId = roomData.Creator.Id;
            _matchRoomPopupPresenter.Show(roomData).Forget();
        }

        private async UniTaskVoid EnterMatch()
        {
            ulong roomId;

            try
            {
                roomId = ulong.Parse(_enterMatchInputField.text);
            }
            catch
            {
                return;
            }

            try
            {
                RoomData roomData =
                    await _roomRegistrar.Connect(roomId, new PlayerData { Nickname = "ConnectedPlayer" });

                _userData.CurrentPlayerId = roomData.Connected!.Id;
                SetGameConfigField(roomData.Configuration);
                _matchController.StartMatch(roomData.ConvertToMatchData());
            }
            catch (HttpRequestException e)
            {
                Debug.LogError(e);
            }
            return;

            void SetGameConfigField(Configuration from)
            {
                _gameConfig.FieldSize = from.FieldSize;
                _gameConfig.IsInfiniteField = from.IsInfiniteField;
                _gameConfig.WinningNumberOfElementInSequence = from.WinningNumberOfElementInSequence;
                _gameConfig.TurnTime = from.TurnTime;
            }
        }
    }
}