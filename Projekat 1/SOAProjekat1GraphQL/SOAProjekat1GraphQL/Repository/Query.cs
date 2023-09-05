namespace SOAProjekat1GraphQL.Repository
{
    public class Query
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<LogVal> GetLogValue([Service] LogDbContext context) =>
            context.LogVals;
    }
}
