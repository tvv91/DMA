using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db
{
    public class DMADbContext : DbContext
    {
        public DMADbContext(DbContextOptions<DMADbContext> options) : base(options) { }
        
        #region Base info
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Reissue> Reissues { get; set; }
        public DbSet<Year> Years { get; set; }
        public DbSet<Storage> Storages { get; set; }
        #endregion

        #region Technical Info
        public DbSet<Adc> Adces { get; set; }
        public DbSet<AdcManufacturer> AdcManufacturers { get; set; }
        public DbSet<Amplifier> Amplifiers { get; set; }
        public DbSet<AmplifierManufacturer> AmplifierManufacturers { get; set; }
        public DbSet<Bitness> Bitnesses { get; set; }
        public DbSet<Cartridge> Cartridges { get; set; }
        public DbSet<CartridgeManufacturer> CartridgeManufacturers { get; set; }
        public DbSet<DigitalFormat> DigitalFormats { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerManufacturer> PlayerManufacturers { get; set; }
        public DbSet<SourceFormat> SourceFormats { get; set; }
        public DbSet<Sampling> Samplings { get; set; }
        public DbSet<VinylState> VinylStates { get; set; }
        public DbSet<Wire> Wires { get; set; }
        public DbSet<WireManufacturer> WireManufacturers { get; set; }
        public DbSet<TechnicalInfo> TechnicalInfos { get; set; }
        public DbSet<Statistic> Statistic { get; set; }
        #endregion

        // pre-defined entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // digital formats
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 1, Data = "FLAC" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 2, Data = "DSD64" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 3, Data = "DSD128" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 4, Data = "DSD256" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 5, Data = "DSD512" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 6, Data = "WV" });
            // bitness
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 1, Data = 1 });
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 2, Data = 24 });
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 3, Data = 32 });
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 4, Data = 64 });
            // sampling
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 1, Data = 96 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 2, Data = 192 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 3, Data = 384 });
            // dsd sampling
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 4, Data = 2.8 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 5, Data = 5.6 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 6, Data = 11.2 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 7, Data = 22.5 });
            // format
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 1, Data = "LP 12'' 33RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 2, Data = "EP 10'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 3, Data = "EP 12'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 4, Data = "SINGLE 7'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 5, Data = "SINGLE 12'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 6, Data = "SHELLAC 10'' 78RPM" });
            // vinyl state
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 1, Data = "Mint" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 2, Data = "Near Mint" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 3, Data = "Very Good+" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 4, Data = "Very Good" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 5, Data = "Good" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 6, Data = "Unknown" });
        }
    }
}
