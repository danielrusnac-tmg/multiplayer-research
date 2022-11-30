using Fusion;
using UnityEngine;

namespace TMG.Survival.Gameplay
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private float _movementSpeed = 5f;

        private NetworkCharacterControllerPrototype _characterController;

        private void Awake()
        {
            _characterController = GetComponent<NetworkCharacterControllerPrototype>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
                _characterController.Move(Runner.DeltaTime * _movementSpeed * data.Direction);
        }
    }
}