using DMA.Domain.Statistics;
using MediatR;

namespace DMA.Application.Statistics.ProcessStatistic;

public sealed record ProcessStatisticQuery : IRequest<Statistic>;
