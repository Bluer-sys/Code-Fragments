namespace Game.Tutorial
{
    using System;
    using System.Collections;
    using Game.Configs;
    using Game.FX;
    using Game.Gameplay;
    using Game.Profiles;
    using Game.UI;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class TutorialController3 : MonoBehaviour, IInitializable, IDisposable
    {
        [Inject] IThermalController			_thermalController;
        [Inject] GameProfile				_gameProfile;
        [Inject] FeatureFlags				_featureFlags;
        [Inject] IZoomController            _zoomController;
        [Inject] IUiGameplayController      _uiGameplayController;
        
        IDisposable     _hideTutor;
        
        
        public void Initialize()
        {
            SetActive( false );

            if (!IsTutorial)
                return;
            
            StartCoroutine( Cor() );
        }

        public void Dispose()   => _hideTutor?.Dispose();

        IEnumerator Cor()
        {
            // Enable hand on zoom
            _hideTutor  = _zoomController.IsZoom.Subscribe( SetActive );
            
            // Wait until tap thermal button
            yield return _thermalController.IsThermal.Where( v => v ).First().ToYieldInstruction();
            
            // Deactivate Tutor
            _hideTutor.Dispose();
            SetActive( false );
        }

        void SetActive( bool state )      => _uiGameplayController.SetThermalTutorialHand( state );

        bool IsTutorial
        =>
            _featureFlags.Tutorial                  &&
            _gameProfile.LevelNumber.Value == 4; // Thermal Level
    }
}