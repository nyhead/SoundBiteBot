using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InlineQueryResults;

namespace SoundBiteBot {
  class Program 
  {
          static ITelegramBotClient botClient;
          static SoundContext context = new SoundContext();

          static void Main() 
          {
            botClient = new TelegramBotClient("");

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnInlineQuery += Bot_OnInlineQuery;

            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();
          }

          static async void Bot_OnInlineQuery(object sender, InlineQueryEventArgs e)
          {
              Console.WriteLine($"Received inline query from: {e.InlineQuery.From.Id}: {e.InlineQuery.Query}"); 
              var results = await SearchByQuery(e.InlineQuery.Query);

              await botClient.AnswerInlineQueryAsync(
                  inlineQueryId: e.InlineQuery.Id,
                  results: results,
                  isPersonal: true,
                  cacheTime: 0
              );            
          }

          static async Task<List<InlineQueryResultBase>> SearchByQuery(string query)
          {
            List<InlineQueryResultBase> results = new List<InlineQueryResultBase>();

            var fileIdQuery = context.Posts.Select(x => x.FileId);
            var postIdQuery = context.Posts.Select( x=> x.PostId);

            if(!String.IsNullOrWhiteSpace(query))
            {            
                fileIdQuery = context.Posts.Where(x => x.Artist.Contains(query) || x.Title.Contains(query)).Select(x => x.FileId);
                postIdQuery = context.Posts.Where(x => x.Artist.Contains(query) || x.Title.Contains(query)).Select(x => x.PostId);              
            }

            using (var sequenceFileId = fileIdQuery.GetEnumerator())
              using(var sequencePostId = postIdQuery.GetEnumerator())
                while (sequenceFileId.MoveNext() && sequencePostId.MoveNext())
                {
                    results.Add(
                      //Alright boah
                      new InlineQueryResultCachedAudio(
                          id: sequencePostId.Current,
                          audioFileId: sequenceFileId.Current
                      )          
                    );               
                    
                }

            return results;
          }
    }
}
