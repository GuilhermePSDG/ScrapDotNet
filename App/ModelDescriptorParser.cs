using Html;
using Html.Adapters;
using ModelBuilder.Models;
using ModelBuilder.Processors;
using Requests.Contracts;
using Requests.Models;

public class ModelDescriptorParser<T> : IRequestResultParser<T>
{
    private ModelDescription modelDescriptor;
    private readonly IHtmlLoader loader;
    private readonly IModelDescriptorProcessor modelDescriptorProcessor;

    public ModelDescriptorParser(ModelDescription modelDescriptor, IHtmlLoader loader, IModelDescriptorProcessor? modelDescriptorProcessor = null)
    {
        this.modelDescriptorProcessor = modelDescriptorProcessor ?? new ModelDescriptorProcessor();
        this.modelDescriptor = modelDescriptor;
        this.loader = loader;
    }
    public T? TryParse(RequestResult result, out Exception? exception)
    {
        try
        {
            var html = loader.Load(result.StringContent!);
            var parsed = this.modelDescriptorProcessor.Process<T>(this.modelDescriptor, html);
            if (parsed == null) throw new InvalidOperationException("Not null procesed result");
            exception = null;
            return parsed;
        }
        catch (Exception ex)
        {
            exception = ex;
            return default;
        }
    }
}