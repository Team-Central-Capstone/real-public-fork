using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Real.Model.InformationSchema {

    [Table("Capstone_Information_Schema_Tables")]
    [Keyless]
    public class Table {

        public override string ToString() { return $"{TABLE_SCHEMA}.{TABLE_NAME}"; }
        
        [StringLength(64)]
        public string TABLE_CATALOG { get; set; }
        
        [StringLength(64)]
        public string TABLE_SCHEMA { get; set; }
        
        [StringLength(64)]
        public string TABLE_NAME { get; set; }
        
        [StringLength(11)]
        public string TABLE_TYPE { get; set; }
        
        [StringLength(64)]
        public string ENGINE { get; set; }
        
        
        public int? VERSION { get; set; }
        
        [StringLength(10)]
        public string ROW_FORMAT { get; set; }
        
        
        public Int64? TABLE_ROWS { get; set; }
        
        
        public Int64? AVG_ROW_LENGTH { get; set; }
        
        
        public Int64? DATA_LENGTH { get; set; }
        
        
        public Int64? MAX_DATA_LENGTH { get; set; }
        
        
        public Int64? INDEX_LENGTH { get; set; }
        
        
        public Int64? DATA_FREE { get; set; }
        
        
        public Int64? AUTO_INCREMENT { get; set; }
        
        
        public DateTime CREATE_TIME { get; set; }
        
        
        public DateTime? UPDATE_TIME { get; set; }
        
        
        public DateTime? CHECK_TIME { get; set; }
        
        [StringLength(64)]
        public string TABLE_COLLATION { get; set; }
        
        
        public Int64? CHECKSUM { get; set; }
        
        [StringLength(256)]
        public string CREATE_OPTIONS { get; set; }
        
        [StringLength(65535)]
        public string TABLE_COMMENT { get; set; }


        public virtual List<Column> Columns { get; set; } = new List<Column>();
    }   
}