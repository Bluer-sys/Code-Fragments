using Cysharp.Threading.Tasks;
using Data.Configs;
using JetBrains.Annotations;
using Logic.Interfaces.Sync;
using SharedLibrary.Match.Board;
using SharedLibrary.Match.Turn;
using UnityEngine;

namespace Logic.Sync.Gameplay
{
	[UsedImplicitly]
	public class TurnsRegistrar : ITurnsRegistrar
	{
		private readonly IServerAccessService _serverAccessService;
		private readonly ServerConfig _serverConfig;

		public TurnsRegistrar(
			IServerAccessService serverAccessService,
			ServerConfig serverConfig)
		{
			_serverAccessService = serverAccessService;
			_serverConfig = serverConfig;
		}

		public async UniTask RegisterTurn(ulong? matchId)
		{
			await _serverAccessService.Put( $"{_serverConfig.TurnUrl}/{matchId}" );
		}

		public async UniTask RegisterFigurePlaced(ulong? matchId, Vector2Int position, CellContent cellContent)
		{
			TurnData playerTurn = new(new CellPosition(position.x, position.y), cellContent);
			await _serverAccessService.Post<TurnData, bool>(
				$"{_serverConfig.TurnUrl}/{matchId}", playerTurn);
		}
	}
}