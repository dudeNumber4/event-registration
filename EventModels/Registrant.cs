using System;
using System.Collections.Generic;
using System.Linq;

namespace EventModels
{

    public record Registrant : IEventRecord
    {

        public int Id { get; set; }
        public Personal PersonalInfo { get; set; }
        public Employment EmploymentInfo { get; set; }

        public IEventRecord FromBasicRecord(IEnumerable<string> record)
        {
            var recordList = record.ToList();
            if ((recordList?.Count == 6) && int.TryParse(recordList[0], out var id))
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

        public IEnumerable<string> ToBasicRecord()
        {
            yield return PersonalInfo.FirstName;
            yield return PersonalInfo.LastName;
            yield return PersonalInfo.Email;
            yield return EmploymentInfo.OrgName;
            yield return EmploymentInfo.Industry;
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
