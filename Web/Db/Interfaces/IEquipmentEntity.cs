namespace Web.Db.Interfaces
{
    public interface IEquipmentEntity<TManufacturer> where TManufacturer : class, IManufacturer
    {
        int Id { get; }
        string? Data { get; }
        string? Description { get; }
        TManufacturer? Manufacturer { get; }
    }
}
