using Html;
using ModelBuilder.Models;

namespace ModelBuilder.Processors;

public interface IModelDescriptorProcessor
{
    T Process<T>(ModelDescription member, IPerformQuerySelection element);
}