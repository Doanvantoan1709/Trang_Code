﻿using BaseProject.Service.Common;
using BaseProject.Service.Dto;

namespace BaseProject.Service.OperationService.Dto
{
    public class OperationSearch : SearchBase
    {
        public Guid? ModuleId {get; set; }
		public string? Name {get; set; }
		public string? URL {get; set; }
		public string? Code {get; set; }
		public bool? IsShow {get; set; }
    }
}
