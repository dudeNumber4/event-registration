using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EventModels
{

    public class EventBase
    {

        /// <summary>
        /// Bundle up any record type into a json object with dumb properties.
        /// Each specific type should have a converter to go back to that object from the result of this.
        /// </summary>
        /// <param name="record"></param>
        /// <returns>Any record type returned from IEventRepository</returns>
        public static JObject ToJson(List<string> record)
        {
            var result = new JObject();
            for (int i = 0; i < record.Count; i++)
            {
                result.Add($"s{i}", record[i]);
            }
            return result;
        }

    }

}
