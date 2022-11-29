using UnityEditor;
using UnityEngine;

namespace TMG.Survival.Infrastructure.SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Loader/Game Scene", fileName = "scene_")]
    public class GameScene : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset _scene;
#endif
        [SerializeField] [HideInInspector] private string _sceneName;
        [Tooltip("The scene with the highest order (or the same order for last loaded) will be set active")]
        [SerializeField] private int _order;

        public int Order => _order;
        public string SceneName => _sceneName;

        private void OnValidate()
        {
            UpdateSceneName();
        }

        private void UpdateSceneName()
        {
#if UNITY_EDITOR
            string newSceneName = _scene == null ? string.Empty : _scene.name;

            if (string.Equals(newSceneName, _sceneName))
                return;

            _sceneName = newSceneName;
            EditorUtility.SetDirty(this);
#endif
        }
    }
}