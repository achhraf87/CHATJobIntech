using JobIntechCHAT.API.Data;
using JobIntechCHAT.API.Interfaces;
using JobIntechCHAT.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace JobIntechCHAT.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationsServices(this IServiceCollection builder,IConfiguration config)
        {

            builder.AddDbContext<AppDbContext>(options => options.UseSqlServer(
            config.GetConnectionString("Db")));
            builder.AddScoped<ITokenService, TokenServices>();
            return builder;
        }
    }
}
