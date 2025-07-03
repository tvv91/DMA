using Web.Models;

namespace Web.Db.Interfaces
{
    public interface IStatisticRepository
    {
        Task<Statistic> Process();
    }
}
