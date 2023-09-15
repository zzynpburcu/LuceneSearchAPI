using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // JObject kullanmak için ekleyin
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;


class Program
{
    static async Task Main()
    {
        // JSON verisini URL'den al
        string jsonUrl = "https://www.vizgr.org/historical-events/search.php?format=json&begin_date=-3000000&end_date=20151231&lang=en"; // JSON dosyasının URL'sini buraya ekleyin
        string json = await FetchJsonData(jsonUrl);

        if (!string.IsNullOrEmpty(json))
        {
            // Lucene endeksi oluştur
            string indexPath = "../LuceneIndex"; // Lucene endeks dosyasının yolu
            FSDirectory directory = FSDirectory.Open(indexPath);
            IndexWriterConfig config = new IndexWriterConfig(LuceneVersion.LUCENE_48, new StandardAnalyzer(LuceneVersion.LUCENE_48));
            IndexWriter writer = new IndexWriter(directory, config);

            // JSON verisini işle ve Lucene endeksine yaz
            ProcessAndIndexJsonData(json, writer);

            // Endekslemeyi tamamla ve kaynakları serbest bırak
            writer.Flush(triggerMerge: true, applyAllDeletes: false);
            writer.Commit();
            writer.Dispose();
            directory.Dispose();
        }
        else
        {
            Console.WriteLine("JSON verisi alınamadı.");
        }
        PrintFirst10RecordsFromIndex();
    }

    static async Task<string> FetchJsonData(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON verisi alınamadı: {ex.Message}");
                return null;
            }
        }
    }

    static void ProcessAndIndexJsonData(string json, IndexWriter writer)
    {

        string[] eventItems = json.Split(new string[] { "\"event\"" }, StringSplitOptions.RemoveEmptyEntries);
       
        // Toplanmış veriyi saklayacak bir StringBuilder oluşturun
        StringBuilder combinedData = new StringBuilder();

        combinedData.Append("{ \"events\": [");
        int countItems = eventItems.Length - 1;
        Console.WriteLine(countItems);
        // Ayırılan öğeleri dolaşın ve toplayın
        for (int i = 1; i < eventItems.Length; i++)
        {
            // Her bir item'ı uygun biçimde getirin ve toplam veriye ekleyin
            string formattedItem = eventItems[i].Trim().Substring(2, eventItems[i].Length - 4);
            combinedData.Append(formattedItem);
            
            // Son öğe değilse virgül ekleyin
            if (i < eventItems.Length - 1)
            {
                combinedData.Append(", ");
            }
        }

        combinedData.Append("] }");

        //  JSON veriyi C# nesnelerine dönüştürün (yukarıdaki örnekte olduğu gibi)
        RootObject root = JsonConvert.DeserializeObject<RootObject>(combinedData.ToString());
        List<Event> events = root.events;
        int k = 0;
        
        if (events != null)
        {
            foreach (var evt in events)
            {
                if (evt != null)
                {
                    k++;
                    //Console.WriteLine(k);
                    Document doc = new Document();
                    Console.WriteLine(k);
                    doc.Add(new StringField("date", evt.date ?? string.Empty, Field.Store.YES)); // Null ise boş bir string olarak saklar
                    doc.Add(new TextField("description", evt.description ?? string.Empty, Field.Store.YES)); // Null ise boş bir string olarak saklar
                    doc.Add(new StringField("lang", evt.lang ?? string.Empty, Field.Store.YES)); // Null ise boş bir string olarak saklar
                    doc.Add(new StringField("category1", evt.category1 ?? string.Empty, Field.Store.YES)); // Null ise boş bir string olarak saklar
                    doc.Add(new StringField("category2", evt.category2 ?? string.Empty, Field.Store.YES)); // Null ise boş bir string olarak saklar
                    doc.Add(new StringField("granularity", evt.granularity ?? string.Empty, Field.Store.YES)); // Null ise boş bir string olarak saklar

                    writer.AddDocument(doc);
                }
             else
            {
                // Event öğesi boşsa burada uygun bir işlemi gerçekleştirebilirsiniz
                Console.WriteLine("Boş bir Event öğesi tespit edildi, atlanacak.");
            }}
        }

    }
    static void PrintFirst10RecordsFromIndex()
    {
        string indexPath = "../LuceneIndex"; // Lucene endeks dosyasının yolu
        FSDirectory directory = FSDirectory.Open(indexPath);

        // Lucene endeksini aç
        using (var reader = DirectoryReader.Open(directory))
        {
            int numDocs = reader.NumDocs;
            int maxDocsToRetrieve = Math.Min(4, numDocs); // İlk 4 dokümanı al

            Console.WriteLine($"Toplam {numDocs} doküman bulundu. İlk {maxDocsToRetrieve} doküman:");

            for (int i = 0; i < maxDocsToRetrieve; i++)
            {
                Document doc = reader.Document(i);
                Console.WriteLine($"Doküman {i + 1}");
                Console.WriteLine($"Description: {doc.Get("description")}");
                Console.WriteLine($"Date: {doc.Get("date")}");
                Console.WriteLine($"Lang: {doc.Get("lang")}");
                Console.WriteLine($"Category1: {doc.Get("category1")}");
                Console.WriteLine($"Category2: {doc.Get("category2")}");
                Console.WriteLine($"Granularity: {doc.Get("granularity")}");
                Console.WriteLine();
            }
        }

        directory.Dispose();
    }
}
public class Event
{
    public string date { get; set; }
    public string description { get; set; }
    public string lang { get; set; }
    public string category1 { get; set; }
    public string category2 { get; set; }
    public string granularity { get; set; }
}

public class RootObject
{
    public List<Event> events { get; set; }
}
