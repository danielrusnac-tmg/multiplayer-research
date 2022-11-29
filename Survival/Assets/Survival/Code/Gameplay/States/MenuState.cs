using TMG.Survival.Gameplay.Messages;
using TMG.Survival.Infrastructure.PubSub;
using VContainer;
using VContainer.Unity;

namespace TMG.Survival.Gameplay
{
    public class MenuState : LifetimeScope
    {
        private IPubSubService _pubSubService;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }

        [Inject]
        private void Construct(IPubSubService pubSubService)
        {
            _pubSubService = pubSubService;
        }

        public void StartHost()
        {
            
        }

        public void StartJoin()
        {
            
        }

        public void Quit()
        {
            _pubSubService.Publish(new QuitApplicationMessage());
        }
    }
}