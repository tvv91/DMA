using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Web.Enums;

namespace Web.ViewModels
{
    public class EquipmentViewModel
    {
        public int Id { get; set; }
        [Required]
        public string ModelName { get; set; }
        public string? Manufacturer { get; set; }
        public EntityType EquipmentType { get; set; }
        public ActionType? Action { get; set; }
        //public int EquipmentId { get; set; }
        public string? EquipmentCover { get; set; }
        public string? Description { get; set; }
        public List<SelectListItem> Equipments { get; } = new List<SelectListItem>
        {
            new SelectListItem { Text = EntityType.Adc.ToString() },
            new SelectListItem { Text = EntityType.Amplifier.ToString() },
            new SelectListItem { Text = EntityType.Cartridge.ToString() },
            new SelectListItem { Text = EntityType.Player.ToString() },
            new SelectListItem { Text = EntityType.Wire.ToString() },
        };
    }
}
