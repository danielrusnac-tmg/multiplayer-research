using System.Collections;
using TMG.Survival.Gameplay.Messages;
using TMG.Survival.Infrastructure;
using TMG.Survival.Infrastructure.PubSub;
using TMG.Survival.SceneManagement;
using TMG.Survival.ScreenCurtain;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace TMG.Survival.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope, ICoroutineRunner
    {
        [SerializeField] private CoroutineRunnerBehavior _coroutineRunner;
        [SerializeField] private AlphaScreenCurtain _screenCurtain;
        [SerializeField] private GameScene[] _defaultScenes;

        private bool _isRestarting;
        private ISceneLoader _sceneLoader;
        private IPubSubService _pubSubService;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(_screenCurtain).As<IScreenCurtain>();
            builder.RegisterInstance(_coroutineRunner).As<ICoroutineRunner>();
            builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();
            builder.Register<PubSubService>(Lifetime.Singleton).As<IPubSubService>();
        }

        [Inject]
        private void Construct(ISceneLoader sceneLoader, IPubSubService pubSubService)
        {
            _pubSubService = pubSubService;
            _sceneLoader = sceneLoader;
        }

        private IEnumerator Start()
        {
            _pubSubService.RegisterListener<QuitApplicationMessage>(OnQuitRequested);
            
            _screenCurtain.ShowCurtainImmediate();

            foreach (GameScene scene in _defaultScenes)
                yield return _sceneLoader.Load(scene);

            yield return _screenCurtain.HideCurtain();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _pubSubService.UnregisterListener<QuitApplicationMessage>(OnQuitRequested);
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
            _pubSubService.UnregisterListener<QuitApplicationMessage>(OnQuitRequested);
            
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

        private void OnQuitRequested()
        {
            Quit();
        }
    }
}