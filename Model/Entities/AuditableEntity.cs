﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{

    public interface IAuditableEntity
    {
        DateTime CreatedDate { get; set; }
        string? CreatedBy { get; set; }
        Guid? CreatedId { get; set; }
        DateTime UpdatedDate { get; set; }
        Guid? UpdatedId { get; set; }
        string? UpdatedBy { get; set; }
        bool? IsDelete { get; set; }
        DateTime? DeleteTime { get; set; }
        Guid? DeleteId { get; set; }
    }

    public class AuditableEntity : Entity, IAuditableEntity
    {
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        [BsonRepresentation(BsonType.String)]
        public Guid? CreatedId { get; set; }
        public DateTime UpdatedDate { get; set; }
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
        [JsonIgnore]
        [BsonRepresentation(BsonType.String)]
        public Guid? UpdatedId { get; set; }
        [JsonIgnore]
        public bool? IsDelete { get; set; }
        [JsonIgnore]
        public DateTime? DeleteTime { get; set; }
        [JsonIgnore]
        [BsonRepresentation(BsonType.String)]
        public Guid? DeleteId { get; set; }
    }
}
