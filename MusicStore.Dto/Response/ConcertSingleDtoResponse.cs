namespace MusicStore.Dto.Response;

public class ConcertSingleDtoResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;
    public string Place { get; set; } = default!;

    public string DateEvent { get; set; } = default!;

    public string TimeEvent { get; set; } = default!;

    public string? ImageUrl { get; set; }
    public int TicketsQuantity { get; set; }
    public string Status { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public GenreDtoResponse GenreDtoResponse { get; set; } = default!;
}