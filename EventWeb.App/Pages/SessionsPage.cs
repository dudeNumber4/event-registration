using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Common;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor;

namespace EventWeb.App.Pages
{

	/// <summary>
	/// Note that you can use this code-behind style or jimmy all your code into the markup.
	/// </summary>
	public class SessionsPage : BlazorComponent
	{

		protected List<Session> _sessions;

		[Inject]
		protected HttpClient Http { get; set; }

		protected override async Task OnInitAsync()
		{
			_sessions = await Http.GetJsonAsync<List<Session>>("api/Session/GetAllSessions");
		}

	}

}
