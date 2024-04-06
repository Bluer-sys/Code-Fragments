using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Logic.Interfaces;
using Logic.Interfaces.Presenters;
using UI.Popups;

namespace Logic.Services.Presenters
{
	[UsedImplicitly]
	public class HowToPlayPopupPresenter : IHowToPlayPopupPresenter
	{
		private const string HowToPlayPopupKey = "HowToPlayPopup";
		
		private readonly IPopupSystem _popupSystem;

		public HowToPlayPopupPresenter(IPopupSystem popupSystem)
		{
			_popupSystem = popupSystem;
		}

		public void Show()
		{
			_popupSystem.ShowPopup<HowToPlayPopup>( HowToPlayPopupKey ).Forget();
		}
	}
}