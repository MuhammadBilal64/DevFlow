using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Common.Models
{
    public class PaginationRequest
    {
       
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }

        public bool Descending { get; set; } = false;

    }
}
