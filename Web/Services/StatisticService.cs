using DMA.Application.Statistics.ProcessStatistic;
using MediatR;
using Web.Interfaces;

namespace Web.Services;

public class StatisticService(IMediator mediator) : IStatisticService
{
    public Task<Statistic> ProcessAsync() => mediator.Send(new ProcessStatisticQuery());
}
