using Unity.Netcode;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayObject : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _netWorkObject;
        
        public void Initialize()
        {
            _netWorkObject.Spawn();
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log($"{gameObject.name} Spawn Complete");
            
        }
    }
}
