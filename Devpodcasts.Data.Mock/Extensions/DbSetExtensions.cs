using Microsoft.EntityFrameworkCore;
using Moq;

namespace devpodcasts.data.mock.Extensions;

public static class DbSetExtensions
{
    public static DbSet<T> BuildMockDbSet<T>(this IEnumerable<T> source) where T : class
    {
        var data = source.ToList();
        var queryable = data.AsQueryable();

        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        return mockSet.Object;
    }
}