using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using User.Gateway.Configure;
using User.Gateway.Extensions;
using User.Gateway.Utils;

namespace User.Gateway
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
            services.AddHttpContextAccessor();

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.RegisterService();

            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(xmlFilePath);
            });

            services.RegisterJWT(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
                app.UseSwagger(options =>
                {
                    options.PreSerializeFilters.Add((swagger, req) =>
                    {
                        swagger.Servers = new List<OpenApiServer>() { new OpenApiServer() { Url = $"https://{req.Host}" } };
                    });
                });

                app.UseSwaggerUI(options =>
                {
                    foreach (var desc in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"../swagger/{desc.GroupName}/swagger.json", desc.ApiVersion.ToString());
                        options.DefaultModelsExpandDepth(-1);
                        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    }
                });
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                string bearer = context.Request.Headers["Authorization"];
                bearer = bearer ?? "";
                var accessToken = bearer.Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    var jwtToken = handler.ReadJwtToken(accessToken);
                    var userIdClaim = jwtToken.Claims.SingleOrDefault(x => x.Type.Equals(ClaimUtil.USER_ID));
                    var tokenIdClaim = jwtToken.Claims.SingleOrDefault(x => x.Type.Equals(ClaimUtil.TOKEN_ID));
                    var userNameClaim = jwtToken.Claims.SingleOrDefault(x => x.Type.Equals(ClaimUtil.USERNAME));
                    context.Items[ClaimUtil.USER_ID] = userIdClaim is null ? "" : userIdClaim.Value;
                    context.Items[ClaimUtil.TOKEN_ID] = tokenIdClaim is null ? "" : tokenIdClaim.Value;
                    context.Items[ClaimUtil.USERNAME] = userNameClaim is null ? "" : userNameClaim.Value;
                    context.Items[ClaimUtil.ACCESS_TOKEN] = accessToken is null ? "" : accessToken;
                }
                catch (Exception)
                {
                }

                await next.Invoke();
            });

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
