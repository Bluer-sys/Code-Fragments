using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Buttons
{
    public abstract class AbstractButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void OnValidate()
        {
            _button.CheckNotNull();
        }

        private void Awake()
        {
            _button.onClick.AddListener(PerformedAction);
        }

        protected abstract void PerformedAction();
    }
}
