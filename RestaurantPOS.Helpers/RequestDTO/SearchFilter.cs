using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class SearchFilter
    {
        public string SearchText { get; set; }
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public SortFilter sortFilter { get; set; }
        public List<DatabaseFilter> Filters { get; set; }
        [JsonIgnore]
        public string BasePath { get; set; }
    }
    public class SortFilter
    {
        public string SortBy { get; set; }
        public string Direction { get; set; }
    }
    public class DatabaseFilter
    {
        public string Name { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Logic { get; set; }
    }
}
