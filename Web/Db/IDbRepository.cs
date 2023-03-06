using Web.Models;

namespace Web.Db
{
    public interface IDbRepository
    {
        IQueryable<Adc> Adcs { get; }
        IQueryable<Album> Albums { get; }
        IQueryable<Amplifier> Amplifiers { get; }
        IQueryable<Artist> Artists { get; }
        IQueryable<Bitness> Bitnesses { get; }
        IQueryable<Cartrige> Cartriges { get; }
        IQueryable<Codec> Codecs { get; }
        IQueryable<Country> Countries { get; }
        IQueryable<Device> Devices { get; }
        IQueryable<Format> Formats { get; }
        IQueryable<Genre> Genres { get; }
        IQueryable<Label> Labels { get; }
        IQueryable<Processing> Processings { get; }
        IQueryable<Reissue> Reissues { get; }
        IQueryable<Sampling> Samplings { get; }
        IQueryable<State> States { get; }
        IQueryable<Year> Years { get; }

    }
}
