using Unity.Entities;

namespace GameLife
{
    /// <summary>
    /// Keeps track if cell is clicked
    /// </summary>
    [System.Serializable]
    public struct ClickStatus : IComponentData
    {
        public bool clicked;
    }
}
