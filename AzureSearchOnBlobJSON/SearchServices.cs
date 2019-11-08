using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace AzureSearchOnBlobJSON
{
    class SearchServices
    {
        readonly string SEARCHSERVICE_NAME = ""; //Search Service Name
        readonly string SEARCHSERVICE_KEY = ""; //Admin Api Key for Search Service
        readonly string INDEX_NAME = ""; //Custom Name

        readonly string DATASOURCE_NAME = ""; //Custom Name
        readonly string STORAGECONTAINER_NAME = ""; //Blob Container Name
        readonly string FOLDER_NAME = ""; //Folder inside blob container

        static string STORAGEACCOUNT_NAME = ""; //Storage Account Name
        static string STORAGEACCOUNT_KEY = ""; //StorageAccountKey

        //Connection String to Storage Account
        readonly string DATASOURCE_CONNECTIONSTRING = $"DefaultEndpointsProtocol=https;AccountName={STORAGEACCOUNT_NAME};AccountKey={STORAGEACCOUNT_KEY}";

        static string INDEXER_NAME = ""; //Custom Name

        /// <summary>
        /// Creates Index, Data Source and Indexer for search service
        /// </summary>
        public bool InitializeSearchServiceDependency()
        {
            try
            {
                ISearchServiceClient searchClient = CreateSearchServiceClient();
                CreateSearchIndex(searchClient);
                CreateDataSource(searchClient);
                CreateIndexer(searchClient);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// Creates search parametre to run the search query upon
        /// </summary>
        /// <param name="selectQuery"></param>
        /// <param name="orderbyQuery"></param>
        /// <param name="SearchQuery"></param>
        /// <returns></returns>
        public DocumentSearchResult<Document> RunQuery(string[] selectQuery,string[] orderbyQuery, string SearchQuery)
        {
            ISearchIndexClient searchIndexClient = CreateSearchIndexClient();
            SearchParameters searchParam = new SearchParameters
            {
                Select = selectQuery,
                OrderBy = orderbyQuery,
            };
            return SearchUponQuery(searchIndexClient, searchParam,SearchQuery);
        }
        public DocumentSearchResult<Document> RunQuery(string[] selectQuery, string[] orderbyQuery, string filterQuery, string SearchQuery)
        {
            ISearchIndexClient searchIndexClient = CreateSearchIndexClient();
            SearchParameters searchParam = new SearchParameters
            {
                Select = selectQuery,
                OrderBy = orderbyQuery,
                Filter = filterQuery
            };
            return SearchUponQuery(searchIndexClient, searchParam, SearchQuery);
        }
        /// <summary>
        /// Creates new SearchIndex with INDEX_NAME provided, if already exists no change happen
        /// </summary>
        /// <param name="searchClient"></param>
        private void CreateSearchIndex(ISearchServiceClient searchClient)
        {
            if (searchClient.Indexes.Exists(INDEX_NAME))
                return;
            var definition = new Index()
            {
                Name = INDEX_NAME,
                Fields = FieldBuilder.BuildForType<SampleEntity>()
            };
            searchClient.Indexes.Create(definition);
        }

        /// <summary>
        /// Creates new DataSource with DATASOURCE_NAME provided, if already exists no change happen
        /// </summary>
        /// <param name="searchClient"></param>
        private void CreateDataSource(ISearchServiceClient searchClient)
        {
            if (searchClient.DataSources.Exists(DATASOURCE_NAME))
                return;

            var dataSourceConfig = new DataSource()
            {
                Name = DATASOURCE_NAME,
                Container = new DataContainer(STORAGECONTAINER_NAME, FOLDER_NAME),
                Credentials = new DataSourceCredentials(DATASOURCE_CONNECTIONSTRING),
                Type = DataSourceType.AzureBlob
            };

            searchClient.DataSources.Create(dataSourceConfig);
        }

        /// <summary>
        /// Creates new Indexer with INDEXER_NAME provided, if already exists no change happen
        /// </summary>
        /// <param name="searchClient"></param>
        private void CreateIndexer(ISearchServiceClient searchClient)
        {
            if (searchClient.Indexers.Exists(INDEXER_NAME))
                return;
            var parseConfig = new Dictionary<string, object>(); //Can add other configurations like RootOfDocument
            parseConfig.Add("parsingMode", "jsonArray");
            
            var indexerConfig = new Indexer()
            {
                Name = INDEXER_NAME,
                DataSourceName = DATASOURCE_NAME,
                TargetIndexName = INDEX_NAME,
                Parameters = new IndexingParameters()
                {
                    Configuration = parseConfig
                }
            };
            searchClient.Indexers.Create(indexerConfig);
        }

        /// <summary>
        /// Helper method for Querying in Search Service
        /// </summary>
        /// <param name="searchIndexClient"></param>
        /// <param name="searchParameters"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        private DocumentSearchResult<Document> SearchUponQuery(ISearchIndexClient searchIndexClient,SearchParameters searchParameters,string searchQuery)
        {
            try
            {
                var result = searchIndexClient.Documents.Search(searchQuery, searchParameters);
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            
        }

        private SearchServiceClient CreateSearchServiceClient()
        {
            SearchServiceClient serviceClient = new SearchServiceClient(SEARCHSERVICE_NAME, new SearchCredentials(SEARCHSERVICE_KEY));
            return serviceClient;

        }

        private SearchIndexClient CreateSearchIndexClient()
        {

            SearchIndexClient searchIndexClient = new SearchIndexClient(SEARCHSERVICE_NAME,INDEX_NAME, new SearchCredentials(SEARCHSERVICE_KEY));
            return searchIndexClient;

        }
    }

    
}
