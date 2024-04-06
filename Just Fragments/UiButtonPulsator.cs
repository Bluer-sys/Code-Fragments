namespace Game.UI
{
	using DG.Tweening;
	using Sirenix.OdinInspector;
	using UnityEngine;

	
	public class UiButtonPulsator : MonoBehaviour
	{
		[SerializeField] bool	_playOnAwake;
		[SerializeField] float	_scale;
		[SerializeField] float	_duration;
		[SerializeField] Ease	_scaleEase;
		
		Tween	_tween;


		void OnEnable()
		{
			if (_playOnAwake)
				Play();
		}

		void OnDisable()
		{
			if (_playOnAwake)
				RewindAndKill();
		}

		void OnDestroy()
		{
			RewindAndKill();
		}


		[Button]
		public void PlayStop( bool state )
		{
			if (state)
				Play();
			else
				RewindAndKill();
		}


		void Play()
		=>
			_tween = transform
				.DOScale( _scale, _duration )
				.SetEase( _scaleEase )
				.SetLoops( -1, LoopType.Yoyo )
				.SetUpdate( true );


		void RewindAndKill()
		{
			_tween?.Rewind();
			_tween?.Kill();

			_tween		= null;
		}
	}
}

