using System.Collections;
using TMG.Survival.Gameplay.Messages;
using TMG.Survival.Infrastructure;
using TMG.Survival.Infrastructure.PubSub;
using TMG.Survival.Infrastructure.SceneManagement;
using TMG.Survival.Infrastructure.ScreenCurtain;
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
        [SerializeField] private GameScene[] _menuScenes;
        [SerializeField] private GameScene[] _gameplayScenes;

        private bool _isRestarting;
        private bool _isLoading;
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
            _pubSubService.RegisterListener<LoadMenuMessage>(OnLoadMenuRequested);
            _pubSubService.RegisterListener<LoadGameplayMessage>(OnLoadGameplayRequested);

            _screenCurtain.ShowCurtainImmediate();

            yield return LoadMenuRoutine();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _pubSubService.UnregisterListener<QuitApplicationMessage>(OnQuitRequested);
            _pubSubService.UnregisterListener<LoadMenuMessage>(OnLoadMenuRequested);
            _pubSubService.UnregisterListener<LoadGameplayMessage>(OnLoadGameplayRequested);
        }

        private IEnumerator LoadMenuRoutine()
        {
            yield return LoadScenes(_gameplayScenes, _menuScenes);
        }

        private IEnumerator LoadGameplayRoutine()
        {
            yield return LoadScenes(_menuScenes, _gameplayScenes);
        }

        private IEnumerator LoadScenes(GameScene[] unload, GameScene[] load)
        {
            if (_isLoading)
                yield break;
            
            _isLoading = true;
            yield return _screenCurtain.ShowCurtain();

            foreach (GameScene scene in unload)
                yield return _sceneLoader.Unload(scene);

            foreach (GameScene scene in load)
                yield return _sceneLoader.Load(scene);

            yield return _screenCurtain.HideCurtain();
            _isLoading = false;
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

        private void OnLoadMenuRequested()
        {
            if (!_isLoading)
                StartCoroutine(LoadMenuRoutine());
        }

        private void OnLoadGameplayRequested()
        {
            if (!_isLoading)
                StartCoroutine(LoadGameplayRoutine());
        }

        private void OnQuitRequested()
        {
            Quit();
        }
    }
}