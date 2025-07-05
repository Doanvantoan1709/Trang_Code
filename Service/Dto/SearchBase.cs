﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.Dto
{
    public class SearchBase
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string SortColumn { get; set; } = "CreatedDate";
        public string SortOrder {get;set;} = "desc";

    }
}
