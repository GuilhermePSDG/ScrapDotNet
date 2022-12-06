using App;
using Html.Adapters;
using Requests.Contracts;
using Requests.Models;
using System.Text.Json;

var produto_descriptor = new MercadoLivreProdutoDescriptor();

var parser = new ModelDescriptorParser<Produto>(produto_descriptor,new AngleSharpDocumentAdapter());

var handler = new ProdutoRequestHandler();
var processor = new TypedRequestProcessor<Produto>(parser,handler);

var requests = new List<Request>
{
    new("none",new("https://www.mercadolivre.com.br/moto-e7-power-dual-sim-32-gb-vermelho-coral-2-gb-ram/p/MLB17840063#reco_item_pos=1&reco_backend=machinalis-homes-pdp-boos&reco_backend_type=function&reco_client=home_navigation-recommendations&reco_id=3c17ed58-fc3b-461c-9a53-4ad50a04c29e&c_id=/home/navigation-recommendations/element&c_element_order=2&c_uid=04a13b47-2007-4c41-b6d6-2dde393e1c6b"))
};

processor.AddRequest(requests);
await processor.ProcessAsync();

public class ProdutoRequestHandler : IRequestResultHandler<Produto>
{
    public Task EndOfWorkAsync()
    {
        Console.WriteLine("Done");
        return Task.CompletedTask;
    }

    public Task<Next> HandleFailureAsync(RequestResult result, IBaseProcessor requestProcessor)
    {
        Console.WriteLine(
            $"Request Failed" +
            $"\r\nCode : {result.StatusCode.ToString()}" +
            $"\r\nMsg : {result.Exception?.Message ?? "None"}");
        return Task.FromResult(Next.Continue);
    }

    public Task HandleSuccessAsync(Produto data, RequestResult context, IBaseProcessor requestProcessor)
    {
        Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions()
        {
            WriteIndented = true,
        }));
        return Task.CompletedTask;
    }
}
