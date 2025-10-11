﻿namespace Web.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Digitization> Digitizations { get; set; } = [];
    }
}
