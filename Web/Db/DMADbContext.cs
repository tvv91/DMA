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
        public DbSet<Year> Years { get; set; }
        public DbSet<Reissue> Reissues { get; set; }
        public DbSet<Storage> Storages { get; set; }
        #endregion

        #region Hardware
        public DbSet<Adc> Adces { get; set; }
        public DbSet<Amplifier> Amplifiers { get; set; }
        public DbSet<Cartridge> Cartridges { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Wire> Wires { get; set; }
        #endregion

        #region Manufacturers
        public DbSet<Manufacturer> Manufacturer { get; set; }
        #endregion

        #region Digitization
        public DbSet<Bitness> Bitnesses { get; set; }
        public DbSet<Sampling> Samplings { get; set; }
        public DbSet<DigitalFormat> DigitalFormats { get; set; }
        public DbSet<SourceFormat> SourceFormats { get; set; }
        public DbSet<VinylState> VinylStates { get; set; }
        public DbSet<Digitization> Digitizations { get; set; }
        public DbSet<FormatInfo> FormatInfos { get; set; }
        public DbSet<EquipmentInfo> EquipmentInfos { get; set; }
        #endregion

        #region Posts
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        #endregion

        #region Statistic
        public DbSet<Statistic> Statistics { get; set; }
        #endregion

        // pre-defined entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // digital formats
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 1, Name = "FLAC" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 2, Name = "DSD64" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 3, Name = "DSD128" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 4, Name = "DSD256" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 5, Name = "DSD512" });
            modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 6, Name = "WV" });
            // bitness
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 1, Value = 1 });
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 2, Value = 24 });
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 3, Value = 32 });
            modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 4, Value = 64 });
            // sampling
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 1, Value = 96 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 2, Value = 192 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 3, Value = 384 });
            // dsd sampling
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 4, Value = 2.8 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 5, Value = 5.6 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 6, Value = 11.2 });
            modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 7, Value = 22.5 });
            // format
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 1, Name = "LP 12'' 33RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 2, Name = "EP 10'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 3, Name = "EP 12'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 4, Name = "SINGLE 7'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 5, Name = "SINGLE 12'' 45RPM" });
            modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 6, Name = "SHELLAC 10'' 78RPM" });
            // vinyl state
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 1, Name = "Mint" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 2, Name = "Near Mint" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 3, Name = "Very Good+" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 4, Name = "Very Good" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 5, Name = "Good" });
            modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 6, Name = "Unknown" });
            // indexes
            modelBuilder.Entity<Digitization>().HasIndex(d => d.AlbumId);

            // cascade delete
            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.Album)
                .WithMany(a => a.Digitizations)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
