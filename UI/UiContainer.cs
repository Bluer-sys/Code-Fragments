using UI.Interfaces;
using UnityEngine;

namespace UI
{
	public class UiContainer : IUiContainer
	{
		public Transform ScreenContainer { get; private set; }
		public Transform PopupContainer { get; private set; }
		public GameObject PopupBackground { get; private set; }

		public void Set(Transform screenContainer, Transform popupContainer, GameObject popupBackground)
		{
			ScreenContainer = screenContainer;
			PopupContainer = popupContainer;
			PopupBackground = popupBackground;
		}
	}
}