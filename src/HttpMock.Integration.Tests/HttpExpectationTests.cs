using System.Net;
using NUnit.Framework;

namespace HttpMock.Integration.Tests
{
	[TestFixture]
	public class HttpExpectationTests
	{
		[Test]
		public void Should_assert_a_request_was_made() {
			var stubHttp = HttpMockRepository.At("http://localhost:9091");
			stubHttp.Stub(x => x.Get("/api/status")).Return("OK").OK();

			new WebClient().DownloadString("http://localhost:9091/api/status");

			stubHttp.AssertWasCalled(x => x.Get("/api/status"));

		}

		[Test]
		public void Should_assert_that_a_request_was_not_made()
		{
			var stubHttp = HttpMockRepository.At("http://localhost:9091");
			stubHttp.Stub(x => x.Get("/api/status")).Return("OK").OK();
			stubHttp.Stub(x => x.Get("/api/echo")).Return("OK").OK();

			new WebClient().DownloadString("http://localhost:9091/api/status");

			stubHttp.AssertWasNotCalled(x => x.Get("/api/echo"));
		}

		[Test]
		public void Should_assert_when_stub_is_missing()
		{
			var stubHttp = HttpMockRepository.At("http://localhost:9091");

			Assert.Throws<AssertionException>(() => stubHttp.AssertWasCalled(x => x.Get("/api/echo")));
		}

		[Test]
		public void Should_match_a_POST_request_was_made_with_the_expected_body() {

			var stubHttp = HttpMockRepository.At("http://localhost:9091");
			stubHttp.Stub(x => x.Post("/endpoint/handler")).Return("OK").OK();

			string expectedData = "postdata";
			new WebClient().UploadString("http://localhost:9091/endpoint/handler", expectedData);

			stubHttp.AssertWasCalled(x => x.Post("/endpoint/handler")).WithBody(expectedData) ;
		}


	}
}