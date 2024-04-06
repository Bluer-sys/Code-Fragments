using System;
using DG.Tweening;
using UI.Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Popups
{
	public abstract class BasePopup : MonoBehaviour, IBasePopup
	{
		private const float ScaleDuration = 0.2f;

		[SerializeField] private Button _closeButton;

		private Tween _closeTween;

		public event Action<IBasePopup> OnDestroyed;

		protected virtual void OnValidate()
		{
			_closeButton.CheckNotNull();
		}

		protected virtual void Awake()
		{
			_closeButton.OnClickAsObservable()
				.First()
				.Subscribe( _ => Close() )
				.AddTo( this );

			transform
				.DOScale( Vector3.one, ScaleDuration )
				.From( Vector3.zero );
		}

		private void OnDestroy()
		{
			OnDestroyed?.Invoke(this);
		}

		protected void Close()
		{
			if ( _closeTween != null )
				return;

			_closeTween = transform
				.DOScale( Vector3.zero, ScaleDuration )
				.OnComplete( () => Destroy( gameObject ) );
		}
	}
}