using Newtonsoft.Json;

namespace document.lib.shared.TableEntities
{
    public class DocLibDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("displayName")]
        public string DisplayName { get; set; } = "";

        [JsonProperty("physicalName")]
        public string PhysicalName { get; set; } = "";

        [JsonProperty("blobLocation")]
        public string BlobLocation { get; set; } = "";

        [JsonProperty("company")]
        public string Company { get; set; } = "";

        [JsonProperty("dateOfDocument")]
        public DateTimeOffset? DateOfDocument { get; set; }

        [JsonProperty("uploadDate")]
        public DateTimeOffset UploadDate { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTimeOffset LastUpdate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; } = "";
        [JsonProperty("folderId")] 
        public string FolderId { get; set; }

        [JsonProperty("folderName")]
        public string FolderName { get; set; } = "";

        [JsonProperty("registerName")]
        public string RegisterName { get; set; } = "";

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("unsorted")]
        public bool Unsorted { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; } = "";

        [JsonProperty("digital")]
        public bool DigitalOnly { get; set; } = false;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(DisplayName) ||
                string.IsNullOrWhiteSpace(Company) ||
                DateOfDocument == default ||
                string.IsNullOrWhiteSpace(Category) ||
                Tags == null ||
                !Tags.Any())
            {
                throw new Exception("Please make sure the following fields are filled ['DisplayName, Company, DateOfDocument, Category, Tags']");
            }
        }
    }
}
