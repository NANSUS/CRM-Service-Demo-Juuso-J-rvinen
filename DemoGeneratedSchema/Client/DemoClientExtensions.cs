//---------------------------------------------------------------------
// This code was automatically generated by Linq2GraphQL
// Please don't edit this file
// Github:https://github.com/linq2graphql/linq2graphql.client
// Url: https://linq2graphql.com
//---------------------------------------------------------------------

using Linq2GraphQL.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DemoNamespace;

public static class DemoClientExtensions 
{
    private const string ClientName = "DemoClient";
        
    public static IGraphClientBuilder<DemoClient> AddDemoClient(this IServiceCollection services)
    {
        var graphClientOptions = new GraphClientOptions();
        return GraphClientBuilder(services, graphClientOptions);
    }
    
    public static IGraphClientBuilder<DemoClient> AddDemoClient(this IServiceCollection services, Action<GraphClientOptions> opts)
    {
        var graphClientOptions = new GraphClientOptions();
        opts(graphClientOptions);
        
        return GraphClientBuilder(services, graphClientOptions);
    }

    private static IGraphClientBuilder<DemoClient> GraphClientBuilder(IServiceCollection services,
        GraphClientOptions graphClientOptions)
    {
        var opts = Options.Create(graphClientOptions);
        services.AddKeyedSingleton(ClientName, opts); 
        services.AddMemoryCache();        
        return new ClientBuilder<DemoClient>(ClientName, services);
    }
}