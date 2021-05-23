using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EventModels
{

    public record Employment
    {
        public string OrgName { get; set; }
        public string Industry { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record">The portions representing employment returned when calling EventRepository.GetRecord for Registrant type</param>
        /// <returns></returns>
        public static Employment FromBasicRecord(IEnumerable<string> record)
        {
            if (record?.Count() == 2)
            {
                return new Employment
                {
                    OrgName = record.ElementAt(0),
                    Industry = record.ElementAt(1)
                };
            }
            else
            {
                return null;
            }
        }

    }

}
