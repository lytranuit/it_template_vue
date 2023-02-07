using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Vue.Models
{

	[Table("AspNetUsers")]
	public class UserModel : IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string FullName { get; set; }
		public string? position { get; set; }
		public string? image_sign { get; set; }
		public string? image_url { get; set; }
		public DateTime? expiry_date { get; set; }
		public DateTime? last_login { get; set; }
		public bool? is_first_login { get; set; }

		public string? msnv { get; set; }
		public DateTime? created_at { get; set; }

		public DateTime? updated_at { get; set; }

		public DateTime? deleted_at { get; set; }

		public string? list_department { get; set; }
		[NotMapped]
		public virtual List<string>? departments
		{
			get
			{
				//Console.WriteLine(settings);
				return JsonSerializer.Deserialize<List<string>>(string.IsNullOrEmpty(list_department) ? "[]" : list_department);
			}
			set
			{
				list_department = JsonSerializer.Serialize(value);
			}
		}

	}
}
