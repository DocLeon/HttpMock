﻿using System.Net;
using NUnit.Framework;

namespace HttpMock.Integration.Tests
{
	[TestFixture]
	public class SameServerUsingDifferentBaseUrlsTests
	{

		[Test]
		public void UsingAppOne()
		{
			string expected = "expected response";
			HttpMockRepository.At("http://localhost:8080/appone")
				.Stub(x => x.Get("/appone/endpoint"))
				.Return(expected)
				.OK();

			WebClient wc = new WebClient();
			Assert.That(wc.DownloadString("http://localhost:8080/appone/endpoint"), Is.EqualTo(expected));
		}

		[Test]
		public void UsingAppTwo()
		{
			string expected = "expected response";
			HttpMockRepository.At("http://localhost:8080/apptwo")
				.Stub(x => x.Get("/apptwo/endpoint"))
				.Return(expected)
				.OK();

			WebClient wc = new WebClient();
			Assert.That(wc.DownloadString("http://localhost:8080/apptwo/endpoint"), Is.EqualTo(expected));
		}

		[Test]
		public void UsingAppThree()
		{
			string expected = "expected response";
			HttpMockRepository.At("http://localhost:8080/appthree")
				.Stub(x => x.Get("/appthree/endpoint"))
				.Return(expected)
				.OK();

			WebClient wc = new WebClient();
			Assert.That(wc.DownloadString("http://localhost:8080/appthree/endpoint"), Is.EqualTo(expected));
		}
	}
}