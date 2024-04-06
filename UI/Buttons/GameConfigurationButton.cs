using Logic.Interfaces.Presenters;
using Zenject;

namespace UI.Buttons
{
	public class GameConfigurationButton : AbstractButton
	{
		private IGameConfigurationPopupPresenter _gameConfigurationPopupPresenter;

		[Inject]
		private void Construct(IGameConfigurationPopupPresenter gameConfigurationPopupPresenter)
		{
			_gameConfigurationPopupPresenter = gameConfigurationPopupPresenter;
		}
		
		protected override void PerformedAction() => _gameConfigurationPopupPresenter.Show();
	}
}