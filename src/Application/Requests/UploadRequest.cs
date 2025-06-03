using CleanArchitecture.Application.Enums;

namespace CleanArchitecture.Application.Requests
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }

        //only required for document upload
        public int VersionNumber { get; set; }
        public string DocumentType { get; set; }
        public string ExternalApplication { get; set; }
    }
}