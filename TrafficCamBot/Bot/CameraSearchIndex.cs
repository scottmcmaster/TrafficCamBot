using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using System.Text;
using System.Linq;
using log4net;

namespace TrafficCamBot.Bot
{
    /// <summary>
    /// Holds a full-text search index of the available camera names and provides a common
    /// searching API on top of it.
    /// </summary>
    public class CameraSearchIndex : IDisposable
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(CameraSearchIndex));

        private const Lucene.Net.Util.Version LUCENE_VERSION = Lucene.Net.Util.Version.LUCENE_30;

        /// <summary>
        /// Maximum number of results that can come back from a search method.
        /// </summary>
        private const int MAX_SEARCH_RESULTS = 10;

        /// <summary>
        /// The default value for the hit score.
        /// </summary>
        private const double DEFAULT_HIT_SCORE = 0.25;

        /// <summary>
        /// The name of the content field in the generated search doc. Contains various ways of
        /// spelling and phrasing a given camera name.
        /// </summary>
        private const string CONTENT_FIELD = "content";

        /// <summary>
        /// The exact name of the camera in a search doc. Not indexed.
        /// </summary>
        private const string CAMERA_NAME_FIELD = "cameraName";

        private readonly Analyzer analyzer = new StandardAnalyzer(LUCENE_VERSION);
        private RAMDirectory index;
        private Searcher searcher;

        /// <summary>
        /// Generates a document for each given camera name and adds it to the search index.
        /// After this method is called, the searcher is initialized and ready to use.
        /// </summary>
        /// <param name="cameraNames">List of camera names to generate search docs for.</param>
        public CameraSearchIndex(List<string> cameraNames)
        {
            index = new RAMDirectory();
            var altNameGenerator = new AlternateNameGenerator();
            using (IndexWriter writer = new IndexWriter(index, analyzer, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (string title in cameraNames)
                {
                    writer.AddDocument(CreateDocument(title, altNameGenerator.GenerateAlternateCameraNames(title)));
                }

                writer.Optimize();
            }
            searcher = new IndexSearcher(index);
        }

        /// <summary>
        /// The minimum hit threshold when doing phrase search. Anything less will not be returned
        /// in the result list.
        /// </summary>
        public double HitScore { get; set; } = DEFAULT_HIT_SCORE;

        private void CheckNotDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException("CameraSearchIndex");
            }
        }

        private Document CreateDocument(string title, IEnumerable<string> altNames)
        {
            var doc = new Document();
            doc.Add(new Field(CAMERA_NAME_FIELD, title, Field.Store.YES, Field.Index.NOT_ANALYZED));
            var sb = new StringBuilder();
            sb.Append(title);
            sb.Append('\n');
            foreach (var altName in altNames)
            {
                sb.Append(altName);
                sb.Append('\n');
            }
            var content = sb.ToString();
            doc.Add(new Field(CONTENT_FIELD, content, Field.Store.YES, Field.Index.ANALYZED));
            return doc;
        }

        /// <summary>
        /// Searches for the given text. Tries the different search approaches against the index
        /// in the following order:
        /// 1. Exact match.
        /// 2. Fuzzy match (to catch simple misspellings).
        /// 3. Phrase search (which looks for plausible results based on score).
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <returns>A list of one or more results in no particular order.</returns>
        public IList<string> Search(string text)
        {
            CheckNotDisposed();

            var result = SearchExact(text);
            if (result.Count == 0)
            {
                result = SearchFuzzy(text);
            }
            if (result.Count == 0)
            {
                result = SearchPhrase(text);
            }
            return result;
        }

        private IList<string> SearchExact(string text)
        {
            var query = new BooleanQuery();
            foreach (string term in text.Split(' '))
            {
                var tq = new TermQuery(new Term(CONTENT_FIELD, term.ToLower()));
                query.Add(tq, Occur.MUST);
            }

            return RunQuery(query);
        }

        private IList<string> SearchFuzzy(string text)
        {
            var term = new Term(CONTENT_FIELD, text);
            var query = new FuzzyQuery(term);
            return RunQuery(query);
        }

        private IList<string> SearchPhrase(string text)
        {
            var query = new QueryParser(LUCENE_VERSION, CONTENT_FIELD, analyzer).Parse(text);
            return RunQuery(query);
        }

        private IList<string> RunQuery(Query query)
        {
            var collector = TopScoreDocCollector.Create(MAX_SEARCH_RESULTS, false);
            searcher.Search(query, collector);
            var scoreDocs = collector.TopDocs().ScoreDocs;
            logger.Debug("Searching for " + query +
                ", top score is " + (scoreDocs.Any() ? scoreDocs[0].Score.ToString() : "non-existent"));
            var results = from hit in scoreDocs
                          where hit.Score >= HitScore
                          select searcher.Doc(hit.Doc).Get(CAMERA_NAME_FIELD);
            return results.ToList();
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    searcher.Dispose();
                }
                index = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}