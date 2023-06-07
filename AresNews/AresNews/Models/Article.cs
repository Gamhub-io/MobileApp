

using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AresNews.Models
{
    public class Article
    {
        [PrimaryKey, Column("_id"), JsonProperty("uuid")]
        public string Id { get; set; }
        [Column("mongoId"), JsonProperty("_id")]
        public string MongooseId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("textSnipet")]
        public string TextSnipet { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [ForeignKey(typeof(Source))]
        public int SourceIdFk { get; set; }
        [JsonProperty("sourceId")]
        public string SourceId { get; set; }
        //[ManyToOne(CascadeOperations = CascadeOperation.All), JsonProperty("source")]
        //public Source Source { get; set; }
        public Source Source { get
                {
                return App.Sources.FirstOrDefault(s => s.MongoId == SourceId);
            } }
        [JsonProperty("isoDate")]
        public DateTime FullPublishDate { get; set; }
        public string PublishDate
        {
            get
            {
                return FullPublishDate.ToString("dd/MM/yyyy");
            }
        }
        public string PublishTime
        {
            get
            {
                return FullPublishDate.ToLocalTime().ToString("HH:mm");
            }
        }
        public string Url { get; set; }
        public TimeSpan Time
        {
            get
            {
                return DateTime.Now - this.FullPublishDate.ToLocalTime();
            }
        }
        private bool? _isSaved = null;
        public bool IsSaved
        {
            get
            {
                if (_isSaved == null)
                    return App.SqLiteConn.Find<Article>(Id) != null;

                return (bool)_isSaved;
            }
            set { _isSaved = value; }
        }
    }
}