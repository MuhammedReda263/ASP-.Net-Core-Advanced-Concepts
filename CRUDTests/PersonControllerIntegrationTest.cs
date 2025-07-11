using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CRUDTests
{
    public class PersonControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public PersonControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        #region Index
        [Fact]
        public async Task Index_ToReturnView()
        {
            HttpResponseMessage Response = await _client.GetAsync("/persons/index");
            Response.EnsureSuccessStatusCode();

            string content = await Response.Content.ReadAsStringAsync();
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(content);
            var document = html.DocumentNode;
            document.QuerySelector("table.persons").Should().NotBeNull();

        

        }

        #endregion
    }
}
