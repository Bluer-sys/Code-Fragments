using JetBrains.Annotations;
using Logic.Interfaces;
using Logic.Interfaces.Presenters;
using UI.Screens;

namespace Logic.Services.Presenters
{
    [UsedImplicitly]
    public class MatchTypeSelectionScreenPresenter : IMatchTypeSelectionScreenPresenter
    {
        private const string MatchTypeSelectionScreenKey = "MatchTypeSelectionScreen";

        private readonly IScreenManager _screenManager;

        public MatchTypeSelectionScreenPresenter(IScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        public async void Show()
        {
            await _screenManager.ShowScreen<MatchTypeSelectionScreen>(MatchTypeSelectionScreenKey);
        }
    }
}