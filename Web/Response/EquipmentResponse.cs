﻿using Web.Enums;

namespace Web.Response
{
    public class EquipmentResponse
    {
        public int Id { get; set; }
        public EntityType EntityType { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
    }
}
