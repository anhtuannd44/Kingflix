using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("BlogCategory")]
    public partial class BlogCategory
    {
        
        public BlogCategory()
        {
            Blog = new HashSet<Blog>();
        }

        [Key]
        public int BlogCategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(500)]
        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public BlogType Type { get; set; }

        public virtual ICollection<Blog> Blog { get; set; }
    }
}