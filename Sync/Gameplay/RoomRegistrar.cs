using Cysharp.Threading.Tasks;
using Data.Configs;
using JetBrains.Annotations;
using Logic.Interfaces.Sync;
using SharedLibrary.Match.Room;
using SharedLibrary.Player;

namespace Logic.Sync.Gameplay
{
    [UsedImplicitly]
    public class RoomRegistrar : IRoomRegistrar
    {
        private readonly IServerAccessService _serverAccessService;
        private readonly ServerConfig _serverConfig;

        private ulong? _roomId;

        public RoomRegistrar(
            IServerAccessService serverAccessService,
            ServerConfig serverConfig,
            GameConfig gameConfig)
        {
            _serverAccessService = serverAccessService;
            _serverConfig = serverConfig;
        }

        public async UniTask<RoomData> Register(CreateRoomData createRoomData)
        {
            var roomData = await _serverAccessService.Post<CreateRoomData, RoomData>(_serverConfig.RoomUrl, createRoomData);
            _roomId = roomData.RoomId;

            return roomData;
        }

        public async UniTask<RoomData> Connect(ulong roomId, PlayerData connectedPlayer)
        {
            return
                await _serverAccessService.Post<PlayerData, RoomData>(
                    $"{_serverConfig.RoomUrl}/{roomId}", connectedPlayer);
        }

        public async UniTask Unregister()
        {
            string payloadUrl = $"{_serverConfig.RoomUrl}/{_roomId}";

            await _serverAccessService.Delete(payloadUrl);
        }

        public async UniTask<bool> IsSecondPlayerConnected()
        {
            return await _serverAccessService.Get<bool>($"{_serverConfig.RoomUrl}/{_roomId}");
        }
    }
}