using Cinemachine;
using Fusion;
using UnityEngine;

namespace TMG.Survival.Gameplay
{
    public class GameplayCamera : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        private bool _isTargetSet;
        private static CinemachineVirtualCamera s_instance;

        private void Awake()
        {
            s_instance = _virtualCamera;
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!_isTargetSet)
            {
                NetworkObject localObject = Runner.GetPlayerObject(Runner.LocalPlayer);
                if (localObject != null)
                {
                    SetTarget(localObject.transform);
                    _isTargetSet = true;
                }
            }
        }

        public static void SetTarget(Transform target)
        {
            s_instance.m_Follow = target;
            s_instance.m_LookAt = target;
        }
    }
}