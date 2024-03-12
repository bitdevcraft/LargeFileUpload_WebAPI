using System.Text;
using tusdotnet.Helpers;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Models.Expiration;
using tusdotnet.Stores;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton(CreateTusConfiguration);

            services.AddCors(opt =>
            {
                opt.AddPolicy(
                    "CorsPolicy",
                    policy =>
                    {
                        policy
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithOrigins("http://localhost:5173")
                            .WithExposedHeaders(CorsHelper.GetExposedHeaders());
                    }
                );
            });
            return services;
        }

        private static DefaultTusConfiguration CreateTusConfiguration(IServiceProvider serviceProvider)
        {

            //File upload path
            var tmpFiles = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            if (!Directory.Exists(tmpFiles))
                Directory.CreateDirectory(tmpFiles);
            var tusFiles = Path.Combine(Directory.GetCurrentDirectory(), "SavedFile", "tusfiles");
            if (!Directory.Exists(tusFiles))
                Directory.CreateDirectory(tusFiles);

            return new DefaultTusConfiguration
            {
                UrlPath = "/files",
                //File storage path
                Store = new TusDiskStore(tmpFiles),
                //Does metadata allow null values
                MetadataParsingStrategy = MetadataParsingStrategy.AllowEmptyValues,
                //The file will not be updated after expiration
                Expiration = new AbsoluteExpiration(TimeSpan.FromMinutes(5)),
                //Event handling (various events, meet your needs)
                Events = new Events
                {
                    //Upload completion event callback
                    OnFileCompleteAsync = async ctx =>
                    {
                        //Get upload file
                        var file = await ctx.GetFileAsync();

                        //Get upload file=
                        var metadatas = await file.GetMetadataAsync(ctx.CancellationToken);

                        //Get the target file name in the above file metadata
                        var fileNameMetadata = metadatas["name"];

                        //The target file name is encoded in Base64, so it needs to be decoded here
                        var fileName = fileNameMetadata.GetString(Encoding.UTF8);

                        var extensionName = Path.GetExtension(fileName);

                        //Convert the uploaded file to the actual target file
                        File.Move(Path.Combine(tmpFiles, ctx.FileId), Path.Combine(tusFiles, $"{ctx.FileId}{extensionName}"));
                    }
                }
            };
        }
    }
}