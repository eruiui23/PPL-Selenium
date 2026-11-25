# Pemetaan Locator — Login & Inventory (A2)

Sumber verifikasi: source code SauceDemo (`saucelabs/sample-app-web`, commit d5cd655),
lalu dicek ulang di DevTools pada https://www.saucedemo.com (25 Nov 2026).

## Prioritas strategi locator

Urutan umum: **Id > Name > CSS > XPath**.

- **Id**: unik per halaman dan paling cepat dicari browser.
- **Name**: biasanya stabil untuk elemen form.
- **CSS**: fleksibel dan cepat. Konvensi kelompok kita adalah
  `By.CssSelector("[data-test='...']")`, karena atribut `data-test` dibuat khusus
  untuk testing dan tidak berubah kalau desain atau class CSS diganti.
- **XPath**: dipakai hanya jika butuh sesuatu yang tidak bisa dilakukan CSS,
  misalnya mencari elemen berdasarkan teks.

Hindari locator yang bergantung pada class tampilan (`btn_primary`, `error`)
atau posisi (`div[3]/span[2]`), karena mudah rusak saat UI berubah.

## Halaman Login (`/`)

| Elemen | data-test | id | name | Lainnya | Locator dipakai |
|---|---|---|---|---|---|
| Input username | `username` | `user-name` | `user-name` | placeholder `Username` | `[data-test='username']` |
| Input password | `password` | `password` | `password` | type `password` | `[data-test='password']` |
| Tombol Login | `login-button` | `login-button` | `login-button` | value `Login` | `[data-test='login-button']` |
| Pesan error* | `error` | – | – | tag `h3` | `[data-test='error']` |
| Tombol tutup error* | `error-button` | – | – | – | `[data-test='error-button']` |
| Kotak daftar user | `login-credentials` | `login_credentials` | – | – | (tidak dipakai di test) |

\* Hanya muncul setelah login gagal.

Catatan: `data-test` username adalah `username`, tetapi `id`/`name`-nya `user-name`.
Ini jebakan yang sering bikin locator salah.

### Pesan error login

| Kondisi | Teks di `[data-test='error']` |
|---|---|
| Username kosong | `Epic sadface: Username is required` |
| Username diisi, password kosong | `Epic sadface: Password is required` |
| Kombinasi salah | `Epic sadface: Username and password do not match any user in this service` |
| `locked_out_user` | `Epic sadface: Sorry, this user has been locked out.` |

User valid: `standard_user`, `locked_out_user`, `problem_user`,
`performance_glitch_user`, `error_user`, `visual_user`. Password: `secret_sauce`.

## Halaman Inventory (`/inventory.html`)

| Elemen | data-test | id | Locator dipakai |
|---|---|---|---|
| Judul "Products" | `title` | – | `[data-test='title']` |
| Container produk | `inventory-container` | `inventory_container` | `[data-test='inventory-container']` |
| Daftar produk | `inventory-list` | – | `[data-test='inventory-list']` |
| Setiap kartu produk (6 buah) | `inventory-item` | – | `[data-test='inventory-item']` |
| Link judul produk | `item-<N>-title-link` | `item_<N>_title_link` | `By.LinkText("<nama produk>")` |
| Nama produk | `inventory-item-name` | – | `[data-test='inventory-item-name']` |
| Harga produk | `inventory-item-price` | – | `[data-test='inventory-item-price']` |
| Tombol Add to cart | `add-to-cart-<slug>` | sama | `[data-test='add-to-cart-sauce-labs-backpack']` |
| Tombol Remove** | `remove-<slug>` | sama | `[data-test='remove-sauce-labs-backpack']` |
| Ikon keranjang | `shopping-cart-link` | – | `[data-test='shopping-cart-link']` |
| Badge jumlah item** | `shopping-cart-badge` | – | `[data-test='shopping-cart-badge']` |
| Dropdown sort | `product-sort-container` | – | `[data-test='product-sort-container']` |
| Teks sort aktif | `active-option` | – | `[data-test='active-option']` |
| Tombol buka menu | `open-menu` (img) | `react-burger-menu-btn`*** | `By.Id("react-burger-menu-btn")` |
| Link All Items (menu) | `inventory-sidebar-link` | `inventory_sidebar_link` | `[data-test='inventory-sidebar-link']` |
| Link Dynamic Catalog (menu) | `dynamic-catalog-sidebar-link` | `dynamic_catalog_sidebar_link` | `[data-test='dynamic-catalog-sidebar-link']` |
| Link About (menu) | `about-sidebar-link` | `about_sidebar_link` | `[data-test='about-sidebar-link']` |
| Link Logout (menu) | `logout-sidebar-link` | `logout_sidebar_link` | `[data-test='logout-sidebar-link']` |
| Link Reset App State (menu) | `reset-sidebar-link` | `reset_sidebar_link` | `[data-test='reset-sidebar-link']` |

\*\* Hanya muncul setelah ada produk di cart.
\*\*\* Tombol ini tidak punya `data-test`. Cek lewat Inspect pada ikon menu.

### Pola `<slug>` nama produk

Nama produk diubah ke huruf kecil, lalu spasi diganti `-`.
Karakter lain seperti titik dan tanda kurung **tetap ada**.

| Nama produk | Slug |
|---|---|
| Sauce Labs Backpack | `sauce-labs-backpack` |
| Sauce Labs Bike Light | `sauce-labs-bike-light` |
| Sauce Labs Bolt T-Shirt | `sauce-labs-bolt-t-shirt` |
| Sauce Labs Fleece Jacket | `sauce-labs-fleece-jacket` |
| Sauce Labs Onesie | `sauce-labs-onesie` |
| Test.allTheThings() T-Shirt (Red) | `test.allthethings()-t-shirt-(red)` |

Slug dengan titik dan kurung tetap aman di CSS selama nilainya di dalam tanda kutip:
`[data-test='add-to-cart-test.allthethings()-t-shirt-(red)']`.
Tanpa tanda kutip, selector itu akan error.

Nilai option sort: `az`, `za`, `lohi`, `hilo` (pakai `SelectElement.SelectByValue`).

## Kandidat demo 6 jenis locator

| Strategi | Contoh | Kenapa cocok |
|---|---|---|
| `By.Id` | `By.Id("user-name")` | Id unik dan tersedia |
| `By.Name` | `By.Name("password")` | Elemen form dengan atribut `name` |
| `By.CssSelector` | `By.CssSelector("[data-test='login-button']")` | Konvensi kelompok |
| `By.XPath` | `By.XPath("//div[@data-test='inventory-item-name' and text()='Sauce Labs Backpack']")` | Mencari berdasarkan teks, hal yang tidak bisa dilakukan CSS |
| `By.ClassName` | `By.ClassName("inventory_item")`, lalu `FindElements` menghasilkan 6 | Menghitung elemen sejenis |
| `By.LinkText` | `By.LinkText("Sauce Labs Backpack")` | Nama produk adalah elemen `<a>` asli dan langsung terlihat |

Hal yang perlu diperhatikan:
- `By.ClassName` hanya menerima **satu** nama class. `By.ClassName("btn btn_primary")`
  akan melempar `InvalidSelectorException`. Pakai CSS `.btn.btn_primary` untuk kasus itu.
- Untuk LinkText, pakai link judul produk, bukan link menu atau footer. Link menu baru
  bisa diklik setelah animasi selesai. Link footer membuka tab baru, dan teksnya pernah
  berubah ("Twitter" menjadi "X").
- Link judul produk membuka halaman detail produk (`/inventory-item.html?id=<N>`).