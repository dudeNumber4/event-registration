using System;
using System.Collections.Generic;
using System.Linq;

namespace EventModels
{

    public class Registrant : IEventRecord
    {

        public int Id { get; set; }
        public Personal PersonalInfo { get; set; }
        public Employment EmploymentInfo { get; set; }

        public IEventRecord FromBasicRecord(List<string> record)
        {
            if ((record?.Count == 6) && int.TryParse(record[0], out var id))
            {
                var result = new Registrant
                {
                    Id = id,
                    PersonalInfo = Personal.FromBasicRecord(record.Skip(1).Take(3)),
                    EmploymentInfo = Employment.FromBasicRecord(record.Skip(4).Take(2)),
                };
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Registrant becomes registration upon signature.
        /// </summary>
        /// <returns></returns>
        public Registration Sign()
        {
            return new Registration { RegistrantId = Id };
        }

    }

}
