using System.Collections;
using TMG.Survival.SceneManagement;
using TMG.Survival.ScreenCurtain;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace TMG.Survival.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope
    {
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private AlphaScreenCurtain _screenCurtain;
        [SerializeField] private GameScene[] _defaultScenes;

        private bool _isRestarting;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(_sceneLoader).AsImplementedInterfaces();
            builder.RegisterInstance(_screenCurtain).AsImplementedInterfaces();
        }

        private IEnumerator Start()
        {
            _screenCurtain.ShowCurtainImmediate();

            foreach (GameScene scene in _defaultScenes)
                yield return _sceneLoader.Load(scene);

            yield return _screenCurtain.HideCurtain();
        }

        [ContextMenu(nameof(Restart))]
        public void Restart()
        {
            if (_isRestarting)
                return;

            StartCoroutine(RestartRoutine());
        }

        [ContextMenu(nameof(Quit))]
        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private IEnumerator RestartRoutine()
        {
            _isRestarting = true;
            yield return _screenCurtain.ShowCurtain();
            SceneManager.LoadScene(0);
        }
    }
}