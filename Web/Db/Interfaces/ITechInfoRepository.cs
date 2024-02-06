using Web.Models;

namespace Web.Db
{
    public interface ITechInfoRepository
    {
        IQueryable<Adc> Adcs { get; }
        IQueryable<Amplifier> Amplifiers { get; }
        IQueryable<Bitness> Bitnesses { get; }
        IQueryable<Cartrige> Cartriges { get; }
        IQueryable<Codec> Codecs { get; }
        IQueryable<Device> Devices { get; }
        IQueryable<Format> Formats { get; }
        IQueryable<Processing> Processings { get; }
        IQueryable<Sampling> Samplings { get; }
        IQueryable<State> States { get; }
    }
}
