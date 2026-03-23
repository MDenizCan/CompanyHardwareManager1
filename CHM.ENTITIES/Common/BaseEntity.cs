using System;

namespace CHM.ENTITIES.Common;

// Tüm veritabanı tablolarının (Entity'lerin) türetileceği (miras alacağı) temel sınıf.
// Bu sayede her tabloda Id, CreatedAt gibi alanları tekrar tekrar yazmak zorunda kalmayız.
public abstract class BaseEntity
{
    // Her kaydın benzersiz kimliği (Primary Key). GUID kullanılması çakışma riskini sıfıra indirir.
    public Guid Id { get; set; }
    
    // Kaydın ne zaman oluşturulduğu. Otomatik olarak şu anki UTC zamanı atanır.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Kayıt güncellendiğinde buraya tarih yazılır. Soru işareti (?) boş (null) olabileceğini gösterir.
    public DateTime? UpdatedAt { get; set; }
    
    // Kayıt silindiğinde buraya tarih yazılır.
    public DateTime? DeletedAt { get; set; }
    
    // Gerçekten silmek (Hard Delete) yerine, silindi olarak işaretlemek (Soft Delete) için kullanılır.
    // Başlangıçta false'tur.
    public bool IsDeleted { get; set; } = false;
}
