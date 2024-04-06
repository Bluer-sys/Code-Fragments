using Data.Profiles;
using Logic.Interfaces.Presenters;
using Logic.Signals;
using Logic.Signals.Setting;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace UI.Popups
{
	public class SettingsPopup : BasePopup
	{
		[SerializeField] private Toggle _musicToggle;
		[SerializeField] private Toggle _soundToggle;
		[SerializeField] private Button _howToPlayButton;
		
		private SignalBus _signalBus;
		private GameProfile _gameProfile;
		private IHowToPlayPopupPresenter _howToPlayPopupPresenter;

		private SettingsProfile SettingsProfile => _gameProfile.SettingsProfile;
		
		[Inject]
		private void Construct(SignalBus signalBus, GameProfile gameProfile, IHowToPlayPopupPresenter howToPlayPopupPresenter)
		{
			_signalBus = signalBus;
			_howToPlayPopupPresenter = howToPlayPopupPresenter;
			_gameProfile = gameProfile;
			
			LoadSettings();

			Observable
				.Merge(
					_musicToggle.OnValueChangedAsObservable().Where( v => SettingsProfile.MusicOn.Value != v ),
					_soundToggle.OnValueChangedAsObservable().Where( v => SettingsProfile.SoundsOn.Value != v ) )
				.Select( _ => new PopupSettingsChangeSignal( _musicToggle.isOn, _soundToggle.isOn ) )
				.Subscribe( signal => _signalBus.Fire( signal ) )
				.AddTo( this );

			_howToPlayButton
				.OnClickAsObservable()
				.Subscribe( _ => _howToPlayPopupPresenter.Show() )
				.AddTo( this );
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			
			_musicToggle.CheckNotNull();
			_soundToggle.CheckNotNull();
			_howToPlayButton.CheckNotNull();
		}

		private void LoadSettings()
		{
			_musicToggle.isOn = SettingsProfile.MusicOn.Value;
			_soundToggle.isOn = SettingsProfile.SoundsOn.Value;
		}
	}
}