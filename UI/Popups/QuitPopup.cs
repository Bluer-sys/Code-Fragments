using Logic.Interfaces.Presenters;
using Logic.Signals;
using Logic.Signals.GameState;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace UI.Popups
{
	public class QuitPopup : BasePopup
	{
		[SerializeField] private Button _playButton;
		[SerializeField] private Button _quitButton;

		private IMatchFinishScreenPresenter _matchFinishScreenPresenter;
		private SignalBus _signalBus;

		[Inject]
		private void Construct(IMatchFinishScreenPresenter matchFinishScreenPresenter, SignalBus signalBus)
		{
			_matchFinishScreenPresenter = matchFinishScreenPresenter;
			_signalBus = signalBus;
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			
			_playButton.CheckNotNull();
			_quitButton.CheckNotNull();
		}

		protected override void Awake()
		{
			base.Awake();

			_playButton
				.OnClickAsObservable()
				.First()
				.Subscribe( _ => Close() )
				.AddTo( this );

			_quitButton
				.OnClickAsObservable()
				.First()
				.Subscribe( _ => Abort() )
				.AddTo( this );
		}

		private void Abort()
		{
			_signalBus.Fire( new MatchWillEndSignal(true) );
			Close();
		}
	}
}