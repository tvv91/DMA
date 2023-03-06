using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Adc> Adces { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Amplifier> Amplifiers { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Bitness> Bitnesses { get; set; }
        public DbSet<Cartrige> Cartriges { get; set; }
        public DbSet<Codec> Codecs { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Format> Formats { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Processing> Processings { get; set; }
        public DbSet<Reissue> Reissues { get; set; }
        public DbSet<Sampling> Samplings { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Year> Years { get; set; }
    }
}
