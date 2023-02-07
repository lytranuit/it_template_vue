using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Vue.Data;
namespace it_template.Areas.V1.Controllers
{
	[Area("V1")]
	//[Authorize(Roles = "Administrator")]
	 [Authorize]
	public class BaseController : Controller
	{
		protected readonly AuthContext _context;

		public BaseController(AuthContext context)
		{
			_context = context;
		}
	}
}
