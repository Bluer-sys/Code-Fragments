using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Logic.Interfaces.Sync;
using Logic.Signals.GameState;
using Logic.Signals.TurnState;
using SharedLibrary.Match;
using UnityEngine.SceneManagement;
using Zenject;
#if SKILLZ_ENABLED
using SkillzSDK;
using Utilities;
using UnityEngine;
#endif

namespace Logic.Sync.Gameplay
{
    [UsedImplicitly]
    public class MatchController : IDisposable, IMatchSyncData
#if SKILLZ_ENABLED
		,SkillzMatchDelegate
#endif
    {
	    public ulong? Id { get; private set; }
	    public bool IsBot { get; private set; }
	    public bool IsFigurePlacing { get; private set; }

	    private readonly UserData _userData;
	    private readonly SignalBus _signalBus;
	    private readonly IMatchRegistrar _matchRegistrar;
	    private readonly ITurnsRegistrar _turnsRegistrar;

        public MatchController(
            UserData userData,
            SignalBus signalBus,
            IMatchRegistrar matchRegistrar,
            ITurnsRegistrar turnsRegistrar)
        {
            _userData = userData;
            _signalBus = signalBus;

            _matchRegistrar = matchRegistrar;
            _turnsRegistrar = turnsRegistrar;
        }

#if SKILLZ_ENABLED
		public void OnMatchWillBegin(Match matchInfo)
		{
			MatchData matchData = matchInfo.ConvertToShared();

			StartMatch( matchData );
		}

		public void OnProgressionRoomEnter()
		{
		}

		public void OnSkillzWillExit()
		{
			// Do nothing. Loading the initial scene again causes problems with concurrency as the SDK handles relaunching the activity for us
		}
#endif

#if SKILLZ_ENABLED
	    private async void StartMatch(MatchData matchInfo)
#else
	    public async void StartMatch(MatchData matchInfo)
#endif
        {
            Id = matchInfo.Id;
			IsBot = matchInfo.IsBot;
            
#if SKILLZ_ENABLED
			if ( SkillzCrossPlatform.GetMatchInfo().IsCustomSynchronousMatch )
			{
				Debug.Log( "Sync Game Mode Starting..." );
				SetSyncMatchSettings( SkillzCrossPlatform.GetMatchInfo() );
				Debug.Log( "SetSyncMatchSettings complete..." );

				_userData.IsGameOver = false;
				Debug.Log( "Loading SyncScene synchronously..." );
			}
#endif

            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

            if (matchInfo.IsBot)
            {
	            _signalBus.Fire(new MatchWillBeginSignal(await _matchRegistrar.Register(matchInfo)));
            }
            else if (matchInfo.Player2 != null)
            {
	            _signalBus.Fire(new MatchWillBeginSignal(await _matchRegistrar.Register(matchInfo)));
            }
            else
            {
	            _signalBus.Fire(new MatchWillBeginSignal(await _matchRegistrar.Connect(matchInfo.Id)));
            }

            _signalBus.Subscribe<TurnEndSignal>(OnTurnEnd);
            _signalBus.Subscribe<FigurePlacedSignal>(OnFigurePlaced);
            _signalBus.Subscribe<MatchWillEndSignal>(OnMatchWillEnd);
        }

	    private async void OnMatchWillEnd(MatchWillEndSignal signal)
        {
	        if (Id != null)
	        {
		        await UniTask.WaitWhile(() => IsFigurePlacing);
		        await _matchRegistrar.Unregister(Id, _userData.CurrentPlayerId);
	        }

            _signalBus.TryUnsubscribe<TurnEndSignal>(OnTurnEnd);
            _signalBus.TryUnsubscribe<FigurePlacedSignal>(OnFigurePlaced);
            _signalBus.TryUnsubscribe<MatchWillEndSignal>(OnMatchWillEnd);

            ClearSyncSettings();
            ClearMatchID();

            if (signal.IsAborted)
#if SKILLZ_ENABLED
				SkillzCrossPlatform.AbortMatch();
#else
                SceneManager.LoadScene("StartMenuScene", LoadSceneMode.Single);
#endif
        }

        private async void OnTurnEnd(TurnEndSignal signal)
        {
            if (signal.FromServer)
                return;

            await _turnsRegistrar.RegisterTurn(Id);
        }

        private async void OnFigurePlaced(FigurePlacedSignal signal)
        {
            if (signal.FromServer)
                return;

            IsFigurePlacing = true;
            await _turnsRegistrar.RegisterFigurePlaced(Id, signal.Position, signal.CellContent);
            IsFigurePlacing = false;
        }

#if SKILLZ_ENABLED
		private void SetSyncMatchSettings(Match matchInfo)
		{
			CustomServerConnectionInfo info = matchInfo.CustomServerConnectionInfo;
			_userData.SyncUrl = info.ServerIp;
			_userData.SyncPort = (uint)int.Parse(info.ServerPort);
			_userData.SyncMatchToken = info.MatchToken;
			_userData.SyncMatchId = info.MatchId;
			_userData.IsGameOver = false;

			foreach (Player player in matchInfo.Players)
			{
				if ( player.IsCurrentPlayer )
				{
					_userData.CurrentPlayerId = (long)player.ID!;
					_userData.CurrentPlayerAvatarUrl = player.AvatarURL;
					_userData.CurrentPlayerName = player.DisplayName;
				}
				else
				{
					_userData.OpponentAvatarUrl = player.AvatarURL;
					_userData.OpponentName = player.DisplayName;
				}
			}
		}
#endif

	    private void ClearMatchID()
        {
            Id = null;
        }

        private void ClearSyncSettings()
        {
            _userData.SyncUrl = null;
            _userData.SyncPort = 0;
            _userData.SyncMatchToken = null;
            _userData.SyncMatchId = null;
            _userData.CurrentPlayerId = 0;
        }

        public void Dispose()
        {
            OnMatchWillEnd(new MatchWillEndSignal(true));
        }
    }
}