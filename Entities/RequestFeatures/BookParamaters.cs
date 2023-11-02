﻿namespace Entities.RequestFeatures
{
    public class BookParamaters : RequestParameters
	{
        public uint MinPrice { get; set; }
        public uint MaxPrice { get; set; } = 1000;
        public bool ValidPriceRange => MaxPrice > MinPrice;
        public String? SearchTerm { get; set; }
        public BookParamaters()
        {
            OrderBy = "id";
        }
    }
}
