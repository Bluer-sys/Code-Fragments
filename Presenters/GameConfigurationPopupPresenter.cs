using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Logic.Interfaces;
using Logic.Interfaces.Presenters;
using UI.Popups;

namespace Logic.Services.Presenters
{
	[UsedImplicitly]
	public class GameConfigurationPopupPresenter : IGameConfigurationPopupPresenter
	{
		private const string GameConfigurationPopup = "GameConfigurationPopup";
		
		private readonly IPopupSystem _popupSystem;

		public GameConfigurationPopupPresenter(IPopupSystem popupSystem)
		{
			_popupSystem = popupSystem;
		}

		public void Show()
		{
			_popupSystem.ShowPopup<GameConfigurationPopup>( GameConfigurationPopup ).Forget();
		}
	}
}