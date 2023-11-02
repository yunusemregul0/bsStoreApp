namespace Entities.DataTransferObjects
{
    //public int Id "{ get; init; } //readonly
    //public string Title { get; init; }
    //public decimal Price { get; init; }
    public record BookDto //For application.xml feedback
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public decimal Price { get; init; }

    }

}
