﻿using System;
using System.Collections.Generic;
using Manatee.Trello;
using Manatee.Trello.Contracts;
using Manatee.Trello.ManateeJson;
using Manatee.Trello.Test;
using Manatee.Trello.WebApi;

namespace NugetTest
{
	class Program
	{
		private class DebugLog : ILog
		{
			public void Debug(string message, params object[] parameters)
			{
				System.Diagnostics.Debug.WriteLine(message, parameters);
			}
			public void Info(string message, params object[] parameters)
			{
				System.Diagnostics.Debug.WriteLine(message, parameters);
			}
			public void Error(Exception e, bool shouldThrow = true)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
				if (shouldThrow)
					throw e;
			}
		}

		static void Main(string[] args)
		{
			Run(() =>
				{
					Console.WriteLine(Member.Me);
					Console.WriteLine(Member.Me.CreationDate);
				});
			Console.ReadLine();
		}

		private static void Run(System.Action action)
		{
			var serializer = new ManateeSerializer();
			TrelloConfiguration.Serializer = serializer;
			TrelloConfiguration.Deserializer = serializer;
			TrelloConfiguration.JsonFactory = new ManateeFactory();
			TrelloConfiguration.RestClientProvider = new WebApiClientProvider();
			TrelloConfiguration.Log = new DebugLog();

			TrelloAuthorization.Default.AppKey = TrelloIds.AppKey;
			TrelloAuthorization.Default.UserToken = TrelloIds.UserToken;

			action();

			TrelloProcessor.Shutdown();
		}

		private static void OutputCollection<T>(string section, IEnumerable<T> collection)
		{
			Console.WriteLine();
			Console.WriteLine(section);
			foreach (var item in collection)
			{
				Console.WriteLine("    {0}", item);
			}
		}
	}
}
