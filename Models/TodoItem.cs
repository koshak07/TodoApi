using Swashbuckle.AspNetCore.Annotations;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace TodoApi.Models;

public class TodoItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    [SwaggerSchema(ReadOnly = true)]
    public ICollection<FileModel>? FileModels { get; set; } = new Collection<FileModel>();
}