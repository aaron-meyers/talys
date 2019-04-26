﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GiantBombDataTool
{
    public interface IReadOnlyTableStore
    {
        //StoreMetadata CreateMetadata(StoreConfig config);
        IEnumerable<TableEntity> GetEntitiesByTimestamp(
            string table,
            TableConfig config,
            DateTime? lastTimestamp,
            long? lastId);
    }

    public interface ITableStore // TODO: derives IReadOnlyTableStore
    {
        object Location { get; } // TODO: move to IReadOnlyTableStore
    }
    
    public interface ITableMetadataStore
    {
        bool TryInitialize(string table, Metadata metadata);
        bool TryLoadMetadata(string table, out Metadata metadata);
        IEnumerable<string> GetTables();
    }

    public interface ITableStagingStore
    {
        bool TryLoadStagingMetadata(string table, out StagingMetadata metadata);
        void SaveStagingMetadata(string table, StagingMetadata metadata);
        string? WriteStagedEntities(string table, IEnumerable<TableEntity> entities);
    }

    public sealed class TableEntity
    {
        private const string IdProperty = "id";
        private const string TimestampProperty = "timestamp";

        public TableEntity(long id, DateTime timestamp, JObject content)
        {
            content[IdProperty] = id;
            content[TimestampProperty] = timestamp;
            Content = content;
        }

        public TableEntity(JObject content)
        {
            if (content[IdProperty] == null || content[TimestampProperty] == null)
                throw new ArgumentException("Missing id or timestamp in content.");

            Content = content;
        }

        // TODO: Consider shredding content into Dictionary<string, object> Properties
        // and using System.Text.Json.JsonElement for non-primitive property values
        // Would need to implement extension method for writing JsonElement to a Utf8JsonWriter

        public long Id => Content[IdProperty].Value<long>();
        public DateTime Timestamp => Content[TimestampProperty].Value<DateTime>();
        public JObject Content { get; }
    }
}
