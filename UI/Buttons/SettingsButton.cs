using Logic.Interfaces.Presenters;
using Zenject;

namespace UI.Buttons
{
	public class SettingsButton : AbstractButton
	{
		private ISettingsPopupPresenter _settingPopupPresenter;

		[Inject]
		private void Construct(ISettingsPopupPresenter settingsPopupPresenter)
		{
			_settingPopupPresenter = settingsPopupPresenter;
		}
		
		protected override void PerformedAction() => _settingPopupPresenter.Show();
	}
}