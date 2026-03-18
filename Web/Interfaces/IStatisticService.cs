using Web.Models;

namespace Web.Interfaces
{
    public interface IStatisticService
    {
        Task<Statistic> ProcessAsync();
    }
}

