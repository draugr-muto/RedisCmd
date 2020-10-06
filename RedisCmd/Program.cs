using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace RedisClient
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				IDatabase cache = Connection.GetDatabase();
				var key = args[0];
				string value = cache.StringGet(key);
				Console.WriteLine($"Value => {value}");
				var ttl = cache.KeyTimeToLive(key);
				Console.WriteLine($"ttl => {ttl.Value.TotalSeconds}");
			}
			finally
			{
				lazyConnection.Value.Dispose();
			}
		}

		private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
		{
			string path = Directory.GetParent(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))).ToString();
			var builder = new ConfigurationBuilder()
				.SetBasePath(path)
				.AddJsonFile("appsettings.json");
			var config = builder.Build();
			string cacheConnetction = config["ConnectionStrings:RedisConnection"];
			return ConnectionMultiplexer.Connect(cacheConnetction);
		});

		public static ConnectionMultiplexer Connection
		{
			get
			{
				return lazyConnection.Value;
			}
		}
	}
}
