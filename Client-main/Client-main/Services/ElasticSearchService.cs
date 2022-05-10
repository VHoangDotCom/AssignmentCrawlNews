using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClientNews.Models;
using Elasticsearch.Net;
using Nest;

namespace ClientNews.Services
{
    public class ElasticSearchService
    {
        private static ElasticClient searchClient;
        private static string IndexName = "quyet";
        private static string ElasticSearchServer = "http://localhost:9200";
        private static string DefaultIndexName = "quyet";
        private static string ElasticSearchUser = "elastic";
        private static string ElasticSearchPassword = "FDhMkMvuLi082UTlnQVjT1Jd";
        private static string CloudId = "quyet:dXMtY2VudHJhbDEuZ2NwLmNsb3VkLmVzLmlvJDc1MGNhY2IxZGJhNzQwYzZiOTA0ZDFhODRlY2M1N2RmJDM0YjZkZDYzYzdlMzRmNjFiMjE0Yzc2MDZkMWVkZGMw";

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