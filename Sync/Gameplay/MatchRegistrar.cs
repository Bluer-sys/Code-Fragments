using System;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using Data.Configs;
using JetBrains.Annotations;
using Logic.Interfaces.Sync;
using SharedLibrary.Match;
using SharedLibrary.Match.Turn;
using SharedLibrary.Player;
using UnityEngine;

namespace Logic.Sync.Gameplay
{
	[UsedImplicitly]
	public class MatchRegistrar : IMatchRegistrar
	{
		private readonly IServerAccessService _serverAccessService;
		private readonly ServerConfig _serverConfig;

		public MatchRegistrar(
			IServerAccessService serverAccessService,
			ServerConfig serverConfig)
		{
			_serverAccessService = serverAccessService;
			_serverConfig = serverConfig;
		}

		public async UniTask<TurnStateData> Register(MatchData sharedMatch)
		{
			return
				await _serverAccessService.Post<MatchData, TurnStateData>( _serverConfig.MatchUrl, sharedMatch );
		}

		public async UniTask<TurnStateData> Connect(ulong matchId)
		{
			string payloadUrl = $"{_serverConfig.MatchUrl}/{matchId}";

			MatchData matchData = await GetMatchConnectedTo();

			while (matchData == null || matchData.CurrentTurnPlayer == null)
			{
				const float waitDelay = 0.1f;
				await UniTask.WaitForSeconds(waitDelay);
				matchData = await GetMatchConnectedTo();
			}

			PlayerData firstPlayer = matchData.CurrentTurnPlayer;

			return new TurnStateData(firstPlayer.Id, firstPlayer.Figure);

			async UniTask<MatchData> GetMatchConnectedTo()
			{
				try
				{
					return await _serverAccessService.Get<MatchData>(payloadUrl);
				}
				catch (HttpRequestException)
				{
					Debug.Log("Not connected yet");
					return null;
				}
			}
		}

		public async UniTask Unregister(ulong? matchId, ulong playerId)
		{
			string payloadUrl = $"{_serverConfig.MatchUrl}/{matchId}/{playerId}";

			await _serverAccessService.Delete( payloadUrl );
		}
	}
}