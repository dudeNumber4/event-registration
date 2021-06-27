using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventModels
{

    /// <summary>
    /// Registrant becomes registration upon signature.
    /// </summary>
    public record Registration : IEventRecord
    {
        public int Id { get; set; }
        public int RegistrantId { get; set; }

        public IEventRecord FromBasicRecord(IEnumerable<string> record)
        {
            var recordList = record.ToList();
            if ((recordList?.Count == 2) && int.TryParse(recordList[0], out var id) && int.TryParse(recordList[1], out var registrantId))
            {
                var result = new Registration
                {
                    Id = id,
                    RegistrantId = registrantId
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
            yield return RegistrantId.ToString();
        }

    }

}
