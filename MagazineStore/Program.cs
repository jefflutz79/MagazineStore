using MagazineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagazineStore
{
  class Program
  {
    static MagazineStoreClient client = new MagazineStoreClient();

    static async Task Main(string[] args) {
      await RunChallenge();
    }

    private static async Task RunChallenge() {
      Console.WriteLine("Starting challenge...");

      Console.WriteLine();
      Console.WriteLine("Fetching categories...");
      var getCategoriesTask = client.GetCategoriesAsync();

      Console.WriteLine("Fetching subscribers...");
      var getSubscribersTask = client.GetSubscribersAsync();
      
      var categories = await getCategoriesTask;
      Console.WriteLine($"Fetched {categories.Count} categories: " + string.Join(", ", categories));

      Console.WriteLine("Fetching magazines in each category...");
      var magazines = await FetchMagazines(categories);
      var magazineNames = from m in magazines select m.Name;
      Console.WriteLine($"Fetched {magazines.Count} magazines: " + string.Join(", ", magazineNames));

      var subscribers = await getSubscribersTask;
      var subscriberNames = from s in subscribers select $"{s.FirstName} {s.LastName}";
      Console.WriteLine($"Fetched {subscribers.Count} subscribers: " + string.Join(", ", subscriberNames));

      Console.WriteLine("Identifying subscribers to all categories...");
      var subscribersToAllCategories = IdentifySubscribersToAllCategories(subscribers, magazines, categories);
      var subscriberNamesToAllCategories = from s in subscribersToAllCategories select $"{s.FirstName} {s.LastName}";
      Console.WriteLine($"Identified {subscribersToAllCategories.Count} subscribers to all categories: " + string.Join(", ", subscriberNamesToAllCategories));

      Console.WriteLine();
      Console.Write("Submitting answer...");
      var response = SubmitAnswer(subscribersToAllCategories);
      Console.WriteLine("Done!");

      Console.WriteLine();
      Console.WriteLine("Answer response:");
      Console.WriteLine($"  Total time: {response.TotalTime}");
      Console.WriteLine($"  Answer correct: {response.AnswerCorrect}");
      Console.WriteLine($"  Should be: {response.ShouldBe}");

      Console.WriteLine();
      Console.WriteLine("Press [Enter] to continue...");
      Console.ReadLine();
    }

    private static async Task<List<Magazine>> FetchMagazines(List<string> categories) {
      var getMagazinesTasks = categories.Select(c => client.GetMagazinesAsync(c));
      var magazinesByCategory = await Task.WhenAll(getMagazinesTasks);
      var magazines = magazinesByCategory.SelectMany(m => m).ToList();
      return magazines;
    }

    private static List<Subscriber> IdentifySubscribersToAllCategories(List<Subscriber> subscribers, List<Magazine> magazines, List<string> categories) {
      var subscribersToAllCategories = new List<Subscriber>();

      var categoriesSet = new HashSet<string>(categories);
    
      foreach (var s in subscribers) {
        var subscribedCategories = from m in magazines join mid in s.MagazineIds on m.ID equals mid select m.Category;
        var subscribedCategoriesSet = new HashSet<string>(subscribedCategories);

        if (categoriesSet.IsSubsetOf(subscribedCategoriesSet)) {
          subscribersToAllCategories.Add(s);
        }
      }

      return subscribersToAllCategories;
    }

    private static AnswerResponse SubmitAnswer(List<Subscriber> subscribersToAllCategories) {
      var subscriberIdsToAllCategories = from s in subscribersToAllCategories select s.ID;
      var answer = new Answer()
      {
        Subscribers = subscriberIdsToAllCategories.ToList<string>()
      };

      return client.PostAnswer(answer);
    }
  }
}
