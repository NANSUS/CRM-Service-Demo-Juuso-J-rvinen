using DemoNamespace;
using Linq2GraphQL.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;

internal class Program
{
    /// <summary>
    /// This is the entry point of the program. It is called when the program is started.
    /// </summary>
    public async static Task Main()
    {
        Console.WriteLine("Program started");
        // A service collection is a nice way to handle configurations.
        var serviceCollection = new ServiceCollection();

        // Add the Linq2GraphQL client configuration to the service collection
        // https://graph.imdbapi.dev/v1 is a demo database for the IMDB API that I found online. More details at https://imdbapi.dev/.

        var databaseUrl = "https://graph.imdbapi.dev/v1";

        serviceCollection
            .AddDemoClient(x => x.UseSafeMode = false)
            .WithHttpClient(x => x.BaseAddress = new Uri(databaseUrl));
        Console.WriteLine("Database client for " + databaseUrl + " configured");

        // Add a basic HTTP client to the service collection for later Pastebin usage
        serviceCollection.AddHttpClient("PastebinClient");
        Console.WriteLine("HTTP client for Pastebin configured");

        // Create an instance of the Linq2GraphQL client configuration
        var demoClient = serviceCollection.BuildServiceProvider().GetRequiredService<DemoClient>();
        Console.WriteLine("Database client for " + databaseUrl + " instantialized");

        // Build a query to get the plot of a movie by its ID. This query is quite simple because the IMDB API is quite limited.
        var idToSearch = "tt15398776";

        // The methods and fields "Title" and "Plot" are generated beforehand by the Linq2GraphQL schema generator command ran from the command line based on the GraphQL schema of the database that exists at https://graph.imdbapi.dev/v1.
        var plotOfATitle = await demoClient
            .Query
                .Title(idToSearch)
                .Select(x => x.Plot)
                .ExecuteAsync();
        Console.WriteLine("Query for ID " + idToSearch + " completed");

        Console.WriteLine("Query result below");
        Console.WriteLine(plotOfATitle);
        Console.WriteLine("Query result above");

        // Ask the user if they want to upload the result to Pastebin
        Console.Write("Input your Pastebin dev API key (https://pastebin.com/doc_api) (leave empty if not interestred) (Press Enter to accept input): ");

        var pastebinKey = Console.ReadLine();

        if (string.IsNullOrEmpty(pastebinKey))
        {
            Console.WriteLine("No Pastebin API key provided, skipping Pastebin upload.");
        }
        else
        {
            // Create a Pastebin upload request. Pastebin accepts data in this specific format.
            var pastebinData = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("api_dev_key", pastebinKey),
                new KeyValuePair<string, string>("api_option", "paste"),
                new KeyValuePair<string, string>("api_paste_code", plotOfATitle),
                new KeyValuePair<string, string>("api_paste_name", "Movie Plot (Demo program Juuso Järvinen)"),
                new KeyValuePair<string, string>("api_paste_expire_date", "1M")
            });

            // Instantiate the Pastebin HTTP client from the previously configured service collection
            var httpClient = serviceCollection.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("PastebinClient");

            // Send a POST message to Pastebin and receive a response
            var response = await httpClient.PostAsync("https://pastebin.com/api/api_post.php", pastebinData);

            // Display whether the upload was successful or not
            Console.WriteLine((response.IsSuccessStatusCode ? "Upload success!" : "Upload failed!") + " Pastebin response: " + await response.Content.ReadAsStringAsync());
        }

        // This is just a simple way to keep the console window open until the user presses a key, so that the user can take their time reading any text on the console.
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}