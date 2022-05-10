using CrawlNews.Models;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrawlNews.Services
{
    public class ElasticSearchService
    {
        private static ElasticClient searchClient;
        private static string IndexName = "articles";
        private static string ElasticSearchServer = "http://localhost:9200";
        private static string DefaultIndexName = "example-index";
        private static string ElasticSearchUser = "elastic";
        private static string ElasticSearchPassword = "8QAC1kklkbFkyVRR0ZxDh9tA";
        private static string CloudId = "CrawlNews:YXNpYS1zb3V0aGVhc3QxLmdjcC5lbGFzdGljLWNsb3VkLmNvbSQ4NTNmYzhhMjc4YmM0OWJhYTBmMTBhOTY0YWYyNTViMCQ4NWFmYjEwMTNjNGM0MmQ0ODJmMDZkMjUxZDQ3N2ViMQ==";

        public static ElasticClient GetInstance()
        {
            if (searchClient == null)
            {
                var settings = new ConnectionSettings(CloudId,
                  new BasicAuthenticationCredentials(ElasticSearchUser, ElasticSearchPassword))
                    .DefaultIndex(DefaultIndexName)
                    .DefaultMappingFor<Article>(i => i.IndexName(IndexName));
                searchClient = new ElasticClient(settings);
            }
            return searchClient;
        }
    }
}