using Microsoft.EntityFrameworkCore;
using SportProductsWeb.Services;
using System.Text.Json.Serialization;

namespace SportProductsWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // read email config

            EmailSetting emailconfig = builder.Configuration.GetSection("EmailSetting").Get<EmailSetting>();


            // Add services to the container.

            builder.Services.AddSingleton(emailconfig);

            builder.Services.AddScoped<AppEmailService>();

            builder.Services.AddDbContext<ShopContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConn")));

            builder.Services.AddControllers().AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            }).AddXmlSerializerFormatters();

            builder.Services.AddRazorPages();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDefaultIdentity<UserApplication>(
                op =>
                {
                    op.SignIn.RequireConfirmedAccount = false;
                }
                ).AddEntityFrameworkStores<ShopContext>();

            builder.Services.AddScoped<ProductsRep>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();


            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "";
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.MapSwagger();

            app.Run();
        }
    }
}