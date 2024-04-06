using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Screens
{
	public class MatchFinishScreen : BaseScreen
	{
		[SerializeField] private Image _avatar;
		[SerializeField] private TextMeshProUGUI _playerName;
		[SerializeField] private TextMeshProUGUI _figure;
		[SerializeField] private TextMeshProUGUI _score;
		[SerializeField] private TextMeshProUGUI _bestScore;
		[SerializeField] private TextMeshProUGUI _turnsLeft;
		[SerializeField] private TextMeshProUGUI _timeSpent;

		private void OnValidate()
		{
			_avatar.CheckNotNull();
			_playerName.CheckNotNull();
			_figure.CheckNotNull();
			_score.CheckNotNull();
			_bestScore.CheckNotNull();
			_turnsLeft.CheckNotNull();
			_timeSpent.CheckNotNull();
		}

		public void SetWinner(PlayerData playerData)
		{
			_avatar.sprite = playerData.Avatar;
			_playerName.text = playerData.Name;
			_figure.text = playerData.IsX ? "X" : "O";
		}

		public void SetTurnsLeft(int value)
		{
			_turnsLeft.text = $"Turns Left {value}";
		}
		
		public void SetTimeSpent(float seconds)
		{
			_timeSpent.text = $"Time Spent {seconds / 60:F0} : {seconds % 60:F0}";
		}

		public void SetScore(int value)
		{
			_score.text = $"Total Score {value}";
		}
		
		public void SetBestScore(int value)
		{
			_bestScore.text = $"Best Score {value}";
		}
	}
}