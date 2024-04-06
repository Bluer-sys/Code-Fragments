using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Logic.Interfaces.Game;
using Logic.Interfaces.Sync;
using Logic.Signals.GameState;
using Logic.Signals.Input;
using Logic.Signals.TurnState;
using SharedLibrary.Match.Turn;
using Utilities;
using Zenject;

namespace Logic.Sync.Gameplay
{
    [UsedImplicitly]
    public class TurnsSynchronizer : IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly IServer _server;
        private readonly ITurnState _turnState;
        private readonly IMatchSyncData _matchSyncData;

        private readonly CancellationTokenSource _disposeTokenSource = new();

        public TurnsSynchronizer(
            SignalBus signalBus,
            IServer server,
            ITurnState turnState,
            IMatchSyncData matchSyncData)
        {
            _signalBus = signalBus;
            _server = server;
            _turnState = turnState;
            _matchSyncData = matchSyncData;

            _signalBus.Subscribe<MatchWillBeginSignal>(OnMatchStart);
            _signalBus.Subscribe<MatchWillEndSignal>(OnMatchEnd);
        }

        private void OnMatchStart(MatchWillBeginSignal signal)
        {
            _turnState.InitializeMovesOrder(signal.TurnStateData);
            ServerRequestAsync().Forget();
        }

        private void OnMatchEnd()
        {
            _disposeTokenSource?.Cancel();
        }

        private async UniTaskVoid ServerRequestAsync()
        {
            while (
#if SKILLZ_ENABLED
				SkillzCrossPlatform.IsMatchInProgress() &&
#endif
                _disposeTokenSource.IsCancellationRequested == false)
            {
                if (_turnState.IsCurrentPlayerTurn)
                {
                    _signalBus.Fire(new TurnStartSignal());

                    while (_turnState.IsCurrentPlayerTurn || _matchSyncData.IsFigurePlacing)
                    {
                        const float waitDuration = 0.33f;
                        await UniTask.WaitForSeconds(waitDuration);
                    }
                }
                else
                {
                    _signalBus.Fire(new TurnStartSignal());
                    TurnData turnData = await _server.TryRequestForTurn();

                    while (turnData.IsExist == false &&
                           _disposeTokenSource.IsCancellationRequested == false &&
                           _turnState.IsCurrentPlayerTurn == false)
                    {
                        const float waitDuration = 0.5f;
                        await UniTask.WaitForSeconds(waitDuration);

                        if (_disposeTokenSource.IsCancellationRequested == false)
                        {
                            turnData = await _server.TryRequestForTurn();
                        }
                    }

                    if (_disposeTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    _signalBus.Fire(new ServerTapSignal(turnData.Position.ToVector2Int()));
                    await UniTask.WaitWhile(() => _turnState.IsCurrentPlayerTurn == false);
                }
            }
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<MatchWillEndSignal>(OnMatchEnd);
            _signalBus.Unsubscribe<MatchWillBeginSignal>(OnMatchStart);

            _disposeTokenSource?.Dispose();
        }
    }
}