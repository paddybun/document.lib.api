using System.IO;
using System.Security.Principal;

namespace document.lib.api.Dtos
{
    public class DocumentDownloadDto
    {
        public Stream Data { get; set; }
        public string ContentType { get; set; }
        public string Filename { get; set; }
    }
}