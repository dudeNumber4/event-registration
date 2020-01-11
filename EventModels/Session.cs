using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EventModels
{

    public class Session : IEventRecord
    {

        public int Id { get; set; }
        
        public DayOfWeek Day { get; set; }
        
        [Required(ErrorMessage = "A sesssion needs a title.")]
        [StringLength(50, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }
        
        [StringLength(1024, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }

        public IEventRecord FromBasicRecord(IEnumerable<string> record)
        {
            var recordList = record.ToList();
            if ((recordList?.Count == 4) && int.TryParse(recordList[0], out var id) && int.TryParse(recordList[1], out var d))
            {
                var result = new Session
                {
                    Id = id,
                    Day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), recordList[1], true),
                    Title = recordList[2],
                    Description = recordList[3]
                };
                return result;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<string> ToBasicRecord()
        {
            yield return ((int)Day).ToString();
            yield return Title;
            yield return Description;
        }

        public override int GetHashCode()
        {
            return Id;
        }

    }

}
