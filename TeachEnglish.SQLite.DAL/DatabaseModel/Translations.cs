namespace TeachEnglish.SQLite.DAL.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Translations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Translations()
        {
            ImageFiles = new HashSet<ImageFiles>();
        }

        [Key]
        public long IDTranslation { get; set; }

        [Required]
        [StringLength(50)]
        public string Translation { get; set; }

        public long IDWord { get; set; }

        public long? IDPosRu { get; set; }

        public long? IDPosEn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageFiles> ImageFiles { get; set; }

        public virtual PartsOfSpeechEn PartsOfSpeechEn { get; set; }

        public virtual PartsOfSpeechRu PartsOfSpeechRu { get; set; }

        public virtual Words Words { get; set; }
    }
}
