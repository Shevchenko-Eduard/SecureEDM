namespace SecureEDM.Common.Domain.Models.ValueObject;

public class RangeTest
{
    [Fact]
    public void CreateCorrectRange()
    {
        int from = 0;
        int to = 10;
        _ = new Range<int>(from: from, to: to);
        Assert.True(true);
    }

    [Fact]
    public void CreateIncorrectRange()
    {
        int from = 0;
        int to = 10;
        Assert.Throws<ExceptionInvalidDate>(() => new Range<int>(from: to, to: from));
    }

    [Fact]
    public void IncorrectSetInRange()
    {
        int from = 0;
        int to = 10;
        Range<int> range = new(from: from, to: to);
        Assert.Throws<ExceptionInvalidDate>(() =>
        {
            range.To = from;
            range.From = to;
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void TrueContaintsInRange(int value)
    {
        int from = 0;
        int to = 10;
        Range<int> range = new(from: from, to: to);
        Assert.True(range.Contains(value));
    }
    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void FalseContaintsInRange(int value)
    {
        int from = 0;
        int to = 10;
        Range<int> range = new(from: from, to: to);
        Assert.False(range.Contains(value));
    }
    [Theory]
    [InlineData(-1, -1)]
    [InlineData(11, 1)]
    public void BiasRange(int value, int res)
    {
        int from = 0;
        int to = 10;
        Range<int> range = new(from: from, to: to);
        Assert.Equal(res, range.Bias(value));
    }
}