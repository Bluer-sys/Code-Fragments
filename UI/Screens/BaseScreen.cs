using System;
using UI.Interfaces;
using UnityEngine;

namespace UI.Screens
{
	public class BaseScreen : MonoBehaviour, IBaseScreen
	{
		public event Action<IBaseScreen> OnDestroyed;

		private void OnDestroy()
		{
			OnDestroyed?.Invoke( this );
		}

		public void Show()
		{
			gameObject.SetActive( true );
		}

		public void Hide()
		{
			gameObject.SetActive( false );
		}
		
		public void Close()
		{
			Destroy( gameObject );
		}
	}
}