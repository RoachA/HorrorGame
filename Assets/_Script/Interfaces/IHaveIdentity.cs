using UnityEngine;
using Zenject;

namespace Game.World
{
    public interface IHaveIdentity
    {
        int Id { get; set; }
        GameObject Object { get; set; }
    }
}