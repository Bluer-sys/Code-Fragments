namespace Game.UI
{
	using System;
	using Game.Core;
	using UniRx;
	using Zenject;

	
	public class UiArsenalPartController : IInitializable, IDisposable
	{
		readonly CompositeDisposable _lifetimeDisposables = new CompositeDisposable();

		[Inject] IUiArsenalPartView		_partView;
		[Inject] GameEvents				_gameEvents;
		[Inject] EPart					_partType;
		[Inject] IGcUpgrades			_gcUpgrades;

		EPart SelectedPart			=> _gameEvents.SelectedPart.Value;
		EWeapon SelectedWeapon		=> _gameEvents.SelectedWeapon.Value;
		
		public void Initialize()
		{
			// On Click
			_partView.OnClick
				.Subscribe( _ => _gameEvents.SelectedPart.SetValueAndForceNotify( _partType ) )
				.AddTo( _lifetimeDisposables );
			
			// On Part Changed
			Observable.Merge( 
					_gameEvents.ProposedPart.Select( _ => SelectedPart ), 			
					_gameEvents.SelectedPart )
				.Subscribe( Refresh )
				.AddTo( _lifetimeDisposables );
			
			// On Upgraded
			_gcUpgrades.OnBought
				.Subscribe( _ => OnUpgraded() )
				.AddTo( _lifetimeDisposables );
			
			// On Enter Upgrades
			_gameEvents.CurArsenalState
				.Where( s => s == EArsenalState.Upgrade )
				.Subscribe( _ => OnEnterUpgrades() )
				.AddTo( _lifetimeDisposables );
		}

		void OnUpgraded()
		{
			SetStrokes();

			if ( SelectedPart == _partType )
				_partView.PlayUpgradedTween();
		}

		void OnEnterUpgrades()
		{
			Refresh( SelectedPart );
		}


		public void Dispose() => _lifetimeDisposables?.Dispose();


		void Refresh(EPart part)
		{
			bool isThis		= part == _partType;
			bool isProposed	= _partType == _gameEvents.ProposedPart.Value && 
			                  _gameEvents.IsArsenalPlayState;
			bool isSelect	= _partType == SelectedPart;

			var selectType	= isThis switch
			{
				true when isProposed && isSelect		=> EArsenalSelectionState.Both,
				true when !isProposed && isSelect		=> EArsenalSelectionState.Selected,
				false when isProposed && !isSelect		=> EArsenalSelectionState.Applied,
				_										=> EArsenalSelectionState.None
			};
			
			// Selection State
			_partView.SetSelectionState( selectType );

			// Strokes
			SetStrokes( part, isThis );
		}


		void SetStrokes()
		{
			EPart part		= SelectedPart;
			bool isThis		= part == _partType;

			// Strokes
			SetStrokes( part, isThis );
		}

		
		void SetStrokes( EPart part, bool isThis )
		{
			// White
			_partView.SetWhiteStroke( _gcUpgrades.GetCurLevel( SelectedWeapon, _partType ) );

			// Green
			if ( isThis )
				_partView.SetGreenStroke( _gcUpgrades.GetCurLevel( SelectedWeapon, part ) );
			else
				_partView.DisableGreenStrokes();
		}
	}
}