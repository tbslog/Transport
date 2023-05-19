﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.DriverModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.DriverManage;
using TBSLogistics.Service.Services.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class DriverController : ControllerBase
	{
		private readonly IDriver _driver;
		private readonly IPaginationService _uriService;
		private readonly ICommon _common;

		public DriverController(IDriver driver, IPaginationService uriService, ICommon common)
		{
			_common = common;
			_driver = driver;
			_uriService = uriService;
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreateAccountDriver(string driverId)
		{
			var create = await _driver.CreateAccountDriver(driverId);

			if (create.isSuccess)
			{
				return Ok(create.Message);
			}
			else
			{
				return BadRequest(create.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreateDriver(CreateDriverRequest request)
		{
			var checkPermission = await _common.CheckPermission("M0002");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var create = await _driver.CreateDriver(request);

			if (create.isSuccess == true)
			{
				return Ok(create.Message);
			}
			else
			{
				return BadRequest(create.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> EditDriver(string driverId, EditDriverRequest request)
		{
			var checkPermission = await _common.CheckPermission("M0003");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var update = await _driver.EditDriver(driverId, request);

			if (update.isSuccess == true)
			{
				return Ok(update.Message);
			}
			else
			{
				return BadRequest(update.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> DeleteDriver(string driverId)
		{
			var update = await _driver.DeleteDriver(driverId);

			if (update.isSuccess == true)
			{
				return Ok(update.Message);
			}
			else
			{
				return BadRequest(update.Message);
			}
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> getDriverById(string driverId)
		{
			var checkPermission = await _common.CheckPermission("M0003");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var driver = await _driver.GetDriverById(driverId);
			return Ok(driver);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> getDriverCardId(string cccd)
		{
			var driver = await _driver.GetDriverByCardId(cccd);
			return Ok(driver);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListSelectDriver()
		{
			var list = await _driver.GetListDriverSelect();

			return Ok(list);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListDriver([FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("M0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _driver.getListDriver(filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ListDriverRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
			return Ok(pagedReponse);

		}
	}
}
