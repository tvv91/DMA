﻿namespace Web.Models
{
    /// <summary>
    /// Information about album genre
    /// </summary>
    public class Genre
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
