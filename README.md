# Avans Yönetim Sistemi

Bu proje, şirketlerin avans taleplerini yönetmek için geliştirilmiş kapsamlı bir web uygulamasıdır. ASP.NET Core MVC framework'ü kullanılarak geliştirilmiş olup, modern web teknolojileri ve güvenlik standartlarına uygun olarak tasarlanmıştır.

## 📋 İçindekiler

- [Özellikler](#özellikler)
- [Teknolojiler](#teknolojiler)
- [Kurulum](#kurulum)
- [Geliştirme Ortamı](#geliştirme-ortamı)
- [Proje Yapısı](#proje-yapısı)
- [API Dokümantasyonu](#api-dokümantasyonu)
- [Güvenlik](#güvenlik)
- [Test](#test)
- [Dağıtım](#dağıtım)
- [Katkıda Bulunma](#katkıda-bulunma)
- [Lisans](#lisans)
- [İletişim](#iletişim)

## 🚀 Özellikler

### Kullanıcı Yönetimi
- **Rol Tabanlı Yetkilendirme**
  - Admin: Sistem yöneticisi, tüm yetkilere sahip
  - Birim Müdürü: Kendi birimindeki avans taleplerini yönetebilir
  - Direktör: Departman avans taleplerini onaylayabilir
  - Genel Müdür Yardımcısı: Yüksek tutarlı avans taleplerini onaylayabilir
  - Genel Müdür: Tüm avans taleplerini onaylayabilir
  - Finans Müdürü: Finansal onay süreçlerini yönetebilir
  - Ön Muhasebe: Avans ödemelerini takip edebilir

- **Kullanıcı Profil Yönetimi**
  - Profil bilgilerini görüntüleme ve düzenleme
  - Şifre değiştirme
  - Profil fotoğrafı yükleme

- **Güvenlik**
  - Çok faktörlü kimlik doğrulama
  - Oturum yönetimi
  - Güvenli şifre politikaları

### Avans Talepleri
- **Talep Oluşturma**
  - Avans miktarı belirleme
  - Açıklama ekleme
  - Gerekli belgeleri yükleme
  - Talep geçmişi görüntüleme

- **Onay Süreçleri**
  - Çok seviyeli onay akışı
  - Onay/red işlemleri
  - Onay geçmişi takibi
  - Otomatik bildirimler

- **Raporlama**
  - Talep istatistikleri
  - Departman bazlı raporlar
  - Zaman bazlı analizler
  - Excel export özelliği

### Bildirim Sistemi
- **Gerçek Zamanlı Bildirimler**
  - Talep durumu değişiklikleri
  - Onay/red bildirimleri
  - Sistem bildirimleri
  - Okunmamış bildirim sayacı

- **Bildirim Tercihleri**
  - E-posta bildirimleri
  - Sistem içi bildirimler
  - Bildirim filtreleme

## 💻 Teknolojiler

### Backend
- ASP.NET Core MVC 8.0
- Entity Framework Core
- Identity Framework
- AutoMapper
- FluentValidation
- MediatR
- NLog

### Frontend
- Bootstrap 5
- jQuery
- DataTables
- Font Awesome
- Toastr
- SweetAlert2

### Veritabanı
- SQL Server
- Entity Framework Core Migrations

### Güvenlik
- JWT Authentication
- Role-Based Authorization
- CSRF Protection
- XSS Protection
- SQL Injection Prevention

## 🛠️ Kurulum

### Gereksinimler
- .NET 6.0 SDK
- Visual Studio 2022 veya Visual Studio Code
- SQL Server 2019 veya üzeri
- Git

### Adımlar

1. **Projeyi Klonlama**
```bash
git clone [repo-url]
cd VarlikYönetimi
```

2. **Veritabanı Kurulumu**
```bash
# appsettings.json dosyasında connection string'i güncelleyin
# Package Manager Console'da:
Update-Database
```

3. **Uygulamayı Çalıştırma**
```bash
dotnet run --project VarlikYönetimi.MVC
```

4. **Varsayılan Kullanıcılar**
- Admin: admin@example.com / Admin123!
- Birim Müdürü: mudur@example.com / Mudur123!
- Çalışan: calisan@example.com / Calisan123!

## 🔧 Geliştirme Ortamı

### IDE Ayarları
- Visual Studio 2022
  - .NET 8.0 SDK
  - ASP.NET ve web geliştirme iş yükü
  - SQL Server veri araçları

### Kod Standartları
- Clean Architecture
- SOLID Prensipleri
- Repository Pattern
- Unit of Work Pattern
- CQRS Pattern

## 📁 Proje Yapısı

```
VarlikYönetimi/
├── VarlikYönetimi.Core/
│   ├── Entities/
│   ├── Interfaces/
│   ├── DTOs/
│   └── Enums/
├── VarlikYönetimi.Data/
│   ├── Context/
│   ├── Repositories/
│   └── Migrations/
├── VarlikYönetimi.Services/
│   ├── Services/
│   ├── Mappings/
│   └── Validators/
└── VarlikYönetimi.MVC/
    ├── Controllers/
    ├── Views/
    ├── ViewModels/
    └── wwwroot/
```

## 🔒 Güvenlik

### Uygulanan Güvenlik Önlemleri
- HTTPS zorunluluğu
- Güvenli şifre politikaları
- Oturum yönetimi
- CSRF koruması
- XSS koruması
- SQL Injection koruması
- Rate limiting
- Input validation

### Güvenlik Testleri
- OWASP Top 10 kontrolü
- Dependency scanning
- Code analysis
- Penetration testing

## 🧪 Test

### Test Türleri
- Unit Tests
- Integration Tests
- UI Tests
- Performance Tests
- Security Tests

### Test Araçları
- xUnit
- Moq
- Selenium
- Postman
- JMeter

## 🚀 Dağıtım

### Deployment Seçenekleri
- IIS
- Docker
- Azure App Service
- AWS Elastic Beanstalk

### CI/CD Pipeline
- GitHub Actions
- Azure DevOps
- Jenkins

## 🤝 Katkıda Bulunma

1. Fork'layın
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 📞 İletişim

Proje Linki: [https://github.com/alitumer0/AvansYonetimi](https://github.com/alitumer0/AvansYonetimi)

E-posta: [aetumer50@gmail.com](mailto:aetumer50@gmail.com) 
