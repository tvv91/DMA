using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Web.Common;
using Web.Enums;
using Web.Models;

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
        /// <summary>Albums that contain a digitization using this equipment (when viewing details).</summary>
        public PagedResult<Album>? AlbumsUsedIn { get; set; }
        public List<SelectListItem> Equipments { get; } = new List<SelectListItem>
        {
            new SelectListItem { Text = EntityType.Adc.ToString(), Value = EntityType.Adc.ToString() },
            new SelectListItem { Text = EntityType.Amplifier.ToString(), Value = EntityType.Amplifier.ToString() },
            new SelectListItem { Text = EntityType.Cartridge.ToString(), Value = EntityType.Cartridge.ToString() },
            new SelectListItem { Text = EntityType.Player.ToString(), Value = EntityType.Player.ToString() },
            new SelectListItem { Text = EntityType.Wire.ToString(), Value = EntityType.Wire.ToString() },
        };
    }
}
