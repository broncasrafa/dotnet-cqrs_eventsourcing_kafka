using System.ComponentModel.DataAnnotations;

namespace Post.Command.Infrastructure.Configurations;

public class MongoDbConfig
{
    [Required(ErrorMessage = "Connection string is required")]
    public string ConnectionString { get; set; }

    [Required(ErrorMessage = "Database name is required")]
    public string DatabaseName { get; set; }

    [Required(ErrorMessage = "Collection name is required")]
    public string CollectionName { get; set; }
}
