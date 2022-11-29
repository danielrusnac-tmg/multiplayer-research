using System.Collections;
using UnityEngine;

namespace TMG.Survival.Infrastructure.ScreenCurtain
{
    public class AlphaScreenCurtain : MonoBehaviour, IScreenCurtain
    {
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private AnimationCurve _showCurtainEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _hideCurtainEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public bool IsCurtainShown { get; private set; }

        private void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
        }

        private void Awake()
        {
            HideCurtainImmediate();
        }

        public IEnumerator ShowCurtain()
        {
            StopFade();

            _canvas.enabled = true;
            yield return LerpAlpha(1f, _duration, _showCurtainEase);
            
            OnCurtainShown();
        }

        public void ShowCurtainImmediate()
        {
            StopFade();
            OnCurtainShown();
        }

        public IEnumerator HideCurtain()
        {
            StopFade();

            _canvas.enabled = true;
            yield return LerpAlpha(0f, _duration, _hideCurtainEase);
            
            OnCurtainHidden();
        }

        public void HideCurtainImmediate()
        {
            StopFade();
            OnCurtainHidden();
        }

        private void StopFade()
        {
            StopAllCoroutines();
        }

        private void OnCurtainShown()
        {
            _canvasGroup.alpha = 1f;
            _canvas.enabled = true;
            IsCurtainShown = true;
        }

        private void OnCurtainHidden()
        {
            _canvas.enabled = false;
            IsCurtainShown = false;
        }

        private IEnumerator LerpAlpha(float to, float duration, AnimationCurve ease)
        {
            float from = _canvasGroup.alpha;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(from, to, ease.Evaluate(time / duration));
                yield return null;
            }
        }
    }
}