using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.Models.Sgcd
{
    public class ImportDocumentTypes
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Format { get; set; }
        [Required]
        public int ExternalApplicationId { get; set; }
    }
}
