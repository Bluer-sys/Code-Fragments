#if !SKILLZ_ENABLED
using UnityEngine.SceneManagement;
#endif

namespace UI.Buttons
{
	public class SubmitScoreButton : AbstractButton
	{
		protected override void PerformedAction()
		{
#if SKILLZ_ENABLED
			// TODO: Submit Score
			int score = 0;

			SkillzCrossPlatform.SubmitScore( score, null, null );
			SkillzCrossPlatform.DisplayTournamentResultsWithScore( score );
#else
			SceneManager.LoadScene( "StartMenuScene" );
#endif
		}
	}
}