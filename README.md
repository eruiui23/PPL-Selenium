# PPL-Selenium

Eksplorasi otomasi pengujian web menggunakan **Selenium WebDriver** dengan **C# / .NET 10** dan **NUnit** pada studi kasus [SauceDemo](https://www.saucedemo.com) serta [The Internet (Dynamic Loading)](https://the-internet.herokuapp.com/dynamic_loading).

---

## Pengenalan Selenium & Studi Kasus

**Selenium WebDriver** adalah pustaka otomasi pengujian peramban (*browser*) yang memungkinkan skrip kode berinteraksi langsung dengan elemen halaman web sebagaimana pengguna nyata. Pada proyek ini, Selenium dipadukan dengan **C# / .NET 10** dan **NUnit** untuk menguji alur web modern secara otomatis, terstruktur, dan berulang.

### Arsitektur Singkat WebDriver
- **Test & Page Object (`SeleniumTutorial.Tests`):** Kode C# + NUnit mengirim perintah aksi atau *assertion* melalui API Selenium WebDriver.
- **WebDriver / Selenium Manager:** Mengelola komunikasi protokol WebDriver dan membantu penyediaan driver peramban (Chrome) secara otomatis.
- **Browser (Google Chrome):** Mengeksekusi perintah pada halaman web (baik dalam mode *headed* maupun *headless*) dan mengembalikan status atau elemen DOM ke skrip pengujian.

### Studi Kasus yang Digunakan
- **SauceDemo (`https://www.saucedemo.com`):** Aplikasi e-commerce sampel untuk mempraktikkan *Page Object Model* (POM), berbagai strategi *locator* (`By.Id`, `By.Name`, `By.CssSelector` berbasis `data-test`, `By.XPath`, `By.ClassName`, dan `By.LinkText`), serta alur *login*, *cart*, dan *checkout*.
- **The Internet — Dynamic Loading (`https://the-internet.herokuapp.com/dynamic_loading`):** Halaman uji pemuatan DOM asinkron (*Hidden Element* dan *Rendered Element*) untuk mendemokan **Explicit Wait** (`WebDriverWait` melalui `WaitHelper`).

---

## Prasyarat

Pastikan lingkungan pengembangan memenuhi kebutuhan berikut sebelum menjalankan proyek:
- **.NET 10 SDK** (sesuai `global.json` dengan versi SDK `10.0.100`, `rollForward: latestFeature`, dan target framework `net10.0`).
- **Google Chrome** terpasang di sistem.
- **Koneksi internet** saat *restore* paket NuGet dan saat pengujian mengakses situs target (`saucedemo.com` dan `the-internet.herokuapp.com`).

---

## Instalasi

1. **Clone repository:**
   ```powershell
   git clone <url-repository-github>
   cd PPL-Selenium
   ```

2. **Restore dependensi NuGet dan build solution:**
   ```powershell
   dotnet restore SeleniumTutorial.slnx
   dotnet build SeleniumTutorial.slnx
   ```

Paket utama yang digunakan di `src/SeleniumTutorial.Tests/SeleniumTutorial.Tests.csproj` meliputi `NUnit`, `NUnit3TestAdapter`, `Microsoft.NET.Test.Sdk`, `Selenium.WebDriver`, dan `Selenium.Support`.

---

## Cara Menjalankan

Jalankan perintah berikut dari direktori root repository:

- **Menjalankan seluruh test utama (mode *headed* default):**
  ```powershell
  dotnet test SeleniumTutorial.slnx
  ```

- **Menjalankan seluruh test dalam mode *headless* (PowerShell):**
  ```powershell
  $env:HEADLESS="true"; dotnet test SeleniumTutorial.slnx
  ```

- **Menjalankan seluruh test dalam mode *headless* (Bash / Linux / macOS):**
  ```bash
  HEADLESS=true dotnet test SeleniumTutorial.slnx
  ```

- **Menjalankan test tertentu berdasarkan nama class/method:**
  ```powershell
  dotnet test SeleniumTutorial.slnx --filter "FullyQualifiedName~WaitDemoTests"
  ```

---

## Hasil yang Diharapkan

- **`dotnet build` dan `dotnet test` berhasil (exit code `0`)** dengan seluruh *test case* pada *Login*, *LocatorDemo*, *Cart*, *Checkout*, dan *WaitDemo* berstatus *Passed*.
- Pada mode *headless* (`HEADLESS=true`), Chrome berjalan tanpa membuka jendela antarmuka grafis penuh namun tetap mengeksekusi pengujian hingga selesai.
- Jika terjadi kegagalan *test* (`TestStatus.Failed`), `BaseTest.TearDown()` memanggil `ScreenshotHelper.Capture(...)` untuk menyimpan file `.png` ke folder `TestResults/Screenshots/` dan melampirkannya pada konteks hasil test.

---

## Troubleshooting

- **Gagal *restore* paket NuGet:** Periksa koneksi internet, pastikan konfigurasi NuGet aktif (`dotnet nuget list source`), lalu ulangi `dotnet restore SeleniumTutorial.slnx`.
- **Ketidaksesuaian versi .NET SDK:** Cek SDK terpasang dengan `dotnet --info` dan pastikan SDK .NET 10 tersedia sesuai `global.json`.
- **Browser / WebDriver gagal dimulai:** Pastikan Google Chrome terpasang dan mutakhir serta tidak ada proses Chrome/driver yang menggantung di latar belakang.
- **Test gagal karena elemen lambat muncul atau *stale*:** Pastikan menunggu kondisi yang tepat melalui `WaitHelper` / `BasePage` alih-alih memakai `Thread.Sleep` di kode utama.
- **Gaya kode tidak sesuai `.editorconfig`:** Jalankan `dotnet format SeleniumTutorial.slnx` sebelum melakukan *commit*.

---

## Explicit Waits

Pada pengujian web modern (seperti aplikasi React atau halaman yang memuat data secara asinkron), elemen sering kali belum langsung muncul atau belum siap diklik tepat setelah halaman terbuka. Untuk menanganinya dengan stabil dan cepat, proyek ini menggunakan **Explicit Wait** melalui `WebDriverWait`.

### 1. Implementasi `WaitHelper`

Konfigurasi `WebDriverWait` dipusatkan di `src/SeleniumTutorial.Tests/Utils/WaitHelper.cs` dan langsung dipakai oleh `BasePage`:

```csharp
public static class WaitHelper
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(250);

    public static WebDriverWait Create(IWebDriver driver) => Create(driver, DefaultTimeout);

    public static WebDriverWait Create(IWebDriver driver, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(driver);

        var wait = new WebDriverWait(driver, timeout)
        {
            PollingInterval = DefaultPollingInterval
        };

        // Ignore transient DOM lookup errors while polling for dynamic elements.
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        return wait;
    }
}
```

Selain `Create()`, `WaitHelper` menyediakan helper kondisi tunggu yang umum dipakai pada Page Object:
- `UntilVisible(wait, locator)` — menunggu elemen ada di DOM dan terlihat (`Displayed`).
- `UntilClickable(wait, locator)` — menunggu elemen terlihat dan aktif (`Displayed && Enabled`).
- `UntilInvisible(wait, locator)` — menunggu elemen hilang dari DOM atau tidak lagi tampil.
- `UntilTextPresent(wait, locator, expectedText)` — menunggu teks tertentu muncul pada elemen yang terlihat.
- `UntilUrlContains(wait, urlFragment)` — menunggu URL browser mengandung fragmen tertentu.

### 2. Contoh Penerapan pada Dynamic Loading

Halaman `https://the-internet.herokuapp.com/dynamic_loading` digunakan pada `DynamicLoadingPage` dan `WaitDemoTests` untuk mendemokan dua kasus umum DOM asinkron:
1. **Example 1 (Hidden Element)**: elemen hasil sudah ada di DOM dengan `display: none` dan baru ditampilkan setelah proses loading selesai.
2. **Example 2 (Rendered Element)**: elemen hasil baru ditambahkan ke DOM setelah proses loading (~5 detik) selesai.

Cuplikan implementasi di `src/SeleniumTutorial.Tests/Pages/DynamicLoadingPage.cs`:

```csharp
public DynamicLoadingPage WaitForLoadingToFinish()
{
    WaitHelper.UntilInvisible(Wait, LoadingIndicator);
    return this;
}

public DynamicLoadingPage WaitForResultText(string expectedText)
{
    WaitHelper.UntilTextPresent(Wait, FinishText, expectedText);
    return this;
}
```

Dan pemanggilannya di `src/SeleniumTutorial.Tests/Tests/WaitDemoTests.cs`:

```csharp
[Test]
public void DynamicLoading_Example1HiddenElement_WaitsUntilTextIsVisible()
{
    var page = new DynamicLoadingPage(Driver).Open(1);

    page.StartLoading()
        .WaitForLoadingToFinish()
        .WaitForResultText("Hello World!");

    Assert.That(page.GetResultText(), Is.EqualTo("Hello World!"));
}
```

Untuk menjalankan demo Explicit Wait saja:

```powershell
dotnet test --filter "FullyQualifiedName~WaitDemoTests"
```

### 3. Perbandingan: Explicit Wait vs Implicit Wait vs `Thread.Sleep`

| Aspek | Explicit Wait (`WebDriverWait`) | Implicit Wait (`ImplicitWait`) | `Thread.Sleep(...)` |
|---|---|---|---|
| **Cara kerja** | Melakukan *polling* berkala (mis. tiap 250 ms) sampai kondisi terpenuhi atau timeout | Menunggu secara global hanya saat `FindElement` mencari elemen di DOM | Menghentikan eksekusi thread secara paksa selama durasi tetap |
| **Efisiensi waktu** | **Cepat** — langsung lanjut begitu kondisi terpenuhi | Cukup cepat saat elemen muncul, tapi bisa memperlambat pengecekan elemen yang memang seharusnya tidak ada | **Lambat** — selalu menunggu penuh meskipun elemen sudah siap lebih awal |
| **Kondisi yang bisa ditunggu** | Fleksibel: terlihat, bisa diklik, menghilang, teks muncul, URL berubah | Terbatas: hanya keberadaan elemen di DOM (tidak mengecek *visible* atau *clickable*) | Tidak mengecek kondisi apa pun |
| **Risiko flaky** | **Rendah** (aman untuk elemen dinamis & animasi) | Sedang (bisa bentrok jika dicampur dengan Explicit Wait) | **Tinggi** (gagal jika jaringan sedikit lebih lambat dari durasi sleep) |
| **Status di proyek ini** | **Direkomendasikan** di semua Page Object | Tidak digunakan agar tidak tumpang tindih dengan `WebDriverWait` | **Dilarang** di kode utama (hanya dipakai sebagai contoh anti-pattern di `FailureDemo/`) |

#### Kenapa `Thread.Sleep` dan Implicit Wait dihindari?
- **`Thread.Sleep` adalah anti-pattern**: jika loading butuh 5 detik tapi diisi `Thread.Sleep(2000)`, test pasti gagal; jika diisi `Thread.Sleep(10000)` padahal halaman siap dalam 1 detik, 9 detik terbuang sia-sia di setiap test.
- **Implicit Wait tidak cukup & berisiko jika dicampur**: Implicit Wait hanya menunggu sampai elemen *ada* di DOM (belum tentu terlihat atau siap diklik), dan dokumentasi resmi Selenium memperingatkan agar tidak mencampur Implicit Wait dengan Explicit Wait karena dapat menimbulkan waktu tunggu total yang tidak terduga.
