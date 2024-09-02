using Microsoft.AspNetCore.Mvc;
using WebFormsMVC.Models;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using System.Text;
using NuGet.Protocol.Plugins;
namespace WebFormsMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Index()
        {
            
            return View("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            using(var httpClient=new HttpClient())
            {
                StringContent content=new StringContent(JsonConvert.SerializeObject(user),Encoding.UTF8,"application/json");
                using(var response=await httpClient.PostAsync("http://localhost:5055/api/data", content))
                {
                    string apiResponse=await response.Content.ReadAsStringAsync();

                }
            }
          
            return RedirectToAction("Display", user);
        }
        public IActionResult Display(User UserDetails)
        {

            return View(UserDetails);
        }
  
        public async Task<IActionResult> UserList()
        {
            List<User> users = new List<User>();
            using(var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://localhost:5243/api/data"))
                {
                    string apiResponse= await response.Content.ReadAsStringAsync();
                    users=JsonConvert.DeserializeObject<List<User>>(apiResponse);
                }
            }

            return View(users);
        }
        public IActionResult Edit(int id)
        {
            var constring = _configuration.GetConnectionString("myDb1");
            SqlConnection con = new SqlConnection(constring);
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Users where userId=@UserId", con);
            cmd.Parameters.AddWithValue("@UserId", id);
            SqlDataReader reader = cmd.ExecuteReader();
            
            
            while (reader.Read())
            {
                ViewBag.Id = reader.GetInt32(0);
                ViewBag.Name = reader.GetString(1);
                ViewBag.Email = reader.GetString(2);
                ViewBag.Password = reader.GetString(3);
            }

            return View();  
        }
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(user.Id.ToString()), "Id");
                content.Add(new StringContent(user.Name),"Name");
                content.Add(new StringContent(user.Email),"Email");
                content.Add(new StringContent(user.Password),"Password");

                using (var response = await httpClient.PutAsync("http://localhost:5243/api/data", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                }
            }
            return RedirectToAction("UserList");
        }
       
        public async Task<IActionResult> Delete(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("http://localhost:5243/api/data/" + id))
                {
                    string apiResponse=await response.Content.ReadAsStringAsync();  
                }
            }
            return RedirectToAction("UserList");
        }

    }
}
