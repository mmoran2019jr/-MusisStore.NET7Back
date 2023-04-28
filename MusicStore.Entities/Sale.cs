using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStore.Entities;

public class Sale : EntityBase
{
    [ForeignKey(nameof(CustomerFk))]
    public Customer Customer { get; set; } = default!;

    public int CustomerFk { get; set; }

    public Concert Concert { get; set; } = default!;

    public int ConcertId { get; set; }

    public DateTime SaleDate { get; set; }

    public string OperationNumber { get; set; } = default!;

    public decimal Total { get; set; }
    
    public short Quantity { get; set; } // 255
    
}