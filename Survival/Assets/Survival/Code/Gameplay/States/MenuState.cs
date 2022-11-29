using TMG.Survival.Gameplay.Messages;
using TMG.Survival.Infrastructure.PubSub;
using VContainer;
using VContainer.Unity;

namespace TMG.Survival.Gameplay
{
    public class MenuState : LifetimeScope
    {
        private IPubSubService _pubSubService;

        [Inject]
        private void Construct(IPubSubService pubSubService)
        {
            _pubSubService = pubSubService;
            _pubSubService = pubSubService;
        }

        public void StartGame()
        {
            _pubSubService.Publish(new LoadGameplayMessage());
        }

        public void Quit()
        {
            _pubSubService.Publish(new QuitApplicationMessage());
        }
    }
}