using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// using MongoDB.Bson;
// using MongoDB.Bson.Serialization.Attributes;

namespace Real.Model {

    public interface IEntity {
        int Id { get; set; }
    }

    public abstract class EntityBase : IEntity
    {
        // [BsonId]
        // [BsonRepresentation(BsonType.ObjectId)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


    }
}
