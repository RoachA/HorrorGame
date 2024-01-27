namespace Game.World.Objects
{
    public interface IHaveIdentity
    {
        int Id { get; set; }
        
        void GenerateUniqueId();
    }
}