using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using AccessControl.Web;
using AccessControl.Web.Api.V1;
using Application.IRepositories;
using Application.Services.Devices;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests.Web.Api_V1;

public static class Globals
{
	public static string baseUrl = "https://localhost:7036";
	public static string apiVersion = "/api/v1/";
}

public class DevicesTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly WebApplicationFactory<Program> _factory;
	
	private readonly AccessControlDbContext context;
	private readonly IDeviceService DeviceService;
	private readonly IDeviceRepository DeviceRepository;

	public DevicesTests(WebApplicationFactory<Program> factory, IDeviceService ds, IDeviceRepository dr, AccessControlDbContext _context)
	{
		_factory = factory;
		
		DeviceService = ds;
		DeviceRepository = dr;
		context = _context;
	}
	
	[Fact]
	public async Task ListAllDevicesFromDatabase()
	{
		// Arrange
		var options = new WebApplicationFactoryClientOptions();
		options.BaseAddress = new Uri(Globals.baseUrl + Globals.apiVersion);
		var client = _factory.CreateClient(options);
		var content = new StringContent("{'ids': [\"aaa\"]}", Encoding.UTF8, "application/json");
		
	
		
		
		// Act
		var response = await client.PostAsync("devices/getAll", content);
		var contents = await response.Content.ReadAsStringAsync();

		// Assert
		response.EnsureSuccessStatusCode();
	}
}