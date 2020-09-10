using System;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using System.Text.Unicode;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Client.Model;
using System.Net.Http.Headers;

namespace Client
{
	class Program
	{

		static async Task Main(string[] args)
		{
			var result = await PostUser(); //Creates user

			var authenticateUser = await AuthUser(); //authenticates user

			var authResult = JsonConvert.DeserializeObject<AuthenticateResponse>(authenticateUser);//converts from Json
			Console.WriteLine("\njwttoken: " + authResult.JwtToken);
			Console.WriteLine("\nrefresh token: " + authResult.RefreshToken);

			var newTokenResponse = await RefreshToken(authResult.JwtToken, authResult.RefreshToken);
			var newAuthResult = JsonConvert.DeserializeObject<AuthenticateResponse>(newTokenResponse);
			//AuthenticateResponse refreshedToken = JsonConvert.DeserializeObject<AuthenticateResponse>(refreshToken);
			//RefreshTokenRequest refreshedToken = JsonConvert.DeserializeObject<RefreshTokenRequest>(refreshToken);
			//Console.WriteLine("\n" + refreshedToken.RefreshToken );
			
			Console.WriteLine("\nrefreshed jwt token: " + newAuthResult.JwtToken );
			Console.WriteLine("\nrefreshed refresh token: " + newAuthResult.RefreshToken);

			
			Console.ReadKey();
		}

		public static async Task<string> PostUser()
		{
			var user = new RegisterUser();
			user.Title = "Mr";
			user.FirstName = "Niklas";
			user.LastName = "Fredriksson";
			user.Email = "fredriksson-niklas@hotmail.com";
			user.Password = "Hejalla1234";
			user.ConfirmPassword = "Hejalla1234";
			user.AcceptTerms = true;
			

			var url = "http://localhost:4001/accounts/register";
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			using var client = new HttpClient();

			var response = await client.PostAsync(url, data);

			string result = response.Content.ReadAsStringAsync().Result;
			return result;


		}

		public static async Task<string> AuthUser()
		{
			var user = new User { Email = "fredriksson-niklas@hotmail.com", Password = "Hejalla1234" };
			var url = "http://localhost:4001/accounts/authenticate";
			var json = JsonConvert.SerializeObject(user);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();
			

			var response = await client.PostAsync(url, data);

			string result = response.Content.ReadAsStringAsync().Result;

			return result;
		}


		public static async Task<string> RefreshToken(string token, string refreshToken)
		{
			var url = "http://localhost:4001/accounts/refresh-token";

			var refreshTokenReq = new RefreshTokenRequest { RefreshToken = refreshToken };
			var json = JsonConvert.SerializeObject(refreshTokenReq);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync(url, data);
			
			
			//var response = await client.GetAsync(url);

			string result = response.Content.ReadAsStringAsync().Result;

			return result;
		}

		//public static async Task<string> Request()
		//{

		//}

		
	}
}
