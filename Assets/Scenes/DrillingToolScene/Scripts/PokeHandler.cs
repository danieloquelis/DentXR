using UnityEngine;
namespace Oculus.Interaction
{
    public class PokeHandler : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IInteractableView))]
        private UnityEngine.Object _interactableView;
        private IInteractableView InteractableView { get; set; }

        [SerializeField]
        private UnityEngine.Events.UnityEvent OnButtonSelected;

        private bool m_wasPressedDown = false;

        protected virtual void Awake()
        {
            InteractableView = _interactableView as IInteractableView;
        }

        protected virtual void OnEnable()
        {
            InteractableView.WhenStateChanged += CheckButtonState;
        }

        protected virtual void OnDisable()
        {
            InteractableView.WhenStateChanged -= CheckButtonState;
        }

        private void CheckButtonState(InteractableStateChangeArgs args)
        {
            if (InteractableView.State == InteractableState.Select)
                m_wasPressedDown = true;

            else if (InteractableView.State == InteractableState.Normal && m_wasPressedDown)
            {
                OnButtonSelected?.Invoke();
                m_wasPressedDown = false;
            }
        }
    }

}
