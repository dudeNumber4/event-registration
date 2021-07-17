using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EventModels
{

    public record Registration : IEventRecord
    {

        public int Id { get; set; }
        public int RegistrationId { get; set; }

        /// <summary>
        /// I don't know wtf this is for now.
        /// </summary>
        public SessionList SessionList { get; set; } = new SessionList(new List<Session>());

        public List<int> SessionIds { get; set; }

        /// <summary>
        /// When loaded from storage, we will only have session ids, not a fully populated SessionList.
        /// </summary>
        public IEventRecord FromBasicRecord(IEnumerable<string> record)
        {
            var recordList = record.ToList();
            if ((recordList?.Count >= 2) && int.TryParse(recordList[0], out var id) && int.TryParse(recordList[1], out var registrationId))
            {
                var result = new Registration
                {
                    Id = id,
                    RegistrationId = registrationId,
                    SessionIds = record.TakeLast(recordList.Count - 2).Select(i =>
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

        public IEnumerable<string> ToBasicRecord()
        {
            yield return RegistrationId.ToString();
            foreach (var id in SessionIds)
            {
                yield return id.ToString();
            }
        }

    }

}
