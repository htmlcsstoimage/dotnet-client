using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Represents a request to create a templated image using a specified template ID, template version,
/// and a set of parameterized template values.
/// </summary>
public class CreateTemplatedImageRequest:ICreateImageRequestBase
{
    /// <summary>
    /// Identifies the specific template to be used for generating the templated image.
    /// </summary>
    /// <remarks>
    /// This property is mandatory and must correspond to a valid template identifier configured in the system.
    /// Ensure that the provided identifier matches an existing template to avoid errors during the image generation process.
    /// </remarks>
    public required string TemplateId { get; set; }

    /// <summary>
    /// Specifies the version of the template to be used when generating the templated image.
    /// </summary>
    /// <remarks>
    /// This property is optional and allows clients to target a specific version of the template.
    /// If not provided, the newest available version of the template will be used by default.
    /// Ensure that the version matches the one supported by the template configuration.
    /// </remarks>
    public long? TemplateVersion { get; set; }

    /// <summary>
    /// Represents a collection of key-value pairs that defines the dynamic data to be injected into a template
    /// during the creation of a templated image.
    /// </summary>
    /// <remarks>
    /// The property is required and must be provided as an instance of <see cref="System.Text.Json.Nodes.JsonObject"/>.
    /// Typically, this is used to specify template-specific fields and their corresponding values.
    /// </remarks>
    public required JsonObject TemplateValues { get; set; }

    /// <summary>
    /// Creates a new <see cref="CreateTemplatedImageRequest"/> using the provided template values, template ID, and an optional template version.
    /// </summary>
    /// <typeparam name="T">The type of the template values object to be serialized.</typeparam>
    /// <param name="templateValues">The object containing the template values to be applied in the templated image.</param>
    /// <param name="templateId">The ID of the template to be used for generating the image.</param>
    /// <param name="templateVersion">The optional version of the template to be used. If not provided, the default version is used.</param>
    /// <returns>
    /// An instance of <see cref="CreateTemplatedImageRequest"/> containing the specified template values, template ID, and template version.
    /// </returns>
    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    public static CreateTemplatedImageRequest FromObject<T>(T templateValues, string templateId, long? templateVersion = null) => FromObjectCoreNoTypeInfo(templateValues, templateId, templateVersion);


    /// <summary>
    /// Creates a new instance of <see cref="CreateTemplatedImageRequest"/> using the provided template values, template ID,
    /// and optionally a specified template version.
    /// </summary>
    /// <typeparam name="T">The type of the template values object to be serialized.</typeparam>
    /// <param name="templateValues">The object containing the parameterized template values to be applied in the templated image.</param>
    /// <param name="templateId">The ID of the template to be used for generating the image.</param>
    /// <param name="typeInfo">The JsonTypeInfo object for the type of the template values.</param>
    /// <param name="templateVersion">The optional version of the template to be used. If not provided, the default version will be used.</param>
    /// <returns>
    /// An instance of <see cref="CreateTemplatedImageRequest"/> containing the provided template values, template ID, and optionally, the specified template version.
    /// </returns>
    public static CreateTemplatedImageRequest FromObject<T>(T templateValues, string templateId, JsonTypeInfo<T> typeInfo, long? templateVersion = null) =>
        FromObjectCore(templateValues, templateId, templateVersion, typeInfo);

    /// <summary>
    /// Creates an instance of <see cref="CreateTemplatedImageRequest"/> using the specified template values, template ID, and an optional template version.
    /// </summary>
    /// <typeparam name="T">The type of the object containing the template values.</typeparam>
    /// <param name="templateValues">The object containing the dynamic template values to be serialized into the request.</param>
    /// <param name="templateId">The ID of the template to be used for creating the image.</param>
    /// <param name="jsonSerializerOptions">An optional JsonSerializerOptions object to use for serializing the template values.</param>
    /// <param name="templateVersion">The optional version of the template to be used. If not specified, the default version will be applied.</param>
    /// <returns>
    /// An instance of <see cref="CreateTemplatedImageRequest"/> containing the specified template values, template ID, and template version.
    /// </returns>
    public static CreateTemplatedImageRequest FromObject<T>(T templateValues, string templateId, JsonSerializerOptions jsonSerializerOptions, long? templateVersion = null) =>
        FromObjectCore(templateValues, templateId, templateVersion, null, jsonSerializerOptions);

    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    private static CreateTemplatedImageRequest FromObjectCoreNoTypeInfo<T>(T templateValues, string templateId, long? templateVersion = null)
    {
        var serialized_values = JsonSerializer.SerializeToNode(templateValues);
        if (serialized_values == null || serialized_values.GetValueKind() != JsonValueKind.Object)
        {
            throw new ArgumentException("Invalid parameter values");
        }

        return new()
        {
            TemplateId = templateId,
            TemplateVersion = templateVersion,
            TemplateValues = serialized_values.AsObject()
        };
    }
    private static CreateTemplatedImageRequest FromObjectCore<T>(T templateValues, string templateId, long? templateVersion = null, JsonTypeInfo<T>? typeInfo = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        JsonNode? serialized_values;
        if (typeInfo != null)
        {
            serialized_values = JsonSerializer.SerializeToNode(templateValues, typeInfo);
        }else if (jsonSerializerOptions != null)
        {
            serialized_values = JsonSerializer.SerializeToNode(templateValues, jsonSerializerOptions.GetTypeInfo(typeof(T)));
        }
        else
        {
            throw new ArgumentException("Must provide either typeInfo or jsonSerializerOptions");
        }

        if (serialized_values == null || serialized_values.GetValueKind() != JsonValueKind.Object)
        {
            throw new ArgumentException("Invalid parameter values");
        }

        return new()
        {
            TemplateId = templateId,
            TemplateVersion = templateVersion,
            TemplateValues = serialized_values.AsObject()
        };
    }

}