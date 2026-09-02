using DMA.Domain.Albums;
using DMA.Domain.Equipment;
using DMA.Domain.Identity;
using DMA.Domain.Posts;
using DMA.Domain.ReferenceData;
using DMA.Domain.Statistics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMA.Infrastructure.Persistence;

public class DmaDbContext(DbContextOptions<DmaDbContext> options) : IdentityDbContext<ApplicationUser>(options)
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

    #region Release
    public DbSet<Bitness> Bitnesses { get; set; }
    public DbSet<Sampling> Samplings { get; set; }
    public DbSet<DigitalFormat> DigitalFormats { get; set; }
    public DbSet<SourceFormat> SourceFormats { get; set; }
    public DbSet<VinylState> VinylStates { get; set; }
    public DbSet<Release> Releases { get; set; }
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 1, Name = "FLAC" });
        modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 2, Name = "DSD64" });
        modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 3, Name = "DSD128" });
        modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 4, Name = "DSD256" });
        modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 5, Name = "DSD512" });
        modelBuilder.Entity<DigitalFormat>().HasData(new DigitalFormat { Id = 6, Name = "WV" });
        modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 1, Value = 1 });
        modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 2, Value = 24 });
        modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 3, Value = 32 });
        modelBuilder.Entity<Bitness>().HasData(new Bitness { Id = 4, Value = 64 });
        modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 1, Value = 96 });
        modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 2, Value = 192 });
        modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 3, Value = 384 });
        modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 4, Value = 2.8 });
        modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 5, Value = 5.6 });
        modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 6, Value = 11.2 });
        modelBuilder.Entity<Sampling>().HasData(new Sampling { Id = 7, Value = 22.5 });
        modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 1, Name = "LP 12'' 33RPM" });
        modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 2, Name = "EP 10'' 45RPM" });
        modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 3, Name = "EP 12'' 45RPM" });
        modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 4, Name = "SINGLE 7'' 45RPM" });
        modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 5, Name = "SINGLE 12'' 45RPM" });
        modelBuilder.Entity<SourceFormat>().HasData(new SourceFormat { Id = 6, Name = "SHELLAC 10'' 78RPM" });
        modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 1, Name = "Mint" });
        modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 2, Name = "Near Mint" });
        modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 3, Name = "Very Good+" });
        modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 4, Name = "Very Good" });
        modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 5, Name = "Good" });
        modelBuilder.Entity<VinylState>().HasData(new VinylState { Id = 6, Name = "Unknown" });

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

        modelBuilder.Entity<Album>().HasIndex(a => a.Title);
        modelBuilder.Entity<Album>().HasIndex(a => a.ArtistId);
        modelBuilder.Entity<Album>().HasIndex(a => a.GenreId);
        modelBuilder.Entity<Release>().HasIndex(d => d.AlbumId);
        modelBuilder.Entity<Release>().HasIndex(d => d.CountryId);
        modelBuilder.Entity<Release>().HasIndex(d => d.LabelId);
        modelBuilder.Entity<Release>().HasIndex(d => d.YearId);
        modelBuilder.Entity<Release>().HasIndex(d => d.StorageId);
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

        modelBuilder.Entity<Release>()
            .HasOne(d => d.Album)
            .WithMany(a => a.Releases)
            .HasForeignKey(d => d.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Release>()
            .HasOne(d => d.FormatInfo)
            .WithMany()
            .HasForeignKey(d => d.FormatInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Release>()
            .HasOne(d => d.EquipmentInfo)
            .WithMany()
            .HasForeignKey(d => d.EquipmentInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Release>()
            .HasOne(d => d.Country)
            .WithMany(c => c.Releases)
            .HasForeignKey(d => d.CountryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Release>()
            .HasOne(d => d.Label)
            .WithMany(l => l.Releases)
            .HasForeignKey(d => d.LabelId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Release>()
            .HasOne(d => d.Year)
            .WithMany(y => y.Releases)
            .HasForeignKey(d => d.YearId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Release>()
            .HasOne(d => d.Reissue)
            .WithMany(r => r.Releases)
            .HasForeignKey(d => d.ReissueId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Release>()
            .HasOne(d => d.Storage)
            .WithMany(s => s.Releases)
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
