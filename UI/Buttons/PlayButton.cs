using Logic.Interfaces.Presenters;
using Zenject;

namespace UI.Buttons
{
    public class PlayButton : AbstractButton
    {
        private IMatchTypeSelectionScreenPresenter _matchTypeSelectionScreenPresenter;

        [Inject]
        private void Construct(IMatchTypeSelectionScreenPresenter matchTypeSelectionScreenPresenter)
        {
            _matchTypeSelectionScreenPresenter = matchTypeSelectionScreenPresenter;
        }

        protected override void PerformedAction()
        {
// #if SKILLZ_ENABLED
//             SkillzCrossPlatform.LaunchSkillz( _matchController );
// #else
//             PlayerData currentPlayer = new PlayerData { Id = 0, Nickname = "CurrentPlayer" };
//             _matchDelegate.StartMatch(new MatchData { Player1 = currentPlayer });
// #endif
            _matchTypeSelectionScreenPresenter.Show();
        }
    }
}