using Data.Configs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace UI.Popups
{
	public class GameConfigurationPopup : BasePopup
	{
		[SerializeField] private Toggle _infiniteField;
		[SerializeField] private TMP_InputField _fieldSize;
		[SerializeField] private TMP_InputField _winningNumberOfElementInSequence;
		[SerializeField] private TMP_InputField _turnTime;

		[SerializeField] private Button _applyButton;

		private GameConfig _gameConfig;

		[Inject]
		private void Construct(GameConfig gameConfig)
		{
			_gameConfig = gameConfig;
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			
			_infiniteField.CheckNotNull();
			_fieldSize.CheckNotNull();
			_winningNumberOfElementInSequence.CheckNotNull();
			_turnTime.CheckNotNull();
			_applyButton.CheckNotNull();
		}

		protected override void Awake()
		{
			base.Awake();

			_applyButton
				.OnClickAsObservable()
				.First()
				.Subscribe( _ => Apply() )
				.AddTo( this );
		}

		private void OnEnable()
		{
			_infiniteField.isOn = _gameConfig.IsInfiniteField;
			_fieldSize.text = _gameConfig.FieldSize.ToString();
			_winningNumberOfElementInSequence.text = _gameConfig.WinningNumberOfElementInSequence.ToString();
			_turnTime.text = _gameConfig.TurnTime.ToString();
		}

		private void Apply()
		{
			// TODO: Validate inputs
			try
			{
				_gameConfig.IsInfiniteField = _infiniteField.isOn;
				_gameConfig.FieldSize = int.Parse( _fieldSize.text );
				_gameConfig.WinningNumberOfElementInSequence = int.Parse( _winningNumberOfElementInSequence.text );
				_gameConfig.TurnTime = int.Parse( _turnTime.text );
			}
			catch
			{
				Debug.LogError( "Failed to apply game configuration" );
				throw;
			}
			
			Close();
		}
	}
}