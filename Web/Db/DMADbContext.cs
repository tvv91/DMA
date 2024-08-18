using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Web.Models;

namespace Web.Db
{
    public class DMADbContext : DbContext
    {
        public DMADbContext(DbContextOptions<DMADbContext> options) : base(options) { }
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
        public DbSet<Storage> Storages { get; set; }
        public DbSet<TechnicalInfo> TechnicalInfos { get; set;}

        // pre-defined entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // codecs
            modelBuilder.Entity<Codec>().HasData(new Codec { Id = 1, Data = "FLAC" });
            modelBuilder.Entity<Codec>().HasData(new Codec { Id = 2, Data = "DSD64" });
            modelBuilder.Entity<Codec>().HasData(new Codec { Id = 3, Data = "DSD128" });
            modelBuilder.Entity<Codec>().HasData(new Codec { Id = 4, Data = "DSD256" });
            modelBuilder.Entity<Codec>().HasData(new Codec { Id = 5, Data = "DSD512" });
            modelBuilder.Entity<Codec>().HasData(new Codec { Id = 6, Data = "WV" });
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
            modelBuilder.Entity<Format>().HasData(new Format { Id = 1, Data = "LP 12'' 33RPM" });
            modelBuilder.Entity<Format>().HasData(new Format { Id = 2, Data = "EP 10'' 45RPM" });
            modelBuilder.Entity<Format>().HasData(new Format { Id = 3, Data = "EP 12'' 45RPM" });
            modelBuilder.Entity<Format>().HasData(new Format { Id = 4, Data = "SINGLE 7'' 45RPM" });
            modelBuilder.Entity<Format>().HasData(new Format { Id = 5, Data = "SINGLE 12'' 45RPM" });
            modelBuilder.Entity<Format>().HasData(new Format { Id = 6, Data = "SHELLAC 10'' 78RPM" });
            // vinyl state
            modelBuilder.Entity<State>().HasData(new State { Id = 1, Data = "Mint" });
            modelBuilder.Entity<State>().HasData(new State { Id = 2, Data = "Near Mint" });
            modelBuilder.Entity<State>().HasData(new State { Id = 3, Data = "Very Good+" });
            modelBuilder.Entity<State>().HasData(new State { Id = 4, Data = "Very Good" });
            modelBuilder.Entity<State>().HasData(new State { Id = 5, Data = "Good" });
            modelBuilder.Entity<State>().HasData(new State { Id = 6, Data = "Unknown" });
        }
    }
}
