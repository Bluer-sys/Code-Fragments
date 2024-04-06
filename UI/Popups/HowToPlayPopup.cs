using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Popups
{
	public class HowToPlayPopup : BasePopup
	{
		[SerializeField] private Button _nextButton;
		[SerializeField] private Button _backButton;
		[SerializeField] private GameObject[] _slides;
		
		private int _currentSlide;

		protected override void OnValidate()
		{
			base.OnValidate();
			
			_nextButton.CheckNotNull();
			_backButton.CheckNotNull();
			_slides.CheckNotNull();
		}

		protected override void Awake()
		{
			base.Awake();

			_nextButton
				.OnClickAsObservable()
				.Subscribe( _ => OnNext() )
				.AddTo( this );
			
			_backButton
				.OnClickAsObservable()
				.Subscribe( _ => OnBack() )
				.AddTo( this );
			
			SetInteractive();
		}

		private void OnNext()
		{
			_currentSlide++;
			
			SetInteractive();
		}

		private void OnBack()
		{
			_currentSlide--;
			
			SetInteractive();
		}

		private void SetInteractive()
		{
			for (int i = 0; i < _slides.Length; i++)
			{
				_slides[i].SetActive( i == _currentSlide );
			}
			
			_nextButton.interactable = _currentSlide < _slides.Length - 1;
			_backButton.interactable = _currentSlide > 0;
		}
	}
}