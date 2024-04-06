using JetBrains.Annotations;
using Logic.Interfaces;
using Logic.Interfaces.Game;
using Logic.Interfaces.Presenters;
using SharedLibrary.Match.Board;
using UI.Screens;
using UnityEngine;
using PlayerData = Data.PlayerData;

namespace Logic.Services.Presenters
{
	[UsedImplicitly]
	public class MatchFinishScreenPresenter : IMatchFinishScreenPresenter
	{
		private const string MatchFinishScreenKey = "MatchFinishScreen";

		private readonly IScreenManager _screenManager;
		private readonly ITurnState _turnState;
		private readonly IGameTimer _gameTimer;

		public MatchFinishScreenPresenter(IScreenManager screenManager, ITurnState turnState, IGameTimer gameTimer)
		{
			_screenManager = screenManager;
			_turnState = turnState;
			_gameTimer = gameTimer;
		}

		public async void Show()
		{
			MatchFinishScreen matchFinishScreen = await _screenManager.ShowScreen<MatchFinishScreen>( MatchFinishScreenKey );

			int score = CalculateScore();

			matchFinishScreen.SetWinner( GetWinner() );
			matchFinishScreen.SetTurnsLeft( _turnState.TurnNumber.Value );
			matchFinishScreen.SetTimeSpent( _gameTimer.GameTimeInSeconds );
			matchFinishScreen.SetScore( score );
			matchFinishScreen.SetBestScore( score );
		}

		private PlayerData GetWinner()
		{
			return _turnState.CurrentFigure == CellContent.X
				? new PlayerData( null, "Player 1", true )
				: new PlayerData( null, "Player 2", false );
		}

		private int CalculateScore()
		{
			return Mathf.RoundToInt( _turnState.TurnNumber.Value * 100 / _gameTimer.GameTimeInSeconds );
		}
	}
}