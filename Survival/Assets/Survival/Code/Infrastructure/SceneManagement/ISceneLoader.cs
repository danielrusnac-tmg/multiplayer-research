using System.Collections;

namespace TMG.Survival.Infrastructure.SceneManagement
{
    public interface ISceneLoader
    {
        bool IsLoaded(GameScene scene);
        IEnumerator Load(GameScene scene);
        IEnumerator Unload(GameScene scene);
    }
}