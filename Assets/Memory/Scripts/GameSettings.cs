using Unity.Entities;

namespace Memory
{
    [System.Serializable]
    [GenerateAuthoringComponent]
    public struct GameSettings : IComponentData
    {
        public int matches;
        public int facesShowing;
        public float secondsToTurn;
    }
}