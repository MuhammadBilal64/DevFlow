using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Common.Models
{
    public class PaginatedData<T>
    {
        public IReadOnlyList<T> Items { get; set; } =new List<T>();
        public int TotalCount {  get; set; }
    }
}
