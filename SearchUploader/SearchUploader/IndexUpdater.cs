using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using SearchUploader.Model;
using System.Text;
using  Newtonsoft.Json;


namespace SearchUploader
{
    public static class IndexUpdater
    {
        [FunctionName("IndexUpdater")]
        public static async Task Run([BlobTrigger("properties/{name}", Connection = "StorageConnection")]Stream myBlob, string name, TraceWriter log)
        {
            var searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            var adminApiKey = ConfigurationManager.AppSettings["SearchServiceAdminApiKey"];
            var serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

            var blogPost = GetBlogPost(myBlob);
            blogPost.filePath = $"{ConfigurationManager.AppSettings["FilePathBeginning"]}{name}";

            var action = new[]
            {
                IndexAction.Upload(blogPost)
            };

            var batch = IndexBatch.New(action);
            var indexClient = serviceClient.Indexes.GetClient(ConfigurationManager.AppSettings["SearchIndexName"]);

            try
            {
                await indexClient.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException e)
            {
                log.Error("Error: ", e);
                throw;
            }
        }

        public static BlogPost GetBlogPost(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);
            var data = Encoding.ASCII.GetString(bytes);
            var blog = JsonConvert.DeserializeObject<BlogPost>(data);
            return blog;
        }
    }
}
