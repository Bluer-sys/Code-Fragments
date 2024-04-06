using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Logic.Interfaces;
using Logic.Interfaces.Presenters;
using UI.Popups;

namespace Logic.Services.Presenters
{
	[UsedImplicitly]
	public class SettingsPopupPresenter : ISettingsPopupPresenter
	{
		private const string SettingsPopupKey = "SettingsPopup";
		
		private readonly IPopupSystem _popupSystem;

		public SettingsPopupPresenter(IPopupSystem popupSystem)
		{
			_popupSystem = popupSystem;
		}

		public void Show()
		{
			_popupSystem.ShowPopup<SettingsPopup>( SettingsPopupKey ).Forget();
		}
	}
}