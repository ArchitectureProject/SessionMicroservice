using System.ComponentModel.DataAnnotations.Schema;

namespace SessionMicroservice.Models.Entities;

[Table("session")]
public class Session
{
    public string Id { get; set; }
    
    // Unix Epoch Time date
    public long StartedAt { get; set; }
    public long? ClosedAt { get; set; }
    
    public string LocationQrCode { get; set; }
    public List<string> Orders { get; set; } = new();
    public float TotalPrice { get; set; }
    public float CurrentlyPaid { get; set; }
    public bool ActuallyProcessingPayment { get; set; }
}