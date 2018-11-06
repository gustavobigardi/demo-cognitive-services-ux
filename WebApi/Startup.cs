using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Filters;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IComputerVisionClient>(c => {
                var computerVisionClient = new ComputerVisionClient(
                    new ApiKeyServiceClientCredentials("3bf62ffad207448e9569fa36530d64ff"),
                    new DelegatingHandler[] { }
                );
                computerVisionClient.Endpoint = "https://brazilsouth.api.cognitive.microsoft.com/";
                return computerVisionClient;
            });

            services.AddScoped<ICadastroService, CadastroService>();

            services.AddCors(options =>
            {
                options.AddPolicy("Development",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Cognitive Services",
                    Description = "Swagger",
                    Contact = new Contact { Name = "Gustavo Bigardi", Email = "gbbigardi@gmail.com", Url = "https://blog.gbbigardi.tech" },
                    License = new License { Name = "None", Url = "about:blank" },
                    
                });
                s.DescribeAllEnumsAsStrings();
                s.OperationFilter<FileUploadOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("Devlopment");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cognitive Services");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
