
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using Vue.Data;
using Vue.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using CertificateManager;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Data;

namespace it_template.Areas.V1.Controllers
{

	[Authorize(Roles = "Administrator")]
	public class UserController : BaseController
	{
		private UserManager<UserModel> UserManager;
		private RoleManager<IdentityRole> RoleManager;
		private readonly IConfiguration _configuration;
		public UserController(AuthContext context, UserManager<UserModel> UserMgr, RoleManager<IdentityRole> RoleMgr, IConfiguration configuration) : base(context)
		{
			_configuration = configuration;
			UserManager = UserMgr;
			RoleManager = RoleMgr;
		}

		// POST: UserController/Create
		[HttpPost]
		public async Task<JsonResult> Create(UserModel User, string password, List<string> roles)
		{

			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			var user_current = await UserManager.GetUserAsync(currentUser); // Get user id:
																			//string password = "!PMP_it123456";
			UserModel user = new UserModel
			{
				Email = User.Email,
				UserName = User.Email,
				EmailConfirmed = true,
				FullName = User.FullName,
				image_url = User.image_url,
				departments = User.departments,
				expiry_date = DateTime.Now + TimeSpan.FromDays(180)
			};
			IdentityResult result = await UserManager.CreateAsync(user, password);
			if (result.Succeeded)
			{
				//return Ok(result);
				foreach (string group in roles)
				{
					await UserManager.AddToRoleAsync(user, group);
				}
				// Generate private-public key pair
				//var serviceProvider = new ServiceCollection()
				//	  .AddCertificateManager()
				//	  .BuildServiceProvider();

				//string passwordPublic = "!PMP_it123456";
				//var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

				//X509Certificate2 rootCaL1 = new X509Certificate2("private/rootca/localhost_root.pfx", passwordPublic);
				//var serverL3 = createClientServerAuthCerts.NewClientChainedCertificate(
				//	new DistinguishedName { CommonName = user.FullName + "<" + user.Email + ">", OrganisationUnit = user.position },
				//	new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
				//	"localhost", rootCaL1);
				//var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();
				//var serverCertL3InPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(passwordPublic, serverL3, rootCaL1);
				//System.IO.File.WriteAllBytes("private/pfx/" + user.Id + ".pfx", serverCertL3InPfxBtyes);

				/// Audittrail
				var audit = new AuditTrailsModel();
				audit.UserId = user_current.Id;
				audit.Type = AuditType.Create.ToString();
				audit.DateTime = DateTime.Now;
				audit.description = $"Tài khoản {user.FullName} đã tạo tài khoản mới.";
				audit.TableName = "UserModel";
				audit.PrimaryKey = user.Id;
				audit.NewValues = JsonConvert.SerializeObject(user);

				_context.Add(audit);
				await _context.SaveChangesAsync();

				return Json(new { success = true, message = "Tạo thành công" });
			}
			else
				return Json(new { success = false, message = "Email đã tồn tài!" });

		}


		// POST: UserController/Edit/5
		[HttpPost]
		public async Task<JsonResult> Edit(UserModel User, List<string> roles)
		{
			UserModel User_old = await UserManager.FindByIdAsync(User.Id);
			var OldValues = JsonConvert.SerializeObject(User_old);
			User_old.Email = User.Email;
			User_old.UserName = User.Email;
			User_old.FullName = User.FullName;
			User_old.image_url = User.image_url;
			User_old.departments = User.departments;


			var RolesForThisUser = await UserManager.GetRolesAsync(User_old);
			await UserManager.RemoveFromRolesAsync(User_old, RolesForThisUser);
			foreach (string group in roles)
			{
				await UserManager.AddToRoleAsync(User_old, group);
			}

			IdentityResult result = await UserManager.UpdateAsync(User_old);
			if (result.Succeeded)
			{
				/// Audittrail
				System.Security.Claims.ClaimsPrincipal currentUser = this.User;
				var user = await UserManager.GetUserAsync(currentUser); // Get user
				var audit = new AuditTrailsModel();
				audit.UserId = user.Id;
				audit.Type = AuditType.Update.ToString();
				audit.DateTime = DateTime.Now;
				audit.description = $"Tài khoản {user.FullName} đã chỉnh sửa cho tài khoản {User_old.FullName}.";
				audit.TableName = "UserModel";
				audit.PrimaryKey = User_old.Id;
				audit.NewValues = JsonConvert.SerializeObject(User_old);
				audit.OldValues = OldValues;
				_context.Add(audit);
				await _context.SaveChangesAsync();

				return Json(new { success = true, message = "Thành công" });
			}
			else
				return Json(new { success = false, message = "Xảy ra lỗi!" });

		}

		public async Task<IActionResult> Delete(string id)
		{
			UserModel User = await UserManager.FindByIdAsync(id);
			if (User != null)
			{
				var OldValues = JsonConvert.SerializeObject(User);
				User.deleted_at = DateTime.Now;
				IdentityResult result = await UserManager.UpdateAsync(User);
				if (result.Succeeded)
				{
					/// Audittrail
					System.Security.Claims.ClaimsPrincipal currentUser = this.User;
					var user = await UserManager.GetUserAsync(currentUser); // Get user
					var audit = new AuditTrailsModel();
					audit.UserId = user.Id;
					audit.Type = AuditType.Delete.ToString();
					audit.DateTime = DateTime.Now;
					audit.description = $"Tài khoản {user.FullName} đã xóa tài khoản {User.FullName}.";
					audit.TableName = "UserModel";
					audit.PrimaryKey = User.Id;
					audit.NewValues = JsonConvert.SerializeObject(User);
					audit.OldValues = OldValues;
					_context.Add(audit);
					await _context.SaveChangesAsync();
				}
			}
			return Redirect("/user");
		}
		public async Task<JsonResult> Get(string id)
		{
			UserModel User = await _context.UserModel.Where(d => d.Id == id).FirstOrDefaultAsync();
			var roles = await UserManager.GetRolesAsync(User);
			return Json(new { success = true, id = User.Id, departments = User.departments, roles = roles, email = User.Email, FullName = User.FullName, image_url = User.image_url });
		}

		[HttpPost]
		public async Task<JsonResult> Table()
		{
			var draw = Request.Form["draw"].FirstOrDefault();
			var start = Request.Form["start"].FirstOrDefault();
			var length = Request.Form["length"].FirstOrDefault();
			var searchValue = Request.Form["search[value]"].FirstOrDefault();
			int pageSize = length != null ? Convert.ToInt32(length) : 0;
			int skip = start != null ? Convert.ToInt32(start) : 0;
			var customerData = (from tempcustomer in UserManager.Users select tempcustomer);
			customerData = customerData.Where(m => m.deleted_at == null);
			int recordsTotal = customerData.Count();
			if (!string.IsNullOrEmpty(searchValue))
			{
				customerData = customerData.Where(m => m.UserName.Contains(searchValue) || m.FullName.Contains(searchValue));
			}
			int recordsFiltered = customerData.Count();
			var datapost = customerData.Skip(skip).Take(pageSize).ToList();
			var data = new ArrayList();
			foreach (var record in datapost)
			{
				var image = "<img src='" + record.image_url + "' class='thumb-sm rounded-circle'>";
				var sign = "<img src='" + record.image_sign + "' class='' width='100'>";
				var data1 = new
				{
					action = "<div class='btn-group'><a href='/v1/user/delete/" + record.Id + "' class='btn btn-danger btn-sm' title='Xóa?' data-type='confirm'>'"
						+ "<i class='fas fa-trash-alt'>"
						+ "</i>"
						+ "</a></div>",
					Id = "<a href='/user/edit/" + record.Id + "'><i class='fas fa-pencil-alt mr-2'></i> " + record.Id + "</a>",
					email = record.Email,
					name = record.FullName,
					sign = sign,
					image = image
				};
				data.Add(data1);
			}
			var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
			return Json(jsonData);
		}

		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			public string id { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "New password")]
			public string? NewPassword { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[DataType(DataType.Password)]
			[Display(Name = "Confirm new password")]
			[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
			public string? ConfirmPassword { get; set; }
		}
		[HttpPost]
		public async Task<IActionResult> ChangePassword(InputModel Input)
		{
			//Get User By Id
			var User = await UserManager.FindByIdAsync(Input.id);

			//Generate Token
			var token = await UserManager.GeneratePasswordResetTokenAsync(User);

			//Set new Password
			var changePasswordResult = await UserManager.ResetPasswordAsync(User, token, Input.NewPassword);

			if (!changePasswordResult.Succeeded)
			{
				var ErrorMessage = "";
				foreach (var error in changePasswordResult.Errors)
				{
					ErrorMessage += error.Description + "<br>";
				}
				return Json(new { success = false, message = ErrorMessage });
			}
			User.expiry_date = DateTime.Now + TimeSpan.FromDays(180);
			await UserManager.UpdateAsync(User);

			/// Audittrail
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			var user = await UserManager.GetUserAsync(currentUser); // Get user
			var audit = new AuditTrailsModel();
			audit.UserId = user.Id;
			audit.Type = AuditType.Update.ToString();
			audit.DateTime = DateTime.Now;
			audit.description = $"Tài khoản {user.FullName} đã thay đổi mật khẩu tài khoản {User.FullName}.";
			_context.Add(audit);
			await _context.SaveChangesAsync();

			return Json(new { success = true, message = "Mật khẩu đã được thay đổi" });

		}



		public async Task<JsonResult> Roles()
		{
			var Model = RoleManager.Roles.Select(a => new
			{
				id = a.Name,
				label = a.Name
			}).ToList();
			//var jsonData = new { data = ProcessModel };
			return Json(Model);
		}
		public async Task<JsonResult> Departments()
		{
			var All = GetChild(0);
			//var jsonData = new { data = ProcessModel };
			return Json(All);
		}
		private List<SelectResponse> GetChild(int parent)
		{
			var DepartmentModel = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == parent).OrderBy(d => d.stt).ToList();
			var list = new List<SelectResponse>();
			if (DepartmentModel.Count() > 0)
			{
				foreach (var department in DepartmentModel)
				{
					var DepartmentResponse = new SelectResponse
					{

						id = department.id.ToString(),
						label = department.name
					};
					var count_child = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == department.id).Count();
					if (count_child > 0)
					{
						var child = GetChild(department.id);
						DepartmentResponse.children = child;
					}
					list.Add(DepartmentResponse);
				}
			}
			return list;
		}

		public class SelectResponse
		{
			public string id { get; set; }
			public string label { get; set; }

			public string name { get; set; }
			public virtual List<SelectResponse> children { get; set; }
		}
	}
}
