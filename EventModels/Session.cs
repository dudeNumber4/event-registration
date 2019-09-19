using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventModels
{

    public class Session : IEventRecord
    {

        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }
        [StringLength(1024, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }

        public IEventRecord FromBasicRecord(List<string> record)
        {
            if ((record?.Count == 4) && int.TryParse(record[0], out var id) && int.TryParse(record[1], out var dayOfWeek))
            {
                var result = new Session
                {
                    Id = id,
                    Day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), record[1], true),
                    Title = record[2],
                    Description = record[3]
                };
                return result;
            }
            else
            {
                return null;
            }
        }

        public override int GetHashCode()
        {
            return Id;
        }

    }

}
