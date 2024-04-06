using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data.Configs;
using JetBrains.Annotations;
using Logic.Interfaces.Game;
using Logic.Interfaces.Sync;
using Logic.Sync.Gameplay;
using SharedLibrary.Match;
using SharedLibrary.Match.Board;
using SharedLibrary.Match.Turn;
using UniRx;

namespace Logic.Sync.ServerType
{
    [UsedImplicitly]
    public class Server : IServer, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly IServerAccessService _serverAccessService;
        private readonly ServerConfig _serverConfig;
        private readonly IMatchSyncData _matchSyncData;
        private readonly UserData _userData;

        public Server(
            IServerAccessService serverAccessService,
            ServerConfig serverConfig,
            IMatchSyncData matchSyncData,
            UserData userData)
        {
            _serverAccessService = serverAccessService;
            _serverConfig = serverConfig;
            _matchSyncData = matchSyncData;

            _userData = userData;
        }

        public async UniTask<TurnData> TryRequestForTurn()
        {
            var figurePosition = await _serverAccessService.Get<TurnData>(
                _serverConfig.TurnUrl + "/" + _matchSyncData.Id + "/" + _userData.CurrentPlayerId);

            return figurePosition;
        }
        
        public async UniTask<MatchData> TryRequestForMatchData()
        {
            MatchData matchData = await _serverAccessService.Get<MatchData>(
                _serverConfig.MatchUrl + "/" + _matchSyncData.Id);
            
            return matchData;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}