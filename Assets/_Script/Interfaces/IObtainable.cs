namespace Game.World.Objects
{
    /// <summary>
    /// it is an inventoryItem.
    /// </summary>
    public interface IObtainable : IInteractable, IHaveIdentity
    {
        public ObtainableType Type { get; set; }
        /// <summary>
        /// Register to inventory if ok.
        /// </summary>
        public void OnObtained();

        /// <summary>
        /// get from inventory is ok and drop.
        /// </summary>
        public void OnDiscarded();
    }

    public enum ObtainableType
    {
        Key,
        Consumable,
        StoryItem,
    }
}