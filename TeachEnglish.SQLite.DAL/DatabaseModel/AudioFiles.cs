namespace TeachEnglish.SQLite.DAL.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AudioFiles
    {
        [Key]
        public long IDAudio { get; set; }

        [Required]
        [MaxLength(2147483647)]
        public byte[] AudioFile { get; set; }

        public long IDWord { get; set; }

        public virtual Words Words { get; set; }
    }
}
