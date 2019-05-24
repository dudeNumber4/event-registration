using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EventModels
{

    /// <summary>
    /// Registration becomes itinerary upon selecting session list and posting.
    /// </summary>
    public class Itinerary : IEventRecord
    {

        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public SessionList SessionList { get; set; } = new SessionList();
        public List<int> SessionIds { get; set; }

        /// <summary>
        /// When loaded from storage, we will only have session ids, not a fully populated SessionList.
        /// </summary>
        public IEventRecord FromBasicRecord(List<string> record)
        {
            if ((record?.Count > 2) && int.TryParse(record[0], out var id) && int.TryParse(record[1], out var registrationId))
            {
                var result = new Itinerary
                {
                    Id = id,
                    RegistrationId = registrationId,
                    SessionIds = record.TakeLast(record.Count - 2).Select(i =>
                    {
                        int.TryParse(i, out var convertedId);
                        return convertedId;
                    }).ToList()
                };
                return result;
            }
            else
            {
                return null;
            }
        }

    }

}
