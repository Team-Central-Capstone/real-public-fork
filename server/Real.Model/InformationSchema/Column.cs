using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Real.Model.InformationSchema {

    [Table("Capstone_Information_Schema_Columns")]
    [Keyless]
    public class Column {
        
        public override string ToString() { return COLUMN_NAME; }

        [StringLength(64)]
        public string TABLE_CATALOG { get; set; }
        
        [StringLength(64)]
        public string TABLE_SCHEMA { get; set; }
        
        [StringLength(64)]
        public string TABLE_NAME { get; set; }
        
        [StringLength(64)]
        public string COLUMN_NAME { get; set; }
        
        
        public int ORDINAL_POSITION { get; set; }
        
        [StringLength(65535)]
        public string COLUMN_DEFAULT { get; set; }
        
        [StringLength(3)]
        public string IS_NULLABLE { get; set; }
        
        
        public string DATA_TYPE { get; set; }
        
        
        public Int64? CHARACTER_MAXIMUM_LENGTH { get; set; }
        
        
        public Int64? CHARACTER_OCTET_LENGTH { get; set; }
        
        
        public Int64? NUMERIC_PRECISION { get; set; }
        
        
        public Int64? NUMERIC_SCALE { get; set; }
        
        
        public int? DATETIME_PRECISION { get; set; }
        
        [StringLength(64)]
        public string CHARACTER_SET_NAME { get; set; }
        
        [StringLength(64)]
        public string COLLATION_NAME { get; set; }
        
        [StringLength(16777215)]
        public string COLUMN_TYPE { get; set; }
        
        [StringLength(3)]
        public string COLUMN_KEY { get; set; }
        
        [StringLength(256)]
        public string EXTRA { get; set; }
        
        [StringLength(154)]
        public string PRIVILEGES { get; set; }
        
        [StringLength(65535)]
        public string COLUMN_COMMENT { get; set; }
        

        public string GENERATION_EXPRESSION { get; set; }
        
        
        public int? SRS_ID { get; set; }

    }   
}