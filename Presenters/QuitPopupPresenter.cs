using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Logic.Interfaces;
using Logic.Interfaces.Presenters;
using UI.Popups;

namespace Logic.Services.Presenters
{
	[UsedImplicitly]
	public class QuitPopupPresenter : IQuitPopupPresenter
	{
		private const string QuitPopupKey = "QuitPopup";
		
		private readonly IPopupSystem _popupSystem;

		public QuitPopupPresenter(IPopupSystem popupSystem)
		{
			_popupSystem = popupSystem;
		}

		public void Show()
		{
			_popupSystem.ShowPopup<QuitPopup>( QuitPopupKey ).Forget();
		}
	}
}