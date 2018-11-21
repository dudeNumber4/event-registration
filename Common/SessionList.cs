using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Common
{

	public class SessionList: List<Session>
	{

		/// <summary>
		/// Disallow multiple sessions with the same Id.
		/// </summary>
		/// <param name="session"></param>
		public new void Add(Session session)
		{
			if (this.Any(s => s.Id == session.Id))
			{
				throw new InvalidOperationException($"{nameof(SessionList)} already contains a session with the given Id.");
			}
			else
			{
				base.Add(session);
			}
		}

		public List<Session> GetSuggestedSessions()
		{
			var result = new List<Session>();
			if (Count == 1)
			{
				result.Add(this[0]);
			}
			else if (Count > 1)
			{
				var random = new Random();
				int randomCutoffIndex = random.Next(0, Count - 1);
				for (int i = 0; i <= randomCutoffIndex; i++)
				{
					int nextRandomIndex = random.Next(randomCutoffIndex);
					Session nextRandomSession = this[nextRandomIndex];
					if (!result.Contains(nextRandomSession))
					{
						result.Add(nextRandomSession);
					}
				}
			}
			return result;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = 17;
				this.ForEach(s => result = result * 23 + s.Id);
				return result;
			}
		}

	}

}
