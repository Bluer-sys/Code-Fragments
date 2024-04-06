using Logic.Interfaces.Presenters;
using Zenject;

namespace UI.Buttons
{
	public class BackToMenuButton : AbstractButton
	{
		private IQuitPopupPresenter _quitPopupPresenter;

		[Inject]
		private void Construct(IQuitPopupPresenter quitPopupPresenter)
		{
			_quitPopupPresenter = quitPopupPresenter;
		}
		
		protected override void PerformedAction() => _quitPopupPresenter.Show();
	}
}