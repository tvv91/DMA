using Web.Models;
using Web.Request;

namespace Web.Db
{
    public interface ITechInfoRepository
    {
        IQueryable<TechnicalInfo> TechInfos { get; }
        IQueryable<Adc> Adcs { get; }
        IQueryable<Amplifier> Amplifiers { get; }
        IQueryable<Bitness> Bitnesses { get; }
        IQueryable<Cartrige> Cartriges { get; }
        IQueryable<DigitalFormat> Codecs { get; }
        IQueryable<Player> Devices { get; }
        IQueryable<SourceFormat> Formats { get; }
        IQueryable<Processing> Processings { get; }
        IQueryable<Sampling> Samplings { get; }
        IQueryable<VinylState> States { get; }
        Task<TechnicalInfo> CreateNewTechnicallInfoAsync(AlbumDataRequest request);

    }
}
