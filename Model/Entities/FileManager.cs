using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    [Table("FileManager")]
    public class FileManager : AuditableEntity
    {
        public string Name { get; set; }
        public Guid? ParentId{ get; set; }
        public string? Path { get; set; }
        public long? Size{ get; set; }
        public bool? IsDirectory { get; set; }
        public string? FileExtension { get; set; }
        public string? MimeType { get; set; }
        public string? PhysicalPath { get; set; }
    }
}
