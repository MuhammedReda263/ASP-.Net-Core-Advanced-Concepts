﻿using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                var descriper = services.SingleOrDefault(temp => temp.ServiceType == (typeof(DbContextOptions<ApplicationDbContext>)));
                if (descriper != null)
                   services.Remove(descriper);
                
                services.AddDbContext<ApplicationDbContext>(Options => Options.UseInMemoryDatabase("DatabaseForTesting"));

            });


        }

    }
}
