using Core.ServiceLocator;
using UnityEngine;

namespace Gameplay.GameServices
{
    public class CameraHolder : MonoBehaviour
    {
        [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 5.82f, -2.17f);

        private PlayerBall _player;
        private Camera _cam;
        
        private void Start()
        {
            _player = Services.Get<PlayerHolder>().Player;
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            if (_player == null || _cam == null) return;
            Vector3 target = _player.transform.position + _cameraOffset;
            _cam.transform.position = Vector3.Lerp(_cam.transform.position, target, 5f * Time.deltaTime);
            _cam.transform.LookAt(_player.transform.position + Vector3.forward * 6f);
        }
    }
}