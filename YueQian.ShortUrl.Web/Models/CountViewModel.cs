﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models.Enums;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Models
{
    public class CountViewModel : ViewModelListBase
    {
        public CountViewModel(string keyword, DateTime? startDate, DateTime? endDate,
            int pageIndex = 1, int pageSize = 20)
        {
            var totalItemCount = 0;
            var queries = new List<IMongoQuery>();
            queries.Add(Query.EQ("UserId", CurrentUserId));
            if (startDate.HasValue)
                queries.Add(Query.GTE("CreationDate", startDate.Value));
            if (endDate.HasValue)
                queries.Add(Query.LT("CreationDate", endDate.Value.AddDays(1)));

            var source = MongoHelper.Instance.PagedFind<Integral>(queries.Count > 0 ? Query.And(queries.ToArray()) : Query.Null, out totalItemCount, pageIndex: pageIndex, pageSize: pageSize, sortby: SortBy.Descending("CreationDate"));

            List = new ListData(source.Select(s => new ListDataItem
            {
                Id = s.Id,
                Title = string.Format("{0}{1}", s.IntegralType.ToString(), s.IntegralType == IntegralType.有效访问 ? "(yqurl.com/" + s.ShortUrl + ")" : ""),
                Url = s.ShortUrl,
                DateTimeString = s.CreationDate.ToString("yyyy年MM月dd日")
            }), pageIndex, pageSize, totalItemCount);

            SearchModel = new SearchModel
            {
                Keyword = keyword,
                StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "",
                EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : ""
            };
        }
    }
}
