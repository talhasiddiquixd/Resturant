using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RestaurantPOS.Helpers
{
    public class ConfigurationHelper
    {
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IConfiguration config;
        private IWebHostEnvironment hostingEnvironment;

        public ConfigurationHelper(IConfiguration config, IWebHostEnvironment hostingEnvironment)
        {
            this.config = config;
            this.hostingEnvironment = hostingEnvironment;
        }

        public ConfigurationHelper(IConfiguration config, IErrorLogRepository errorLogRepository, IWebHostEnvironment hostingEnvironment = null)
        {
            _config = config;
            _errorLogRepository = errorLogRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// Get Image Path 
        /// </summary>
        /// <param name="imageLocationConfigName"></param>
        /// <returns>Return image base path string</returns>
        public string GetBasePath(string imageLocationConfigName)
        {
            string folderName = _config[imageLocationConfigName];
            string apiBaseUrl = _config["Utility:APIBaseURL"];
            return apiBaseUrl + folderName;
        }
        /// <summary>
        /// Get Image Path 
        /// </summary>
        /// <param name="imageLocationConfigName"></param>
        /// <returns>Return image base path string</returns>
        public string GetSaveImagePath(string imageLocationConfigName)
        {
            string folderName = config[imageLocationConfigName];
            if (string.IsNullOrWhiteSpace(hostingEnvironment.WebRootPath))
            {
                hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            string baseUrl = hostingEnvironment.WebRootPath;
            var path = Path.Combine(baseUrl, folderName);
            return path;
        }
        /// <summary>
        /// Get Image base path by using these parameters
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="loginUserId">int</param>
        /// <returns>Return image base path string</returns>
        /// 
        public void SaveError(Exception ex, string loginUserId, string controllerName, string actionName)
        {
            _errorLogRepository.Save(new ErrorLogRequestDTO()
            {
                Id = 0,
                LogController = controllerName,
                LogAction = actionName,
                LogMessage = ex?.Message,
                LogDetail = ex?.StackTrace,
                ErrorLogDate = DateTime.Now.ToString("dd/MMM/yyyy"),
                ErrorLogTime = DateTime.Now.ToShortTimeString(),
                ErrorCode = "500",
                ErrorLine = ex?.Source ?? "N/A",
                IsSynchronized = false,
            });
        }
    }
    public class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext schemaFilterContext)
        {
            if (schema.Properties.Count == 0)
                return;

            const BindingFlags bindingFlags = BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance;
            var memberList = schemaFilterContext.Type
                                .GetFields(bindingFlags).Cast<MemberInfo>()
                                .Concat(schemaFilterContext.Type
                                .GetProperties(bindingFlags));
            var excludedList = memberList.Where(m =>
                                                m.GetCustomAttribute<SwaggerIgnoreAttribute>()
                                                != null)
                                         .Select(m =>
                                             (m.GetCustomAttribute<JsonPropertyAttribute>()
                                              ?.PropertyName
                                              ?? m.Name.ToCamelCase()));
            foreach (var excludedName in excludedList)
            {
                if (schema.Properties.ContainsKey(excludedName))
                    schema.Properties.Remove(excludedName);
            }
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }
    internal static class StringExtensions
    {
        internal static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
    }
}
