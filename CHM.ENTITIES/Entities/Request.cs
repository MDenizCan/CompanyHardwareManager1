using CHM.ENTITIES.Common;

namespace CHM.ENTITIES.Entities;

// Talep tipini belirten numaralandırma.
public enum RequestType
{
    FaultReport,    // Arıza bildirimi
    ReturnRequest   // İade talebi
}

// Talebin güncel durumunu belirten numaralandırma.
public enum RequestStatus
{
    Pending,        // Beklemede / Henüz işleme alınmadı
    Approved,       // Onaylandı
    Rejected,       // Reddedildi
    Completed       // İşlem tamamlandı
}

// Kullanıcıların sistem üzerinden yapabileceği talepleri temsil eden sınıf (Örn: Arıza Bildirimi).
public class Request : BaseEntity
{
    // Talebi yapan kullanıcının ID'si.
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Eğer bir cihazla ilgiliyse cihazın ID'si. (Genel taleplerde null olabilir)
    public Guid? AssetId { get; set; }
    public Asset? Asset { get; set; }

    // Talebin türü.
    public RequestType Type { get; set; }

    // Talebin detaylı açıklaması. Kullanıcının notları burada tutulur.
    public string Description { get; set; } = null!;

    // Talebin durumu (Varsayılan olarak "Beklemede").
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
}
