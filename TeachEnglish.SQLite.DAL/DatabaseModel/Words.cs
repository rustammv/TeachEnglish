namespace TeachEnglish.SQLite.DAL.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Words
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Words()
        {
            AudioFiles = new HashSet<AudioFiles>();
            Translations = new HashSet<Translations>();
        }

        [Key]
        public long IDWord { get; set; }

        [Required]
        [StringLength(50)]
        public string Word { get; set; }

        [Required]
        [StringLength(50)]
        public string Transcription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AudioFiles> AudioFiles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Translations> Translations { get; set; }
    }
}
