using System.Net;
using HttpMock;
using NUnit.Framework;

namespace SevenDigital.HttpMock.Integration.Tests
{
	public class StubConstraintsTests
	{
		private IHttpServer _httpMockRepository;
		private IHttpServer _stubHttp;
		private WebClient _wc;

		[Test]
		public void Constraints_can_be_applied_to_urls()
		{
			_stubHttp
				.Stub(x => x.Post("/firsttest"))
				.WithUrlConstraint(url => url.Contains("/blah/blah") == false)
				.Return("<Xml>ShouldntBeReturned</Xml>")
				.OK();

			try
			{
				_wc.UploadString("Http://localhost:9090/firsttest/blah/blah", "x");

				Assert.Fail("Should have 404d");
			}
			catch (WebException ex)
			{
				Assert.That(((HttpWebResponse)ex.Response).StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
			}
		}

		[SetUp]
		public void SetUp()
		{
			_httpMockRepository = HttpMockRepository.At("http://localhost:9090/");
			_wc = new WebClient();
			_stubHttp = _httpMockRepository.WithNewContext();
		}
	}
}