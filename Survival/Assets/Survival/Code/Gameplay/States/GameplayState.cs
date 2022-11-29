using System.Collections;
using Fusion;
using TMG.Survival.Gameplay.Messages;
using TMG.Survival.Infrastructure.PubSub;
using TMG.Survival.Infrastructure.ScreenCurtain;
using TMG.Survival.Networking;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TMG.Survival.Gameplay
{
    public class GameplayState : LifetimeScope
    {
        [SerializeField] private BasicSpawner _basicSpawner;

        private bool _left;
        private bool _isInitialized;
        private IScreenCurtain _curtain;
        private INetworkManager _networkManager;
        private IPubSubService _pubSubService;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(_basicSpawner).As<INetworkManager>();
        }

        [Inject]
        private void Construct(IScreenCurtain curtain, INetworkManager networkManager, IPubSubService pubSubService)
        {
            _pubSubService = pubSubService;
            _networkManager = networkManager;
            _curtain = curtain;
        }

        private void Start()
        {
            StartGame();
        }
        
        public void QuitGameplay()
        {
            if (!_isInitialized || _left)
                return;
            
            _networkManager.Leave();
            _pubSubService.Publish(new LoadMenuMessage());

            _left = true;
        }

        private async void StartGame()
        {
            if (await _networkManager.StartGame(GameMode.AutoHostOrClient))
            {
                StartCoroutine(OnGameFoundRoutine());
            }
            else
            {
                _pubSubService.Publish(new LoadMenuMessage());
            }
        }

        private IEnumerator OnGameFoundRoutine()
        {
            yield return _curtain.HideCurtain();
            _isInitialized = true;
        }
    }
}