# PPL-Selenium

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
