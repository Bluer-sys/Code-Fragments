using UnityEngine;

namespace UI.Interfaces
{
	public interface IUiContainer
	{
		Transform ScreenContainer { get; }
		Transform PopupContainer { get; }
		GameObject PopupBackground { get; }
		
		void Set(Transform screenContainer, Transform popupContainer, GameObject popupBackground);
	}
}