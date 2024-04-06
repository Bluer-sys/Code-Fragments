namespace Game.UI
{
	using System;
	using System.Collections.Generic;
	using DG.Tweening;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;


	public interface IUiArsenalPartView
	{
		IObservable<Unit> OnClick	{ get; }
		void SetSelectionState(EArsenalSelectionState state);
		void SetWhiteStroke(int count);
		void SetGreenStroke(int number);
		void DisableGreenStrokes();
		void PlayUpgradedTween();
	}

	
	public class UiArsenalPartView : MonoBehaviour, IUiArsenalPartView
	{
		[SerializeField] Button		_button;
		
		// Strokes
		[SerializeField] GameObject[] _whiteStrokes;
		[SerializeField] GameObject[] _greenStrokes;
		
		// Selection States
		[SerializeField] List<ArsenalSelectionData> _partDatas;

		[Title("Upgraded Tween")]
		[SerializeField] float      _glowFrom;
		[SerializeField] float      _glowTo;
		[SerializeField] float      _glowDur;
		[SerializeField] Ease		_glowEase;
		[SerializeField] float		_glowPlace;
		[SerializeField] float		_scaleTo;
		[SerializeField] float		_scaleDur;
		[SerializeField] Ease		_scaleEase;
		[SerializeField] RawImage   _glow;
		
		Sequence		_upgradedSequence;


		void OnDisable()		=> KillTween();
		

		public IObservable<Unit> OnClick	=> _button.OnClickAsObservable();
		
		
		public void SetSelectionState(EArsenalSelectionState state) 
		=> 
			_partDatas.ForEach( d => d.SelectionObject.SetActive( d.SelectionState == state ) );

		
		public void SetWhiteStroke(int count) 
		=> 
			_whiteStrokes.ForEach( (s, i) => s.SetActive( i < count ) );

		
		public void SetGreenStroke(int number) 
		=> 
			_greenStrokes.ForEach( (s, i) => s.SetActive( i == number ) );

		
		public void DisableGreenStrokes() 
		=> 
			_greenStrokes.ForEach( s => s.SetActive( false ) );

		
		public void PlayUpgradedTween()
		{
			KillTween();
			
			_upgradedSequence	= DOTween.Sequence();
			
			Tween scaleTween = transform
				.DOScale( _scaleTo, _scaleDur )
				.SetEase( _scaleEase )
				.SetLoops( 2, LoopType.Yoyo )
				.SetUpdate( true );

			Tween glowTween = DOTween.To(
					() => _glow.uvRect.x,
					x => _glow.uvRect = new Rect( x, 0, 1, 1 ),
					_glowTo,
					_glowDur
				)
				.From( _glowFrom )
				.SetEase( _glowEase )
				.SetUpdate( true );

			_upgradedSequence.Insert( 0, scaleTween );
			_upgradedSequence.Insert( _glowPlace, glowTween );
			_upgradedSequence.SetUpdate( true );
			_upgradedSequence.OnComplete( () => _upgradedSequence = null );
		}

		void KillTween()	=> _upgradedSequence?.Kill( true );
	}
}