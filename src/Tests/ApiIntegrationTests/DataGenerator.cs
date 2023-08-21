using Core.Entities;
using Framework.Identification;

namespace ApiIntegrationTests;

public class DataGenerator
{
    public DataGenerator()
    {
        SampleOwner = new Owner
        {
            Id = SequentialGuid.Create(),
            Name = "Sample Test Owner",
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow,
            IsDeleted = false,
        };
    }

    public Owner SampleOwner;
}
