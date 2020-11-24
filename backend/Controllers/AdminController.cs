using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToothSoupAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ToothSoupAPI.Controllers
{
	[ApiController]
	[Authorize(Roles = UserRole.ADMIN)]
	[Route("api/[controller]")]
	public class AdminController : ControllerBase
	{
		private readonly Database _db;

		public AdminController(Database database)
		{
			_db = database;
		}
	}
}
