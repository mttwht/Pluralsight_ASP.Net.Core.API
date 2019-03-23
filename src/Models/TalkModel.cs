using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Models
{
    public class TalkModel
    {
        public int TalkId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [StringLength(4000, MinimumLength = 20)]
        public string Abstract { get; set; }
        [Range(100, 400)]
        public int Level { get; set; }

        public SpeakerModel Speaker { get; set; }
    }
}
