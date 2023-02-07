using System;
using System.IO;
using System.Net.Http.Headers;
using Vue.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Vue.Services
{
	public class LoginMailPyme
	{

		public LoginMailPyme()
		{

		}
		public bool is_pyme(string email)
		{
			string[] words = email.Split('@');
			var is_pyme = false;
			if (words.Length > 1)
			{
				is_pyme = words[1] == "pymepharco.com" ? true : false;
			}
			return is_pyme;
		}
		public async Task<LoginResponse> login(string email, string password)
		{
			try
			{


				var client = new HttpClient();

				client.DefaultRequestHeaders.Accept
					.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
				var values = new Dictionary<string, string>
				{
					{ "user", email },
					{ "password", password }
				};
				var content = new FormUrlEncodedContent(values);
				var url = "https://mail.pymepharco.com/WorldClientAPI/authenticate/basic";
				var response = await client.PostAsync(url, content);
				if (response.IsSuccessStatusCode)
				{
					LoginResponse responseJson = await response.Content.ReadFromJsonAsync<LoginResponse>();
					return responseJson;
				}
				else
				{
					return new LoginResponse() { authed = false };
				}
			}
			catch
			{
				return new LoginResponse() { authed = false };
			}
		}
	}
	public class LoginResponse
	{
		public bool authed { get; set; }

		public string? error { get; set; }
		public string? parameter { get; set; }

		public string? session { get; set; }
		public string? user { get; set; }
		public string? token { get; set; }
	}
}