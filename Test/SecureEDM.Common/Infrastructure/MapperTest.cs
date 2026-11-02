namespace SecureEDM.Common.Infrastructure;

public class MapperTest
{

    public record RecordEntity(
        int Id,
        string Name,
        DateOnly DOB
    );
    public class ClassEntity
    {
        public int Id { get; init; }
        public string Name { get; private set; } = null!;
        public DateOnly DOB { get; private set; }
    }
    public record RecordDto(
        string Name,
        DateOnly DOB
    );
    public class ClassDto
    {
        public string Name { get; set; } = null!;
        public DateOnly DOB { get; set; }
    }
    [Fact]
    public void MappingToRecordDto()
    {
        MapperService mapper = new();
        RecordEntity entity = new(10, "Bob", new(2000, 4, 20));
        RecordDto dto = mapper.Map<RecordEntity, RecordDto>(entity);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.DOB, dto.DOB);
    }
    [Fact]
    public void MappingToClassDto()
    {
        MapperService mapper = new();
        RecordEntity entity = new(10, "Bob", new(2000, 4, 20));
        ClassDto dto = mapper.Map<RecordEntity, ClassDto>(entity);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.DOB, dto.DOB);
    }
    [Fact]
    public void MappingToClassEntity()
    {
        MapperService mapper = new();
        ClassDto dto = new()
        {
            Name = "Bob",
            DOB = new(2000, 4, 20)
        };
        ClassEntity entity = mapper.Map<ClassDto, ClassEntity>(dto);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.DOB, dto.DOB);
    }
}