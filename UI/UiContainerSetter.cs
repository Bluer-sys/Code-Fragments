using UI.Interfaces;
using UnityEngine;
using Utilities;
using Zenject;

namespace UI
{
	public class UiContainerSetter : MonoBehaviour
	{
		[SerializeField] private Transform _screenContainer;
		[SerializeField] private Transform _popupContainer;
		[SerializeField] private GameObject _popupBackground;
		
		private IUiContainer _uiContainer;

		[Inject]
		private void Construct(IUiContainer uiContainer)
		{
			_uiContainer = uiContainer;
		}

		private void OnValidate()
		{
			_screenContainer.CheckNotNull();
			_popupContainer.CheckNotNull();
			_popupBackground.CheckNotNull();
		}

		private void Awake()
		{
			_uiContainer.Set( _screenContainer, _popupContainer, _popupBackground );
		}
	}
}