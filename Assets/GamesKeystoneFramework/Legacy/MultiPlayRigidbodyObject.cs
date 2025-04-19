using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayRigidbodyObject : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        public void MultiInitialize(bool host = true)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = !host;
        }
    }
}
