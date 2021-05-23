using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EventModels
{

    public record Personal
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record">The portions representing personal info returned when calling EventRepository.GetRecord for Registrant type</param>
        /// <returns></returns>
        public static Personal FromBasicRecord(IEnumerable<string> record)
        {
            if (record?.Count() == 3)
            {
                return new Personal
                {
                    FirstName = record.ElementAt(0),
                    LastName = record.ElementAt(1),
                    Email = record.ElementAt(2)
                };
            }
            else
            {
                return null;
            }
        }
    }

}
