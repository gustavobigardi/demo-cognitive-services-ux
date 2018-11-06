using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "apivaluesuploadpost")
        {
            operation.Parameters.Clear();
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "uploadedFile",
                In = "formData",
                Description = "Upload File",
                Required = true,
                Type = "file"
            });
            operation.Consumes.Add("multipart/form-data");
        }
        }
    }
}