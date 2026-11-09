using Core.ServiceLocator;
using UnityEngine;

namespace Gameplay.GameServices
{
    public class PlayerHolder : MonoBehaviour, IService
    {
        [SerializeField] private PlayerBall _player;
        
        public PlayerBall Player => _player;
    }
}