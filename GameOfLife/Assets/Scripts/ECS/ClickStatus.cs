using Unity.Entities;

[System.Serializable]
public struct ClickStatus : IComponentData
{
    public bool clicked;
}