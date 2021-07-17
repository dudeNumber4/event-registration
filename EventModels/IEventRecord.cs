using System;
using System.Collections.Generic;
using System.Text;

namespace EventModels
{
    public interface IEventRecord
    {
        int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record">Data returned from EventRepository for a given file type</param>
        /// <returns></returns>
        IEventRecord FromBasicRecord(IEnumerable<string> record);

        /// <summary>
        /// Note that this enumeration of strings doesn't include any id.  It's assumed this is a newly instantiated object that hasn't been assigned one yet.
        /// </summary>
        /// <returns>The goods to pass to the storage layer.</returns>
        IEnumerable<string> ToBasicRecord();
        
        /// <summary>
        /// Including Id.
        /// </summary>
        IEnumerable<string> ToFullRecord()
        {
            yield return Id.ToString();
            foreach (var s in ToBasicRecord())
                yield return s;
        }

    }
}
