using System.ComponentModel.DataAnnotations;
using Web.Enums;

namespace Web.Request
{
    public class EquipmentDataRequest
    {
        [Required]
        public string Model { get; set; }
        public string? Manufacturer { get; set; }
        public EntityType EntityType { get; set; }
        public ActionType? Action { get; set; }
        public int EquipmentId { get; set; }
        public string? EquipmentCover { get; set; }
        public string? Description { get; set; }
    }
}
