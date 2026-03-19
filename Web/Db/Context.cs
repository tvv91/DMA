using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {        
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

            // Uniqueness constraints
            modelBuilder.Entity<Adc>().HasIndex(a => a.Name).IsUnique();
            modelBuilder.Entity<Amplifier>().HasIndex(a => a.Name).IsUnique();
            modelBuilder.Entity<Artist>().HasIndex(a => a.Name).IsUnique();
            modelBuilder.Entity<Cartridge>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<DigitalFormat>().HasIndex(d => d.Name).IsUnique();
            modelBuilder.Entity<SourceFormat>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Genre>().HasIndex(g => g.Name).IsUnique();
            modelBuilder.Entity<Label>().HasIndex(l => l.Name).IsUnique();
            modelBuilder.Entity<Manufacturer>().HasIndex(m => m.Name).IsUnique();
            modelBuilder.Entity<Player>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Storage>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<VinylState>().HasIndex(v => v.Name).IsUnique();
            modelBuilder.Entity<Wire>().HasIndex(w => w.Name).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(c => c.Title).IsUnique();
            modelBuilder.Entity<Bitness>().HasIndex(b => b.Value).IsUnique();
            modelBuilder.Entity<Reissue>().HasIndex(r => r.Value).IsUnique();
            modelBuilder.Entity<Sampling>().HasIndex(s => s.Value).IsUnique();
            modelBuilder.Entity<Year>().HasIndex(y => y.Value).IsUnique();

            // Performance indexes
            modelBuilder.Entity<Album>().HasIndex(a => a.Title);
            modelBuilder.Entity<Album>().HasIndex(a => a.ArtistId);
            modelBuilder.Entity<Album>().HasIndex(a => a.GenreId);
            modelBuilder.Entity<Digitization>().HasIndex(d => d.AlbumId);
            modelBuilder.Entity<Digitization>().HasIndex(d => d.CountryId);
            modelBuilder.Entity<Digitization>().HasIndex(d => d.LabelId);
            modelBuilder.Entity<Digitization>().HasIndex(d => d.YearId);
            modelBuilder.Entity<Digitization>().HasIndex(d => d.StorageId);
            modelBuilder.Entity<FormatInfo>().HasIndex(f => f.BitnessId);
            modelBuilder.Entity<FormatInfo>().HasIndex(f => f.SamplingId);
            modelBuilder.Entity<FormatInfo>().HasIndex(f => f.DigitalFormatId);
            modelBuilder.Entity<FormatInfo>().HasIndex(f => f.SourceFormatId);
            modelBuilder.Entity<FormatInfo>().HasIndex(f => f.VinylStateId);
            modelBuilder.Entity<EquipmentInfo>().HasIndex(e => e.PlayerId);
            modelBuilder.Entity<EquipmentInfo>().HasIndex(e => e.CartridgeId);
            modelBuilder.Entity<EquipmentInfo>().HasIndex(e => e.AmplifierId);
            modelBuilder.Entity<EquipmentInfo>().HasIndex(e => e.AdcId);
            modelBuilder.Entity<EquipmentInfo>().HasIndex(e => e.WireId);
            modelBuilder.Entity<Post>().HasIndex(p => p.CreatedDate);
            modelBuilder.Entity<Post>().HasIndex(p => p.IsDraft);
            modelBuilder.Entity<Post>().HasIndex(p => p.Title);
            modelBuilder.Entity<PostCategory>().HasIndex(pc => pc.PostId);
            modelBuilder.Entity<PostCategory>().HasIndex(pc => pc.CategoryId);
            modelBuilder.Entity<Player>().HasIndex(p => p.ManufacturerId);
            modelBuilder.Entity<Cartridge>().HasIndex(c => c.ManufacturerId);
            modelBuilder.Entity<Amplifier>().HasIndex(a => a.ManufacturerId);
            modelBuilder.Entity<Adc>().HasIndex(a => a.ManufacturerId);
            modelBuilder.Entity<Wire>().HasIndex(w => w.ManufacturerId);

            // Cascade delete configuration
            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.Album)
                .WithMany(a => a.Digitizations)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.FormatInfo)
                .WithMany()
                .HasForeignKey(d => d.FormatInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.EquipmentInfo)
                .WithMany()
                .HasForeignKey(d => d.EquipmentInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.Country)
                .WithMany(c => c.Digitizations)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.Label)
                .WithMany(l => l.Digitizations)
                .HasForeignKey(d => d.LabelId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.Year)
                .WithMany(y => y.Digitizations)
                .HasForeignKey(d => d.YearId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.Reissue)
                .WithMany(r => r.Digitizations)
                .HasForeignKey(d => d.ReissueId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Digitization>()
                .HasOne(d => d.Storage)
                .WithMany(s => s.Digitizations)
                .HasForeignKey(d => d.StorageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FormatInfo>()
                .HasOne(f => f.Bitness)
                .WithMany()
                .HasForeignKey(f => f.BitnessId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FormatInfo>()
                .HasOne(f => f.Sampling)
                .WithMany()
                .HasForeignKey(f => f.SamplingId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FormatInfo>()
                .HasOne(f => f.DigitalFormat)
                .WithMany()
                .HasForeignKey(f => f.DigitalFormatId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FormatInfo>()
                .HasOne(f => f.SourceFormat)
                .WithMany()
                .HasForeignKey(f => f.SourceFormatId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FormatInfo>()
                .HasOne(f => f.VinylState)
                .WithMany()
                .HasForeignKey(f => f.VinylStateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EquipmentInfo>()
                .HasOne(e => e.Player)
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EquipmentInfo>()
                .HasOne(e => e.Cartridge)
                .WithMany()
                .HasForeignKey(e => e.CartridgeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EquipmentInfo>()
                .HasOne(e => e.Amplifier)
                .WithMany()
                .HasForeignKey(e => e.AmplifierId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EquipmentInfo>()
                .HasOne(e => e.Adc)
                .WithMany()
                .HasForeignKey(e => e.AdcId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EquipmentInfo>()
                .HasOne(e => e.Wire)
                .WithMany()
                .HasForeignKey(e => e.WireId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PostCategory>()
                .HasOne(pc => pc.Post)
                .WithMany(p => p.PostCategories)
                .HasForeignKey(pc => pc.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.PostCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
