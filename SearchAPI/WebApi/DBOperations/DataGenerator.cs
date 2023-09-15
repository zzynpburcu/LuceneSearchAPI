using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Entities;

namespace WebApi.DBOperations
{
    public class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new EventDbContext(serviceProvider.GetRequiredService<DbContextOptions<EventDbContext>>()))
            {
                if (context.Events.Any())
                {
                    return;
                }
                string indexPath = "../../LuceneIndex"; // Lucene endeks dosyasının yolu
            FSDirectory directory = FSDirectory.Open(indexPath);
            // Lucene endeksini aç
            using (var reader = DirectoryReader.Open(directory))
            {
                int numDocs = reader.NumDocs;
                int maxDocsToRetrieve = Math.Min(10, numDocs); // İlk 10 dokümanı al

                Console.WriteLine($"Toplam {numDocs} doküman bulundu. ");
                List<Event> eventsToAdd = new List<Event>(); // Event nesnelerini saklamak için bir liste oluşturun


                for (int i = 0; i < numDocs; i++)
                {
                    Document doc = reader.Document(i);
                    // Dökümanları işleyin ve Event nesnelerini oluşturun
                    Event newEvent = new Event
                    {
                        date = doc.Get("date"),
                        description = doc.Get("description"),
                        lang = doc.Get("lang"),
                        category1 = doc.Get("category1"),
                        category2 = doc.Get("category2"),
                        granularity = doc.Get("granularity")
                    };
                    eventsToAdd.Add(newEvent); // Oluşturulan Event nesnesini listeye ekleyin

                }
                  // Oluşturulan Event nesnelerini Entity Framework kullanarak veritabanına ekleyin
                context.Events.AddRange(eventsToAdd);
                context.SaveChanges();

            }

            directory.Dispose();
               

                
                    
            }
        }
    }
}