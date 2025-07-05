using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.FileManagerService.ViewModels
{
    public class FileManagerCopyVM
    {
        public List<FileManager> SourceItems { get; set; }
        public FileManager? DestinationFolder { get; set; }
    }
}
