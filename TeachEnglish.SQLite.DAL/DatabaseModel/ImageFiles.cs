namespace TeachEnglish.SQLite.DAL.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ImageFiles
    {
        [Key]
        public long IDImage { get; set; }

        [Required]
        [MaxLength(2147483647)]
        public byte[] ImageFile { get; set; }

        public long IDTranslation { get; set; }

        public virtual Translations Translations { get; set; }
    }
}
