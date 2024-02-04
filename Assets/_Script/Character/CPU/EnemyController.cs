using Game.World;
using UnityEngine;

public class EnemyController : MonoBehaviour, IHaveIdentity
{
    [SerializeField] private Animator _animator;
    
    public int Id { get; set; }
    
    public void GenerateUniqueId()
    {
        Id = UniqueIDHelper.GenerateUniqueId(this);
    }
    
    
}
