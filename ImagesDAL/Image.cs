namespace ImagesDal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblImage")]
    public partial class Image
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Image()
        {
            Tags = new HashSet<Tag>();
        }

        [Key]
        [Column("intId")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nvcImageName")]
        public string ImageName { get; set; }

        [Required]
        [Column("vbImageContent")]
        public byte[] ImageContent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
