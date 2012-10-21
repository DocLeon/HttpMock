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

		[SetUp]
		public void SetUp() {
			_httpMockRepository = HttpMockRepository.At("http://localhost:11111/");
			_client = new WebClient();

		}

		[Test]
		public void Should_get() {
			var expectedResponse = "I am a GET";
			_httpMockRepository
				.Stub(x => x.Get("/endpoint"))
				.Return(expectedResponse)
				.OK();

			var response = _client.DownloadString(ENDPOINT_TO_HIT);
			Assert.That(response, Is.EqualTo(expectedResponse));;
		}

		[Test]
		public void Should_post() {
			var expected = "I am a POST";
			_httpMockRepository
				.Stub(x => x.Post("/endpoint"))
				.Return(expected)
				.OK();


			var response = _client.UploadString(ENDPOINT_TO_HIT, "data");


		}

		[Test]
		public void Should_put() {
			_httpMockRepository
				.Stub(x => x.Put("/endpoint"))
				.Return("I am a PUT")
				.OK();

			AssertResponse("PUT", "I am a PUT");
		}

		[Test]
		public void Should_delete() {
			_httpMockRepository
				.Stub(x => x.Delete("/endpoint"))
				.Return("I am a DELETE")
				.OK();
		
			AssertResponse("DELETE", "I am a DELETE");
		}

		[Test]
		public void Should_head() {
			_httpMockRepository
				.Stub(x => x.Head("/endpoint"))
				.OK();


			var webRequest = (HttpWebRequest)WebRequest.Create(ENDPOINT_TO_HIT);
			webRequest.Method = "HEAD";

			using (var response = (HttpWebResponse) webRequest.GetResponse()) {
				var str = new StreamReader (response.GetResponseStream ()).ReadToEnd ();
				 Console.WriteLine(str);
				Assert.That(response.Headers.Count, Is.GreaterThan(0));
				Assert.That(response.ContentLength, Is.EqualTo(0));
			}
		}

		[Test]
		public void If_no_Mocked_Endpoints_matched_then_should_return_404_with_HttpMockError_status() {
			var webRequest = (HttpWebRequest)WebRequest.Create("http://localhost:11111/zendpoint");
			try {
				using(webRequest.GetResponse()) {
				}
			} catch(WebException ex){
				Assert.That(((HttpWebResponse)ex.Response).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
				Assert.That(((HttpWebResponse)ex.Response).Headers["SevenDigital-HttpMockError"], Is.Not.Null, "Header not set");
			}
		}

		private void AssertResponse(string method, string expected) {

			var webRequest = (HttpWebRequest)WebRequest.Create(ENDPOINT_TO_HIT);
			webRequest.Method = method;
			StreamWriter streamWriter = new StreamWriter( webRequest.GetRequestStream());
			using(streamWriter){
				streamWriter.Write(expected);
			}
			using(var response = webRequest.GetResponse()) {
				using(var sr = new StreamReader(response.GetResponseStream()))
				{
					string readToEnd = sr.ReadToEnd();
					Assert.That(readToEnd, Is.EqualTo(expected));
				}
			}
		}
	}
}