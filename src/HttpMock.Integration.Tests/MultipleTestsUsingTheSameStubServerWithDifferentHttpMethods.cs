using System.IO;
using System.Net;
using HttpMock;
using NUnit.Framework;
using System;

namespace SevenDigital.HttpMock.Integration.Tests
{
	[TestFixture]
	public class MultipleTestsUsingTheSameStubServerWithDifferentHttpMethods
	{
		private const string ENDPOINT_TO_HIT = "http://localhost:11111/endpoint";
		private IHttpServer _httpMockRepository;

		WebClient _client;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_httpMockRepository = HttpMockRepository.At("http://localhost:11111/");
			_client = new WebClient();

		}

		[SetUp]
		public void SetUp()
		{
			_httpMockRepository.WithNewContext();
		}

		[Test]
		public void Should_get()
		{
			const string expectedResponse = "I am a GET";
			_httpMockRepository
				.Stub(x => x.Get("/endpoint"))
				.Return(expectedResponse)
				.OK();

			var response = _client.DownloadString(ENDPOINT_TO_HIT);
			Assert.That(response, Is.EqualTo(expectedResponse)); ;
		}

		[Test]
		public void Should_post()
		{
			const string expectedResponse = "I am a POST";
			_httpMockRepository
				.Stub(x => x.Post("/endpoint"))
				.Return(expectedResponse)
				.OK();


			var response = _client.UploadString(ENDPOINT_TO_HIT, "data");
			Assert.That(response, Is.EqualTo(expectedResponse));


		}

		[Test]
		public void Should_put()
		{
			const string expectedResponse = "I am a PUT";
			_httpMockRepository
				.Stub(x => x.Put("/endpoint"))
				.Return(expectedResponse)
				.OK();

			var response = _client.UploadString(ENDPOINT_TO_HIT, "PUT", "data");
			Assert.That(response, Is.EqualTo(expectedResponse));
		}

		[Test]
		public void Should_delete()
		{
			const string expectedResponse = "I am a DELETE";
			_httpMockRepository
				.Stub(x => x.Delete("/endpoint"))
				.Return(expectedResponse)
				.OK();

			var response = _client.UploadString(ENDPOINT_TO_HIT, "DELETE", "data");
			Assert.That(response, Is.EqualTo(expectedResponse));
		}

		[Test]
		public void Should_head()
		{
			_httpMockRepository
				.Stub(x => x.Head("/endpoint"))
				.OK();


			var webRequest = (HttpWebRequest)WebRequest.Create(ENDPOINT_TO_HIT);
			webRequest.Method = "HEAD";

			using (var response = (HttpWebResponse)webRequest.GetResponse())
			{
				var str = new StreamReader(response.GetResponseStream()).ReadToEnd();
				Console.WriteLine(str);
				Assert.That(response.Headers.Count, Is.GreaterThan(0));
				Assert.That(response.ContentLength, Is.EqualTo(0));
			}
		}

		[Test]
		public void When_no_mocked_endpoints_matched_then_should_return_404_with_http_mock_error_status()
		{

			try
			{
				var response = _client.DownloadString("http://localhost:11111/zendpoint");
			}
			catch (WebException ex)
			{
				Assert.That(((HttpWebResponse)ex.Response).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
				Assert.That(((HttpWebResponse)ex.Response).Headers["SevenDigital-HttpMockError"], Is.Not.Null, "Header not set");
			}
		}
	}
}