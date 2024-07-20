using System.ComponentModel.DataAnnotations;
namespace RingoMediaTask.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
    }
}