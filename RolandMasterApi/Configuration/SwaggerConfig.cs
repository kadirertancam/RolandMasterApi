using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace RolandMasterApi.Configuration
{
    /// <summary>
    /// Swagger yapılandırması
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Swagger servislerini ekler
        /// </summary>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Roland VT-4 Ses Efekt Yönetim API",
                    Version = "v1",
                    Description = "Roland VT-4 efekt cihazlarını merkezi olarak yönetmek için geliştirilmiş API servisini kullanarak tüm ses kartlarını tek bir merkezden kontrol edebilirsiniz. Bu API ile ses kartlarını kayıt etme, robot efekti, harmony, megaphone, reverb, vocoder ve equalizer gibi efektleri cihazlara uygulama işlemlerini gerçekleştirebilirsiniz.",
                    Contact = new OpenApiContact
                    {
                        Name = "Justtech Destek Ekibi",
                        Email = "info@justtech.work",
                        Url = new Uri("https://justtech.work")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT Lisansı",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Endpoint'leri gruplandır
                c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });

                // API açıklamalarını XML dosyasından al
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
                
                // Endpoint gruplarını özelleştir
                c.TagActionsBy(api => 
                {
                    if (api.ActionDescriptor.RouteValues["controller"] == "Devices")
                        return new[] { "Cihaz Yönetimi" };
                    else if (api.ActionDescriptor.RouteValues["controller"] == "Commands")
                        return new[] { "Komut Yönetimi" };
                    else
                        return new[] { api.ActionDescriptor.RouteValues["controller"] };
                });
                
                // Yazar, oluştuma tarihi gibi operasyon filtreleri ekle
                c.OperationFilter<SwaggerOperationFilter>();
                
                // Türkçe kategori isimleri için sırala
                c.OrderActionsBy(apiDesc => apiDesc.GroupName);
            });

            return services;
        }

        /// <summary>
        /// Swagger middleware'ini yapılandırır
        /// </summary>
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
                    };
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Roland VT-4 Ses Efekt Yönetim API v1");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = "Roland VT-4 API Dokümantasyonu";
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                c.DefaultModelsExpandDepth(1);
                c.EnableFilter();
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.DisplayOperationId();
                c.ConfigObject.AdditionalItems.Add("persistAuthorization", true);
                c.ConfigObject.AdditionalItems.Add("tagsSorter", "alpha");
                
                // Özel CSS ekle
                c.InjectStylesheet("/swagger-ui/custom.css");
            });

            return app;
        }
    }

    /// <summary>
    /// Swagger operasyon filtreleri
    /// </summary>
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // İşlem açıklamalarına ekstra bilgiler ekle
            operation.Extensions.Add("x-created-at", new OpenApiString(DateTime.Now.ToString("yyyy-MM-dd")));
            operation.Extensions.Add("x-developer", new OpenApiString("Roland VT-4 API Ekibi"));
            
            // Türkçe operasyon açıklamaları için özel etiketler ekle
            if (operation.Summary?.Contains("listele") == true)
            {
                operation.Tags.Add(new OpenApiTag { Name = "Listeleme İşlemleri" });
            }
            else if (operation.Summary?.Contains("ekle") == true || operation.Summary?.Contains("oluştur") == true)
            {
                operation.Tags.Add(new OpenApiTag { Name = "Kayıt İşlemleri" });
            }
            else if (operation.Summary?.Contains("güncelle") == true)
            {
                operation.Tags.Add(new OpenApiTag { Name = "Güncelleme İşlemleri" });
            }
            else if (operation.Summary?.Contains("sil") == true)
            {
                operation.Tags.Add(new OpenApiTag { Name = "Silme İşlemleri" });
            }
        }
    }
}