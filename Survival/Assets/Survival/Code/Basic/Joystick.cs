using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace TMG.Survival.Gameplay
{
	public class Joystick : OnScreenControl
	{
		private const float ACTIVE_ALPHA = 1f;
		private const float INACTIVE_ALPHA = 0.3f;

		[Range(0f, 1f)]
		[SerializeField] private float _radius = 1f;
		[SerializeField] private RectTransform _defaultJoystickPosition;
		[SerializeField] private RectTransform _handle;
		[SerializeField] private RectTransform _constrain;
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private Camera _camera;

		[InputControl(layout = "Vector2")]
		[SerializeField] private string _controlPath;

		private Vector2 _startDragPosition;
		private RectTransform _rectTransform;
		private bool _isActive;

		private float ConstrainRadius => _constrain.rect.width * 0.5f * _radius;

		protected override string controlPathInternal
		{
			get => _controlPath;
			set => _controlPath = value;
		}

		private void Awake()
		{
			_rectTransform = (RectTransform)transform;
			_canvasGroup.alpha = INACTIVE_ALPHA;

			_constrain.gameObject.SetActive(false);
		}

		private void Start()
		{
			ResetAnchors();
			MoveToDefaultPosition();
		}

		private void Update()
		{
			if (EventSystem.current.currentInputModule.input.GetMouseButtonDown(0))
			{
				MoveToActivePosition(EventSystem.current.currentInputModule.input.mousePosition);
			}

			if (EventSystem.current.currentInputModule.input.GetMouseButton(0))
			{
				if (_startDragPosition != EventSystem.current.currentInputModule.input.mousePosition && !_isActive) 
					_isActive = true;

				if (_isActive)
					OnDrag(EventSystem.current.currentInputModule.input.mousePosition);
			}

			if (EventSystem.current.currentInputModule.input.GetMouseButtonUp(0))
			{
				_isActive = false;
				OnPointerUp();
			}
		}

		private void OnDrag(Vector2 position)
		{
			_constrain.gameObject.SetActive(true);

			Vector2 direction = Vector2.ClampMagnitude(
				position - _startDragPosition,
				ConstrainRadius);

			_handle.localPosition = direction;
			SendValueToControl(direction / ConstrainRadius);
		}

		private void OnPointerUp()
		{
			_constrain.gameObject.SetActive(false);

			MoveToDefaultPosition();
			SendValueToControl(Vector2.zero);
		}

		private void ResetAnchors()
		{
			_constrain.anchorMin = new Vector2(0.5f, 0.5f);
			_constrain.anchorMax = new Vector2(0.5f, 0.5f);
		}

		private void MoveToActivePosition(Vector2 position)
		{
			_constrain.position = position;
			_startDragPosition = position;
			_canvasGroup.alpha = ACTIVE_ALPHA;
		}

		private Vector2 ScreenToLocalPosition(Vector2 screenPosition)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				_rectTransform,
				screenPosition,
				_camera,
				out Vector2 position);

			return position;
		}

		private void MoveToDefaultPosition()
		{
			_canvasGroup.alpha = INACTIVE_ALPHA;
			_constrain.position = _defaultJoystickPosition.position;
			_handle.localPosition = Vector2.zero;
		}
	}
}