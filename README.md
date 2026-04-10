# 🏨 Royal Hotel Dashboard & Rezervasyon Platformu

ASP.NET Core MVC ile geliştirilmiş, veritabanı gerektirmeyen otel arama ve bilgi platformudur.
Booking.com API entegrasyonu ile gerçek zamanlı otel arama ve detay görüntülemenin yanı sıra;
döviz, kripto para, hava durumu, akaryakıt fiyatları ve yapay zeka destekli öneriler sunan
canlı bir yönetim paneli içerir.

---

## 🚀 Özellikler

### 📊 Yönetim Paneli (Dashboard)

Tek sayfalık dashboard ekranında aşağıdaki canlı veriler anlık olarak gösterilir:

#### 💱 Döviz Kurları
- USD → TRY
- EUR → TRY
- GBP → TRY

#### 💎 Kripto Para Fiyatları
- Bitcoin (BTC)
- Ethereum (ETH)
- BNB (Binance Coin)

#### ⛽ Akaryakıt Fiyatları
- Benzin
- Motorin (Mazot)
- LPG

#### 🌤️ Hava Durumu
- Seçilen şehre ait anlık sıcaklık ve hava durumu bilgisi

#### 🤖 Yapay Zeka Destekli İçerik (Claude API)
- Günün yemeği önerisi (kültürel ve ilgi çekici)
- Aydın'a özel seyahat ipuçları ve gezi rotası

#### 🗺️ Gezi Rotası Önerisi
- Arandığı şehir için gezilmesi gereken 3-5 popüler lokasyon
- Her lokasyon için kısa açıklama

---

### 🏩 Otel Arama ve Rezervasyon (Booking.com API)

<img width="825" height="425" alt="image" src="https://github.com/user-attachments/assets/ddb4d8fa-5fa0-409f-bfe0-baad1dd2b65b" />


#### API Akışı (İki Adımlı)
**1. Adım — Destination ID Bulma**
- Kullanıcının girdiği şehir adı ile `searchDestination` endpoint'i çağrılır
- API'den dönen ilk `dest_id` değeri alınır

**2. Adım — Otel Listeleme**
- `searchHotels` endpoint'i çağrılır
- Destination ID, tarihler, kişi sayısı ve diğer parametrelerle filtreleme yapılır
- Sonuçlar otel kartları halinde listelenir

#### Otel Listesi Kartında Gösterilenler
- Otel adı ve adresi
- Gecelik fiyat (EUR)
- Misafir puanı ve yorum sayısı
- Ana fotoğraf

#### Otel Detay Sayfası
Listeden bir otele tıklandığında `getHotelDetails` endpoint'i ile detay sayfası açılır:
- Yüksek çözünürlüklü oda fotoğrafları (galeri/slider)
- Otel adı, adresi ve şehir bilgisi
- Gecelik fiyat (EUR)
- Misafir puanı, yorum sayısı ve puan etiketi
- Tesis olanakları (havuz, WiFi, restoran vb. — ilk 10 olanak)
- Oda açıklaması



<img width="852" height="436" alt="image" src="https://github.com/user-attachments/assets/5310be76-b442-4ae0-a43b-96618c44fb48" />



## 🔒 Güvenlik

- Tüm API anahtarları `.NET User Secrets` ile saklanır, repoya dahil edilmez
- `appsettings.json` içinde hiçbir API anahtarı bulunmaz
- Tema ve şablon dosyaları repoya eklenmeden önce gömülü anahtar içerip içermediği kontrol edilmelidir

---

## 📌 Notlar

- Proje veritabanı gerektirmez, tüm veriler API'lerden anlık olarak çekilir
- Akaryakıt fiyatları şu an statik tanımlıdır, ilerleyen sürümde API entegrasyonu planlanmaktadır
- Giriş/çıkış tarihleri otel listesinden detay sayfasına `Session` aracılığıyla taşınmaktadır

---

> Geliştirici: Münevver Orman — 2026
