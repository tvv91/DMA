using Web.Models;

namespace Web.Interfaces
{
    public interface IStatisticRepository
    {
        Task<Statistic> Process();
    }
}
