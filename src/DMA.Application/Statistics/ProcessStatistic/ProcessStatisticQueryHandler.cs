using System.Text.Json;
using DMA.Domain.Common;
using DMA.Domain.Statistics;
using MediatR;

namespace DMA.Application.Statistics.ProcessStatistic;

public sealed class ProcessStatisticQueryHandler(
    IStatisticRepository statisticRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<ProcessStatisticQuery, Statistic>
{
    public async Task<Statistic> Handle(ProcessStatisticQuery request, CancellationToken cancellationToken)
    {
        var stat = await statisticRepository.GetFirstAsync(cancellationToken);

        if (stat is null)
        {
            await StatisticRefreshGate.WaitAsync(cancellationToken);
            try
            {
                stat = await statisticRepository.GetFirstAsync(cancellationToken);
                if (stat is null)
                {
                    stat = await BuildFreshStatisticAsync(cancellationToken);
                    statisticRepository.Add(stat);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            finally
            {
                StatisticRefreshGate.Release();
            }

            return stat;
        }

        var needsRefresh = timeProvider.GetUtcNow().UtcDateTime - stat.LastUpdate > TimeSpan.FromDays(1);
        var canRefresh = StatisticRefreshGate.CanRefresh(timeProvider);

        if (needsRefresh && canRefresh)
        {
            await StatisticRefreshGate.WaitAsync(cancellationToken);
            try
            {
                stat = await statisticRepository.GetFirstAsync(cancellationToken);
                if (stat is not null && timeProvider.GetUtcNow().UtcDateTime - stat.LastUpdate > TimeSpan.FromDays(1))
                {
                    StatisticRefreshGate.MarkRefreshAttempt(timeProvider);
                    var refreshed = await BuildFreshStatisticAsync(cancellationToken);
                    stat.Data = refreshed.Data;
                    stat.LastUpdate = refreshed.LastUpdate;
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            finally
            {
                StatisticRefreshGate.Release();
            }
        }

        return stat;
    }

    private async Task<Statistic> BuildFreshStatisticAsync(CancellationToken cancellationToken)
    {
        var data = await statisticRepository.ComputeCountersAsync(cancellationToken);
        return new Statistic
        {
            Data = JsonSerializer.Serialize(data),
            LastUpdate = timeProvider.GetUtcNow().UtcDateTime
        };
    }
}
