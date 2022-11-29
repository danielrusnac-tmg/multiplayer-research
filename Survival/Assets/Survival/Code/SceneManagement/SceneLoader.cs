using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace TMG.Survival.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        private HashSet<GameScene> _loadedScenes = new();

        public bool IsLoaded(GameScene scene) => _loadedScenes.Contains(scene);

        public IEnumerator Load(GameScene scene)
        {
            if (!_loadedScenes.Add(scene))
                yield break;

            yield return SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
            UpdateActiveScene();
        }

        public IEnumerator Unload(GameScene scene)
        {
            if (!_loadedScenes.Remove(scene))
                yield break;

            yield return SceneManager.UnloadSceneAsync(scene.SceneName);
            UpdateActiveScene();
        }

        private void UpdateActiveScene()
        {
            if (_loadedScenes.Count == 0)
                return;

            Scene sceneToActivate = GetSceneToActivate();
            ActivateScene(sceneToActivate);
        }

        private static void ActivateScene(Scene sceneToActivate)
        {
            if (SceneManager.GetActiveScene() == sceneToActivate)
                return;

            SceneManager.SetActiveScene(sceneToActivate);
        }

        private Scene GetSceneToActivate()
        {
            int maxOrder = 0;
            GameScene gameSceneToActivate = _loadedScenes.First();

            foreach (GameScene scene in _loadedScenes)
            {
                if (scene.Order == maxOrder)
                    gameSceneToActivate = scene;

                if (scene.Order > maxOrder)
                {
                    maxOrder = scene.Order;
                    gameSceneToActivate = scene;
                }
            }

            Scene sceneToActivate = SceneManager.GetSceneByName(gameSceneToActivate.SceneName);
            return sceneToActivate;
        }
    }
}