namespace TeachEnglish.SQLite.DAL.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PartsOfSpeechRu")]
    public partial class PartsOfSpeechRu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartsOfSpeechRu()
        {
            Translations = new HashSet<Translations>();
        }

        [Key]
        public long IDPosRu { get; set; }

        [Required]
        [StringLength(50)]
        public string PosRu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Translations> Translations { get; set; }
    }
}
