namespace Game.World
{
    public interface IHaveIdentity
    {
        int Id { get; set; }
        
        void GenerateUniqueId();
    }
}