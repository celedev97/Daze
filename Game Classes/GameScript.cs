namespace Daze {
    /// <summary>
    /// A GameScript is a script called by the Engine when it starts and then automatically every Game Cycle
    /// </summary>
    public interface GameScript {
        /// <summary>
        /// A method that keep being called continuosly
        /// </summary>
        void Update();

        /// <summary>
        /// This method is called just once, just after the script initialization
        /// </summary>
        void Start();
    }
}
