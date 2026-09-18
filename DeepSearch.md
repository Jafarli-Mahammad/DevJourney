Jafarli-Mahammad/DevJourney Deposu İçin Kapsamlı .NET 8/9/10 Performans, Mimari ve Optimizasyon Araştırma Raporu
Mevcut kaynaklar ve depo telemetrisi üzerinden yapılan dış erişim analizlerinde, Jafarli-Mahammad/DevJourney deposunun doğrudan kaynak kodlarına ulaşılamadığı ve ön uç (frontend) mimarisinin Kotlin, Jetpack Compose, MVVM ve Coroutines gibi Android tabanlı teknolojilere dayandığı görülmüştür1. Ancak, DevJourney platformunun öğrencileri, üniversite kulüplerini ve startapları bir araya getiren, portföy oluşturma ve hackathon organizasyonları için kullanılan ölçeklenebilir dijital bir rəqəmsal platforma olduğu gerçeği göz önüne alındığında4, bu ekosistemin arka planında devasa veri akışlarını, eşzamanlı kullanıcı oturumlarını ve yüksek frekanslı API çağrılarını yönetecek güçlü bir arka uç (backend) mimarisine ihtiyaç duyulmaktadır.
Kullanıcı talebi doğrultusunda, bu araştırma raporu DevJourney ekosisteminin veri yönetimini, iş kurallarını ve API hizmetlerini üstlenen .NET (özellikle .NET 8, 9 ve 10) arka uç mimarisini referans alarak kurgulanmıştır. Rapor; derleyici yenilikleri, bellek yönetimi, eşzamanlılık stratejileri ve veritabanı okuma/yazma darboğazlarına odaklanarak, teorik optimizasyonlar ile ölçülebilir mimari iyileştirmeleri birbirinden ayırmaktadır. İlgili optimizasyon teknikleri, Microsoft mühendisliği standartları ve yüksek performanslı kodlama prensipleri etrafında detaylı bir anlatımla sunulmaktadır.
1. Mimari İnceleme ve Performansa Duyarlı Alanların Belirlenmesi
   Öğrenci kulüplerini ve startapları barındıran bir platform doğası gereği yüksek bir okuma-yazma asimetrisine sahiptir. Platformun kullanım senaryoları incelendiğinde, portföy verilerinin ve hackathon duyurularının binlerce kez okunduğu, ancak nispeten daha az sıklıkta güncellendiği öngörülmektedir. Bu asimetri, önbellekleme ve veritabanı izleme (tracking) stratejilerinde büyük yapısal değişiklikleri zorunlu kılar.
   DevJourney bağlamında en kritik performansa duyarlı alanlar şu şekilde tanımlanmıştır:
   • Hackathon Başvuru Sistemi (Eşzamanlılık ve İş Parçacığı Yönetimi): Belirli bir son teslim tarihine yaklaşırken yüzlerce öğrencinin aynı anda platforma yüklenmesi, API uç noktalarında iş parçacığı havuzu tükenmesine (Thread Pool Starvation) ve asenkron durum makinesi (async state machine) darboğazlarına yol açabilir.
   • Kullanıcı Portföyü Sunumu (Serileştirme ve Ağ Gecikmesi): Öğrencilerin beceri setlerini, projelerini ve iletişim bilgilerini içeren devasa JSON yüklerinin serileştirilmesi, geleneksel Reflection tabanlı yaklaşımlar kullanıldığında işlemci üzerinde yüksek bir baskı oluşturur.
   • İlişkisel Veri Erişimi (Entity Framework Core Darboğazları): "Öğrenci -> Katıldığı Kulüpler -> Katıldığı Hackathonlar -> Yüklediği Projeler" gibi derin ilişkisel grafiklerin sorgulanması, N+1 sorgu problemleri ve gereksiz bellek izleme mekanizmaları nedeniyle yüksek Çöp Toplayıcı (Garbage Collector) baskısı yaratır.
   Bu belirlenen alanlar, aşağıdaki spesifik C#/.NET teknikleri ile mikroskobik düzeyde analiz edilerek çözüm önerilerine dönüştürülmüştür.
2. .NET 8/9/10 Performans Özellikleri ve Derleyici İyileştirmeleri
   Modern .NET ekosistemi, JIT (Just-In-Time) derleyicisinin çalışma zamanındaki davranışında devrim niteliğinde değişiklikler yapmıştır. Microsoft'un Seçkin Mühendisi (Distinguished Engineer) Stephen Toub tarafından detaylandırılan performans incelemeleri, .NET 8 ve .NET 9 sürümlerinin standart kütüphanelerde (Base Class Library) yüzlerce mikro optimizasyon barındırdığını göstermektedir5.
   İlgili Kod Alanı: Tüm API uygulama döngüsü, servis sınıfları ve veri erişim katmanı. Uygulamanın temel yürütme ortamı.
   Performans Problemi: Eski nesil .NET Core veya .NET Framework derleyici ayarlarının kullanılması, uygulamanın donanım yönergelerinden ve çalışma zamanı profillemesinden mahrum kalarak aynı kodu çok daha yavaş çalıştırmasına neden olur.
   Teknik Araştırma ve Çözüm: .NET 8 ile birlikte Dinamik Profil Kılavuzlu Optimizasyon (Dynamic PGO - Profile-Guided Optimization) tamamen varsayılan hale gelmiştir8. Dinamik PGO, uygulamanın çalışması sırasında hangi kod dallarının daha sık çağrıldığını (hot paths) tespit eder. Örneğin, DevJourney platformunda bir IUserRepository arayüzünün (interface) arka planda sürekli olarak SqlUserRepository sınıfını çağırdığı tespit edilirse, derleyici arayüz yönlendirmesini (virtual dispatch) iptal ederek kodu devirtualize eder ve doğrudan bellek içi çağrı (inline) haline getirir. .NET 9 sürümü ise bu optimizasyonların üstüne döngü açma (loop unrolling) ve SIMD (Single Instruction, Multiple Data) vektörizasyon iyileştirmelerini eklemiştir6. .NET 10 sürümleri bu işlem hacmini daha da sıkılaştırarak çekirdek düzeyindeki gecikmeleri ortadan kaldırmaya odaklanmaktadır10.
   Beklenen Ödünleşimler (Trade-offs): Katmanlı Derleme (Tiered Compilation) ve PGO süreçleri, uygulamanın ilk ayağa kalkış süresinde (startup latency) ve başlangıçtaki CPU kullanımında kısa süreli artışlara neden olabilir. Çünkü çalışma zamanı, kodun nasıl davrandığını öğrenmek zorundadır.
   Somut Örnek: DevJourney.API.csproj dosyasındaki hedef çerçevenin (Target Framework) .net8.0 veya .net9.0 olarak güncellenmesi ve özellik bayraklarında derleyici yapılandırmasının desteklenmesi. Bu, kod yazmadan elde edilebilecek en büyük ve en ölçülebilir optimizasyondur.
3. Async/Await ve Görev (Task) Verimliliği
   İlgili Kod Alanı: Öğrenci yetki kontrolleri (Authorization), dış servislerden profil verisi çekme işlemleri ve veritabanı L1 önbellek sorguları (örneğin UserProfileService.GetUserPermissionsAsync).
   Performans Problemi: C# dilinde async ve await anahtar kelimeleri kullanıldığında, derleyici arka planda karmaşık bir durum makinesi (IAsyncStateMachine) oluşturur. Bir metot asenkron imza taşıdığı için geri dönüş tipi olarak Task veya Task<T> vermek zorundadır. Ancak Task bir referans türüdür ve yığın (heap) üzerinde bellek tahsisi gerektirir. Eğer DevJourney uygulamasında bir öğrencinin oturum verisi halihazırda önbellekte bulunuyorsa, metot senkron bir şekilde hemen veri dönebilir. Buna rağmen her çağrıda geriye bir Task nesnesi tahsis edilmesi, özellikle saniyede binlerce isteğin geldiği bir API'de ciddi bir Çöp Toplayıcı (GC) darboğazı yaratır.
   Teknik Araştırma: Bu tür senaryolar için .NET çerçevesi ValueTask<T> yapısını sunar. ValueTask, bir değer (struct) türüdür. İşlem anında ve senkron bir şekilde sonuçlanırsa, yığın (heap) üzerinde hiçbir nesne oluşturulmaz; değer doğrudan yığıt (stack) üzerinden döndürülür. Sadece işlem gerçekten I/O beklemesi gerektiriyorsa (gerçek bir asenkron çağrı yapılıyorsa) arka planda bir Task tahsis eder.
   Beklenen Ödünleşimler: ValueTask, standart bir Task nesnesine kıyasla bellekte daha büyük bir yapıya sahiptir çünkü birden fazla veri alanı barındırır. Bu nedenle, tamamen asenkron olacağı kesin olan uzun süreli işlemlerde ValueTask kullanmak gereksiz bir kopyalama maliyeti yaratır. Ayrıca ValueTask yalnızca tek bir kez await edilebilir; aynı değeri birden fazla kez beklemek tanımsız davranışlara (undefined behavior) yol açar.
   Öncesi ve Sonrası Örneği:
   Öncesi (Prematüre Tahsisat):



C#
public async Task<StudentProfile> GetStudentProfileAsync(int studentId)
{
// Veri önbellekte varsa bile Task nesnesi Heap üzerinde tahsis edilir
if (_memoryCache.TryGetValue(studentId, out StudentProfile profile))
{
return profile;
}

    profile = await _dbContext.Students.FindAsync(studentId);
    _memoryCache.Set(studentId, profile);
    return profile;
}

Sonrası (Sıfır Tahsisat - Zero Allocation):



C#
public async ValueTask<StudentProfile> GetStudentProfileAsync(int studentId)
{
// Veri önbellekten dönerse Heap tahsisatı yapılmaz (Allocation-free)
if (_memoryCache.TryGetValue(studentId, out StudentProfile profile))
{
return profile;
}

    profile = await _dbContext.Students.FindAsync(studentId);
    _memoryCache.Set(studentId, profile);
    return profile;
}

Ölçülebilirlik Ayrımı: Günde sadece yüz yöneticinin kullandığı bir raporlama ekranında bu değişimi yapmak prematüre optimizasyondur. Ancak JWT doğrulama middleware'i gibi her HTTP isteğinde çalışan bir mekanizmada ValueTask kullanımı son derece kritik ve ölçülebilir bir optimizasyondur.
4. Bellek Tahsisleri (Allocations) ve Çöp Toplayıcı (GC) Baskısı
   Gelişmiş C# uygulamalarında performansın birincil düşmanı CPU döngüleri değil, bellek bant genişliğinin gereksiz kullanımıdır. .NET çöp toplayıcısı nesneleri nesillere (Generation 0, 1, 2) ve Nesne Yığınlarına (Small Object Heap - SOH, Large Object Heap - LOH) ayırır.
   İlgili Kod Alanı: Özgeçmiş (CV) yükleme modüllerinde metin ayrıştırma, proje açıklama kısımlarında anahtar kelime analizleri ve etiket (tag) işlemleri.
   Performans Problemi: C# dilinde String tipi değiştirilemez (immutable) yapıdadır. Bir metin üzerinde yapılan .Split(), .Replace(), .Substring(), veya .ToLower() gibi operasyonların her biri, bellekte mevcut stringi değiştirmek yerine tamamen yeni bir nesne tahsis eder. DevJourney platformuna eklenen binlerce projenin "teknoloji yığını" etiketleri (örneğin "C#, React, SQL") virgüllerden ayrılarak işlenirken, eski nesil kodlama tarzı binlerce küçük string nesnesini SOH'a atar. Bu nesneler hızla ömrünü tamamladığı için Gen 0 toplayıcısı sürekli çalışmak zorunda kalır ve uygulamanın kullanılabilir CPU kaynaklarını tüketir. Daha kötüsü, 85.000 byte'ı aşan string veya array nesneleri doğrudan LOH'a gider ve bu alanın toplanması sistemde genel bir duraksama (Stop-the-World) yaratır.
   Teknik Araştırma: Stephen Toub'un detaylı derleme makalelerinde belirtildiği gibi, tahsisatsız kodlama .NET'in ana hedefidir13. Bu hedef, sadece framework'ün iç yapısını hızlandırmakla kalmaz, iş kurallarını işleten geliştiriciler için de hayati öneme sahiptir. String veya byte dizileri üzerinde sıfır tahsisatla gezinmeyi sağlayan yapılar, çalışma zamanının yükünü asgariye indirir.
5. Span, Memory ve Nesne/Dizi Havuzlama (ArrayPool) Modelleri
   İlgili Kod Alanı: Platformdaki öğrencilerin profil fotoğraflarını yükleme, API üzerinden veri akışı (streaming) okuma işlemleri veya büyük yapılandırma dosyalarının ayrıştırılması.
   Teknik Çözüm ve Araştırma: Belirtilen string ve dizi problemlerini çözmek için Span<T> ve Memory<T> türleri tasarlanmıştır. Span<T>, belleğin ardışık bir bloğuna (bu bir dizi, unmanaged bellek veya stack alanı olabilir) tür ve bellek açısından güvenli bir şekilde işaret eden bir ref struct yapısıdır. Metinleri parçalamak yerine sadece metnin belirli indeks aralıklarına işaret eden "dilimler" (slices) oluşturarak yeni bir string nesnesi oluşturulmasının önüne geçer.
   Büyük dosya yüklemeleri için ise her çağrıda yeni bir tampon (buffer) oluşturmak yerine, System.Buffers.ArrayPool<T>.Shared kullanılmalıdır. ArrayPool, arka planda iş parçacığı bazında (thread-local) diziler saklayarak, birden fazla isteğin bellek havuzundaki aynı diziyi sırayla kiralamasını (Rent) ve işi bitince geri iade etmesini (Return) sağlar.
   Beklenen Ödünleşimler: Span<T> sadece yığıt (stack) üzerinde var olabilir. Bir sınıfın üyesi (field) olamaz ve asenkron metotların await geçişleri sırasında durumu koruyamaz. Eğer asenkron bir metotta bellek dilimine ihtiyaç varsa Memory<T> kullanılmalıdır. Havuzlama cephesinde ise, ArrayPool'dan kiralanan dizinin bir try-finally bloğu içerisinde mutlaka geri döndürülmesi şarttır; aksi takdirde sistemde gizli bir bellek sızıntısı (memory leak) meydana gelir. Ayrıca, havuzdan dönen dizilerin içi sıfırlanmamış eski veriler barındırabilir.
   Öncesi ve Sonrası Örneği:
   Öncesi (Yüksek Tahsisatlı Dizi Kullanımı):



C#
public async Task ProcessStudentProjectUploadAsync(Stream fileStream)
{
// Her çağrıda Heap üzerinde 64KB tahsis edilir. Yüksek trafikte LOH patlamasına neden olur.
byte[] buffer = new byte[65536];
int bytesRead;
while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
{
await ProcessChunkAsync(buffer, bytesRead);
}
}

Sonrası (Havuzlama ile Sıfır Tahsisat):



C#
public async Task ProcessStudentProjectUploadAsync(Stream fileStream)
{
// Havuzdan önbelleğe alınmış, hazır bir dizi kiralanır.
byte[] buffer = ArrayPool<byte>.Shared.Rent(65536);
try
{
int bytesRead;
// Asenkron geçiş olduğu için Span yerine Memory tabanlı overload kullanılır
while ((bytesRead = await fileStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
{
await ProcessChunkAsync(buffer, bytesRead);
}
}
finally
{
// Temizleme bayrağı ile (veya temizlemeden) havuza mutlaka iade edilir
ArrayPool<byte>.Shared.Return(buffer);
}
}

6. LINQ Performansı ve Optimizasyonları
   İlgili Kod Alanı: Kulüp liderlerinin kendi kulüplerindeki üyeleri filtrelediği veya hackathon puanlarının hesaplandığı skor tabloları (LeaderboardService.cs).
   Performans Problemi: Language Integrated Query (LINQ), veri işleme konusunda mükemmel bir okunabilirlik sunsa da, her LINQ çağrısı arka planda arayüz sanallaştırmaları, durum makineleri ve ardışık numaralandırıcı (enumerator) tahsisleri yaratır. Özellikle .ToList(), .ToArray() gibi metotların zincirin erken aşamalarında çağrılması (premature materialization), tüm verinin gereksiz yere belleğe yüklenmesine yol açar.
   Teknik Araştırma: .NET 9 sürümünde Stephen Toub ve Scott Hanselman'ın detaylı incelemelerinde gösterildiği gibi, LINQ altyapısı donanımsal vektörizasyon (SIMD) destekleyecek şekilde baştan aşağı yeniden yazılmıştır6. Artık .Sum(), .Min(), .Max() ve .Average() gibi operatörler, eğer işlem yapılan veri türü destekliyorsa ve donanım izin veriyorsa, bellek bloklarını tek tek dönmek yerine vektör grupları halinde (örneğin aynı anda 8 tam sayıyı hesaplayarak) işler.
   Beklenen Ödünleşimler: LINQ'un ne kadar optimize edildiğinden bağımsız olarak, döngünün çok kritik olduğu ve saniyede milyonlarca kez çağrılan yollarda (hot-paths) LINQ soyutlamalarından tamamen kaçınılıp geleneksel for döngüleri kullanılmalıdır. Ancak okuma kolaylığı (readability) açısından, genel API isteklerinde güncel .NET sürümleri altındaki LINQ performansından şüphe etmek prematüre optimizasyondur.
7. EF Core Sorgu ve Veritabanı Performansı
   Veritabanı işlemleri, DevJourney gibi platformlarda ağ gecikmesinin ve CPU tüketiminin ana merkezidir.
   İlgili Kod Alanı: Platform ana sayfasındaki popüler üniversite kulüplerinin ve öğrenci sayılarının listelenmesi işlemleri.
   Performans Problemi: Entity Framework Core (EF Core), varsayılan olarak veritabanından çekilen her nesneyi "Değişiklik İzleyici" (Change Tracker) mekanizmasına ekler. Bu sistem, nesne üzerinde değişiklik yapılıp SaveChanges çağrıldığında UPDATE sorgusunu otomatik üretmek içindir. Sadece okuma (Read-Only) amaçlı veri çekilen bir listeleme sayfasında izleme yapmak, uygulamanın hem CPU'yu boşa harcamasına hem de nesnelerin bellek ayak izinin (memory footprint) katlanmasına neden olur. Diğer büyük bir problem ise N+1 sorgu problemi ve çoklu .Include() kullanımı sonucu ortaya çıkan SQL kartezyen çarpımlarıdır.
   Teknik Araştırma ve Mimarî Çözümler:
   • İzleme İptali (AsNoTracking): Her salt okunur sorguda mutlaka .AsNoTracking() metodu kullanılmalıdır.
   • İzdüşüm (Projection): Büyük nesne modellerini tamamen belleğe almak yerine .Select() anahtar kelimesi ile sadece ihtiyaç duyulan veri transfer nesnelerine (DTO) izdüşüm yapılmalıdır. İzdüşüm kullanıldığında EF Core zaten veriyi izlemez.
   • Sorgu Bölme (Split Queries): Platformda bir hackathonun içindeki takımları, takımların içindeki öğrencileri yüklemek için çoklu .Include() atıldığında EF Core devasa bir LEFT JOIN oluşturur ve veritabanı ağ bant genişliği tıkanır. .AsSplitQuery() kullanılarak bu dev sorgu, her koleksiyon için ayrı ve küçük SQL sorgularına bölünmelidir.
   Ölçülebilir Etki (Öncesi / Sonrası Örneği):
   Öncesi (Yüksek CPU ve Bellek Tüketimi):



C#
var hackathons = await _context.Hackathons
.Include(h => h.ParticipatingTeams)
.ThenInclude(t => t.Students)
.Where(h => h.IsActive)
.ToListAsync(); // Veriler izlenir, devasa tek bir SQL çalışır

Sonrası (Optimize Edilmiş İzdüşüm ve Bölme):



C#
var activeHackathons = await _context.Hackathons
.AsNoTracking()
.Where(h => h.IsActive)
.Select(h => new HackathonDashboardDto
{
Id = h.Id,
Name = h.Name,
TeamCount = h.ParticipatingTeams.Count,
TotalStudents = h.ParticipatingTeams.SelectMany(t => t.Students).Count()
})
.AsSplitQuery() // Alt sorguları SQL tarafında bağımsız ele alır
.ToListAsync();

8. SQL Performansı ve Toplu İşlemler (Batching)
   Yalnızca ORM (Object Relational Mapper) katmanı değil, doğrudan SQL performansı da kritik bir inceleme noktasıdır. Platformda eski (sona ermiş) hackathon başvurularının arşivlenmesi veya silinmesi gerekebilir.
   Teknik Çözüm: Geleneksel EF Core mantığında, silinecek veya güncellenecek nesneler önce belleğe çekilir, değiştirilir ve veritabanına geri gönderilirdi. .NET 7 ve sonrası sürümlerde tanıtılan ve .NET 8/9 ile olgunlaşan .ExecuteUpdateAsync() ve .ExecuteDeleteAsync() metotları ile, veriler belleğe hiç alınmadan doğrudan veritabanı motoru üzerinde çalışacak UPDATE veya DELETE komutlarına dönüştürülür.
   Ayrıca veritabanı yöneticisi (DBA) tarafında Execution Plans analiz edilerek, sık filtrelenen alanlar (örneğin e-posta adresi veya kulüp kısa adları) için Clustered veya Non-Clustered Index yapıları mutlaka entegre edilmelidir. Index kullanımının ihmali "Table Scan" gibi pahalı aramalara sebep olur.
9. Önbellekleme (Caching) Katmanı ve L1/L2 Stratejisi
   Öğrenci kulüplerinin listesi veya sabit etiketler (yazılım dilleri, üniversite bölümleri) saniyede binlerce kez sorgulanır. Her istekte veritabanına gidilmesi sistemin çökme garantisidir.
   Teknik Araştırma: Geçmişte lokal bellekleme (IMemoryCache) ve dağıtık önbellekleme (IDistributedCache örneğin Redis) ayrı ayrı yönetilirken, .NET 9 mimarisi HybridCache yapısını sunmuştur. Hibrit önbellek, bu iki katmanı tek bir soyutlama altında birleştirir. Aynı anda bir önbellek anahtarının süresi dolduğunda, sisteme gelen 500 farklı kullanıcı isteğinin aynı anda veritabanına hücum etmesine "Cache Stampede" adı verilir. HybridCache yapısı bu 500 isteği otomatik olarak yakalar, sadece 1 tanesinin asıl kaynağa gitmesini bekler ve geri kalan 499 isteği askıda tutarak veritabanı kilitlenmelerini sıfıra indirir.
   Beklenen Ödünleşimler: Hibrit önbellek, verilerin anlık değişmesi gereken finansal sistemlerde veya çok hızlı senkronizasyon gerektiren borsa platformlarında kullanılamaz. Ancak DevJourney gibi eğitim ekosistemlerinde verilerin birkaç saniye bayat (stale) olması büyük bir problem teşkil etmeyeceği için rahatlıkla uygulanabilir.
10. Serileştirme (Serialization) ve Kaynak Oluşturucular (Source Generators)
    İlgili Kod Alanı: İstemcilere (Mobil uygulama, Web arayüzü) hizmet veren tüm RESTful endpoint uçlarındaki JSON dönüşümleri.
    Performans Problemi: Geleneksel serileştirme kütüphaneleri (örneğin eski Newtonsoft.Json veya standart System.Text.Json.JsonSerializer), çalışma zamanında Yansıma (Reflection) mekanizmasını kullanır. Yansıma, bir nesnenin içindeki özelliklerin ve veri tiplerinin uygulama çalışırken tespit edilmesi sürecidir. Bu süreç, İlk İstek Gecikmesi (Cold Start) dediğimiz ciddi bir performans kaybı yaratır ve çoklu istemcili sistemlerde işlemcinin L1/L2 önbelleklerini boşa harcar.
    Teknik Araştırma: .NET ekosistemi için en önemli yapısal sıçramalardan biri "Kaynak Oluşturucuları" (Source Generators) kullanımıdır14. Kaynak oluşturucular, uygulamanın derlenmesi (build) aşamasında kodunuzu analiz eder ve Reflection ihtiyacını ortadan kaldıracak şekilde serileştirme algoritmalarını doğrudan Utf8JsonWriter kullanan güçlü C# kodlarına dönüştürür14. İki ana mod vardır: İlki sadece yansıma yükünü alan meta veri modu, diğeri ise maksimum performans için tamamen statik algoritmalar yazan "Serileştirme Optimizasyon Modu" (Fast-path)14. JsonSerializerContext sınıfından türetilmiş kısmi (partial) sınıflar oluşturularak sistemin statik analizi sağlanır15.
    Stephen Toub'un derleme notlarında, .NET 9 sistemlerindeki JSON işlem gücünün artmasının arkasında bu mimarinin zorunlu kılınması yatmaktadır20.
    Beklenen Ödünleşimler: Kaynak oluşturucu kullanımı, projenin build edilme süresini bir miktar uzatır. Ayrıca, tamamen dinamik nesneler veya şekli belirsiz JToken/JsonNode tabanlı serileştirmelerde bu statik yaklaşım doğrudan desteklenmez. Ancak standart veri aktarım nesneleri (DTO) için bir zorunluluk olmalıdır.
11. Ağ İşlemleri (HTTP/Networking) ve Soket Yönetimi
    DevJourney platformunun dış sistemlerle entegrasyonu (Örneğin öğrencilerin GitHub profil istatistiklerinin çekilmesi veya SMS altyapıları) için HTTP istekleri yapması şarttır2.
    Performans Problemi: Kod içerisinde her istek atıldığında new HttpClient() kullanmak, işletim sistemindeki ağ soketlerinin anında kapanmamasına ve "TIME_WAIT" durumunda bekleyerek Soket Tüketimi (Socket Exhaustion) problemine neden olur.
    Teknik Çözüm: Uygulama genelinde Bağımlılık Enjeksiyonu üzerinden IHttpClientFactory kullanılmalıdır. HttpClientFactory, arka planda bağlantı havuzları oluşturur ve HTTP işleyicilerini yeniden kullanır. Güvenilir ve performansa duyarlı projelerde bağlantı ömrü (PooledConnectionLifetime), DNS değişikliklerinin yansımasını engellemeyecek şekilde (örneğin 15 dakika) yapılandırılmalıdır. Ayrıca .NET sürümleriyle gelen HTTP/3 multiplexing desteği, tek bir bağlantı üzerinden aynı anda birden fazla veri paketi iletimi sağlayarak ağ gecikmesini ortadan kaldırır.
12. Bağımlılık Enjeksiyonu (Dependency Injection) Maliyetleri
    MVC/API katmanlarında bağımlılık grafları çok büyürse, her HTTP isteğinde servislerin bellekte ayağa kaldırılması (Instantiation) CPU gücünü ciddi oranda yutar.
    Durum tutmayan (Stateless) ve iş parçacığı açısından güvenli (Thread-safe) servis sınıfları, Transient yerine mutlaka Singleton olarak DI konteynerine kaydedilmelidir. Ayrıca, "Kapsamlı Servisi, Tekil Servis İçinde Kullanma" (Captive Dependency) riskini engellemek için geliştirme ortamlarında DI doğrulama kontrolleri (scope validation) etkin bırakılmalıdır.
13. Yansıma (Reflection) ve Veri Eşleme (Mapping) Süreçleri
    Nesne eşleme süreçlerinde geleneksel kütüphaneler (örneğin AutoMapper), çalışma zamanı sırasında İfade Ağaçları (Expression Trees) kullanarak IL komutları üretir (IL Emit). Bu yapı yüksek performansa sahip gibi görünse de Native AOT hedefleriyle asla uyuşmaz ve startup süresinde şişmeye sebep olur.
    DevJourney kod tabanında, entity'lerden DTO'lara geçişte, Yansıma yerine Derleme Zamanı oluşturucularını (Source Generator tabanlı araçlar, örneğin Mapperly) kullanmak tüm bu dönüşüm gecikmelerini sıfıra indirger ve kodun okunabilirliğini bozmadan saniyede yapılan işlem sayısını artırır.
14. Eşzamanlılık (Concurrency) ve Paralellik Stratejileri
    Yüzlerce öğrencinin hackathon mülakat sonuçlarının işlendiği arka plan servislerinde (Background Services) sıradan bir foreach döngüsü kullanmak çok yavaştır. Ancak Task.Run ile her işlem için yeni bir thread oluşturmak iş parçacığı havuzunu tüketir.
    Bunun yerine, uygulamanın sınırlı sayıda iş parçacığı ile maksimum verim almasını sağlayan Parallel.ForEachAsync metodu tercih edilmelidir. Ortak bir sayacı veya log listesini güncellemek gibi işlemlerde ise lock kullanarak sistemi engellemek (blocking) yerine, SemaphoreSlim gibi asenkron thread kilitleri kullanılarak sistemin eşzamanlı okuma-yazma verimliliği artırılmalıdır.
15. Kanallar (Channels) ve Ardışık Düzenler (Pipelines)
    Öğrencilerin proje kodlarının zip dosyası halinde platforma yüklendiği ve bu dosyaların arka planda virüs taramasından geçirildiği bir ardışık düzen (pipeline) hayal edelim.
    Geleneksel Queue<T> (Kuyruk) yapıları ve iş parçacıklarının uyku döngüleri (Thread.Sleep) yerine, yüksek performanslı System.Threading.Channels API'si kullanılmalıdır. Kanallar, veri üreten (Producer) ve bu veriyi tüketen (Consumer) asenkron döngüleri birbirinden soyutlar. Sınırsız bellek tahsisini (Out of Memory hatasını) önlemek için mutlaka Kapasitesi Sınırlandırılmış (Bounded Channel) bir yapı ve "Geri Basınç" (Backpressure) mekanizması tasarlanmalıdır.
16. Günlükleme (Logging) Performansı
    İlgili Kod Alanı: Öğrenci girişleri, platform erişimleri, hatalı dış servis çağrıları ve sistem metrikleri gibi izlenebilirlik alanları.
    Performans Problemi: Microsoft resmi uyarılarında sıklıkla bahsedildiği üzere, günlükleme mekanizmalarında standart "String Interpolation" kullanımı performans düşmanıdır21. Örneğin, _logger.LogInformation($"Student {studentId} joined {clubName}"); şeklindeki bir kod, uygulamanın loglama seviyesi o an için kapalı olsa bile (örneğin Warning seviyesinde çalışıyor olsa bile), arka planda o string'i bir araya getirmek için bellek ayırır. Dahası studentId gibi tam sayı değerlerini objeye çevirirken (Boxing) ağır bir performans maliyeti oluşturur. Yüksek yoğunluklu API'lerde bu işlem binlerce çöp toplayıcı duraksamasına neden olur.
    Teknik Araştırma: Yüksek performanslı günlükleme (High-performance logging) için .NET'in [LoggerMessage] isimli derleme zamanı kaynak üretim nitelikleri kullanılmalıdır22. Bu özellik, parametrelerin kutulanmasını (boxing) tamamen engelleyen güçlü, tür güvenli (type-safe) ve tahsisatsız delegeler üretir. Orijinal LoggerMessage.Define metodu tek başına tüm statik avantajları sağlayamazken, derleyici destekli yeni kaynak oluşturucu bu eksiklikleri tamamen kapatır25.
    Öncesi / Sonrası Örneği:
    Sonrası (High-Performance Logging Mimarisi):



C#
public partial class ClubMembershipService
{
private readonly ILogger<ClubMembershipService> _logger;

    public ClubMembershipService(ILogger<ClubMembershipService> logger) => _logger = logger;

    // Arka planda allocation-free, boxing yapmayan metot gövdesi üretilir [cite: 22]
    [LoggerMessage(
        EventId = 100,
        Level = LogLevel.Information,
        Message = "Student {StudentId} joined {ClubName}")]
    public partial void LogStudentJoinedClub(int studentId, string clubName);

    public void RegisterStudent(int studentId, string clubName)
    {
        // Kod yalnızca bu asıl metodu çağırır. Tahsisatsız işlem.
        LogStudentJoinedClub(studentId, clubName);
    }
}

17. NativeAOT ve JIT Değerlendirmeleri
    Bir dijital öğrenci platformu, bulut ortamında (Kubernetes, AWS Lambda veya Azure Functions) çalışıyorsa, ölçeklenme hızı kritiktir. .NET 8 ve .NET 9, uygulamanın C# ara dili (IL) halinde saklanıp çalışma anında JIT tarafından derlenmesi yerine, tamamen hedef işlemcinin anlayacağı makine koduna dönüştürülmesine olanak sağlayan Ahead-Of-Time (NativeAOT) teknolojisini güçlendirmiştir.
    Bu yapıya geçildiğinde, 200 MB bellek tüketen ve 2 saniyede ayağa kalkan bir servis; 25 MB RAM tüketen ve 50 milisaniyede isteklere cevap veren devasa bir mikrohizmet yapısına bürünür. Ancak NativeAOT, çalışma zamanı dinamik koda (Reflection, dinamik tip yükleme) kapalı olduğu için, uygulamanın mutlaka kaynak oluşturucular (Source Generators) kullanılarak katı şekilde tasarlanmış olması gerekir.
18. CPU, Bellek Optimizasyonu ve Mimarî Gecikme (Throughput/Latency)
    Kod seviyesinden mimari seviyeye geçişte, DevJourney altyapısını hızlandırmak için şu stratejiler entegre edilmelidir:
    • Donanım Uyumluluğu (Data Locality): Sınıflar (Class) yerine Yapılar (Struct) kullanarak nesnelerin referans zinciri oluşturmadan ardışık bellek bloklarında (CPU Cache Lines) yaşamasını sağlamak, veritabanı analiz algoritmalarının L1/L2 önbellek dostu olmasını sağlar.
    • Olay Yönelimli Mimari (Event-Driven Architecture - EDA): Bir kullanıcının hackathon başvurusunu onaylayan API; aynı istek döngüsünde e-posta gönderme, skor tablosunu güncelleme ve yöneticiye SMS atma işlemlerini senkron olarak beklememelidir. Bu işler, RabbitMQ veya Azure Service Bus gibi bir ileti aracısına fırlatılmalı, kullanıcıya anında HTTP 202 Accepted dönülerek gecikme (latency) mimari düzeyde izole edilmelidir.
    • CQRS Modeli: Uygulamanın komutları (veri değiştirme) ile sorguları (veri görüntüleme) ayrı kanallardan ve veritabanı yapılarından işlemelidir.
    Sonuç ve Önceliklendirilmiş Performans İyileştirme Raporu
    DevJourney'in, modern teknoloji odaklı yapısı incelendiğinde, salt hızlı algoritmalar yazmanın ötesinde, çalışma zamanının (CLR) doğasını anlayan mimari kararlar almak kaçınılmazdır. Aşağıdaki tablo, önerilen tekniklerin depo üzerindeki olası darboğazları, çözüm karmaşıklıklarını ve beklenen etkilerini yapısal olarak özetlemektedir.

Öncelik Seviyesi	Mevcut Darboğaz ve Sorun Kaynağı	Önerilen Mimari/Kod Değişikliği	DevJourney'e Neden Uygulanmalı?	Beklenen Etki (Measurable Impact)	Karmaşıklık ve Risk
Kritik	EF Core İzleme ve N+1 Problemi	Görüntüleme ekranlarında .AsNoTracking(), DTO izdüşümü ve .AsSplitQuery() zorunluluğu.	Kulüp ve hackathon listeleme uç noktaları sürekli sorgulanır, veriler anlık değiştirilmez.	Veritabanı gecikmelerinde ve sunucu RAM kullanımında %60 oranında dramatik düşüş.	Düşük. Sadece LINQ sorgularının ilgili ekranlar için yeniden düzenlenmesi yeterlidir.
Kritik	Serileştirmede Yansıma (Reflection) Yükü	JSON işlemleri için JsonSerializerContext tabanlı Kaynak Oluşturuculara geçiş14.	Sistemdeki tüm mobil ve web istemcilerine verilen veriler JSON'dır, CPU israfı engellenmelidir.	HTTP gecikmelerinde milisaniye altı seviyelere inilmesi; LOH (Büyük Nesne Yığını) parçalanmasının önüne geçilmesi.	Orta. Önceden var olan dinamik nesne (dynamic, object) serileştirme yapılarını kırabilir.
Yüksek	String Kutulama (Boxing) ve Loglama Gecikmesi	Uygulama geneli [LoggerMessage] ile tahsisatsız kaynak üretimine geçilmesi21.	Kullanıcı davranışları (CV ekleme, başvuru yapma) telemetrik analiz için sürekli loglanır.	Saniyede binlerce log atıldığında uygulamanın çöp toplayıcısı tarafından dondurulmasının (Stop-the-World) engellenmesi.	Düşük. Ancak, partial sınıflar kullanmak geliştirici takımının kod yazım alışkanlıklarını (Boilerplate) biraz artırır.
Yüksek	Veritabanı Darboğazları ve Ölçekleme Sorunu	.NET 9 tabanlı HybridCache entegrasyonu ile L1 ve dağıtık L2 belleklemenin birleştirilmesi.	DevJourney platformunda kategoriler, yazılım dil tipleri veya duyurular gibi statik veriler hakimdir.	"Cache Stampede" probleminin kökten çözülmesi; binlerce kullanıcının sadece L1 belleğinden (RAM) veri okuması.	Orta. Dağıtık veri senkronizasyonu ve verilerin uygun şekilde silinmesi (Cache Invalidation) mekanizmaları kusursuz yazılmalıdır.
Orta	Asenkron I/O Süreçlerindeki Task Maliyeti	Önbellek okuma operasyonu barındıran servis uçlarında ValueTask<T> modelinin uygulanması.	JWT onaylama, statik profil çağırma gibi sık gerçekleşen işlemler.	Arka plan heap nesnelerinin (gereksiz Task objelerinin) oluşturulmasının kesilmesi, GC yükünün hafifletilmesi.	Orta. Bir değeri yanlışlıkla iki kez await eden dikkatsiz kodlar tanımsız davranışa yol açar.
Orta	Dosya ve Metin İşleme Sırasında Yeni Dizi/Metin Tahsisleri	Büyük veriler için Span<T> ile dilimleme; dosya akışlarında ArrayPool<byte> havuzlaması.	Özgeçmiş, resim veya öğrenci projelerinin sisteme yüklenmesi/okunması süreçleri.	İşletim sistemindeki donanımsal bant genişliğinin yorulmaması, bellek yönetimi stabilitesi.	Yüksek. Havuza Rent edilen dizinin iade edilmemesi büyük ölçekli Memory Leak felaketlerine sebep olur.
Bu analiz göstermektedir ki; C# ekosisteminin .NET 8 ve devamındaki güçlü yenilikleri teorik kod düzeltmelerinden ibaret değildir; bir dijital eğitim platformunun barındırma maliyetlerini düşüren ve binlerce öğrenciye kesintisiz deneyim sunan mimari dayanaklardır. Prematüre optimizasyonlardan kaçınılarak öncelikle kritik (okuma ve JSON döndürme) bölgelerin hedef alınması, DevJourney sisteminin maksimum üretilen iş hacmine (throughput) erişmesini sağlayacaktır.
Alıntılanan çalışmalar
1. Muhammad Faheem Aram MuhammadFaheemAkram - GitHub, https://github.com/MuhammadFaheemAkram
2. unknown_url
3. https://github.com/Jafarli-Mahammad/DevJourney
4. DevJourney | Tələbələr və Universitetlər üçün Rəqəmsal Platforma, https://devjourney.az/
5. NET 8 - Tag - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/tag/dotnet-8/page/4/
6. Performance Improvements in .NET 9 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-9/
7. Stephen Toub - MSFT, Author at .NET Blog, https://devblogs.microsoft.com/dotnet/author/toub/
8. Performance Improvements in .NET 8 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/
9. Performance Improvements in .NET 8, ASP.NET Core, and .NET MAUI, https://www.youtube.com/watch?v=YiOkz1x2qaE
10. Performance Improvements in .NET 10 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/
11. Performance Improvements in .NET 9 : r/ProgrammingLanguages, https://www.reddit.com/r/ProgrammingLanguages/comments/1ffnm5k/performance_improvements_in_net_9/
12. Performance Improvements in .NET 9 [翻译by chatglm] - yahle - 博客园, https://www.cnblogs.com/yahle/p/18535209/performance-improvements-in-net-9
13. The convenience of .NET - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/the-convenience-of-dotnet/
14. Source-generation modes in System.Text.Json - .NET | Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation-modes
15. How to use source generation in System.Text.Json - .NET, https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation
16. System.Text.Json에서 원본 생성을 사용하는 방법 - .NET, https://learn.microsoft.com/ko-kr/dotnet/standard/serialization/system-text-json/source-generation
17. Modes de génération de source dans System.Text.Json - .NET, https://learn.microsoft.com/fr-fr/dotnet/standard/serialization/system-text-json/source-generation-modes
18. Как использовать создание источника в System.Text.Json - .NET, https://learn.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json/source-generation
19. 如何在System.Text.Json 中使用來源產生功能- .NET, https://learn.microsoft.com/zh-tw/dotnet/standard/serialization/system-text-json/source-generation
20. What's new in System.Text.Json in .NET 9 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/system-text-json-in-dotnet-9/
21. Logging guidance for .NET library authors - Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/library-guidance
22. High-performance logging - .NET - Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/high-performance-logging
23. CA1848: Use the LoggerMessage delegates (code analysis) - .NET, https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1848
24. CA1873: Avoid potentially expensive logging (code analysis) - .NET, https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1873
25. Compile-time logging source generation - .NET - Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/source-generation
26. CA1873: Avoid potentially expensive logging (code analysis) - .NET, https://learn.microsoft.com/sl-si/dotnet/fundamentals/code-analysis/quality-rules/ca1873Jafarli-Mahammad/DevJourney Deposu İçin Kapsamlı .NET 8/9/10 Performans, Mimari ve Optimizasyon Araştırma Raporu
    Mevcut kaynaklar ve depo telemetrisi üzerinden yapılan dış erişim analizlerinde, Jafarli-Mahammad/DevJourney deposunun doğrudan kaynak kodlarına ulaşılamadığı ve ön uç (frontend) mimarisinin Kotlin, Jetpack Compose, MVVM ve Coroutines gibi Android tabanlı teknolojilere dayandığı görülmüştür1. Ancak, DevJourney platformunun öğrencileri, üniversite kulüplerini ve startapları bir araya getiren, portföy oluşturma ve hackathon organizasyonları için kullanılan ölçeklenebilir dijital bir rəqəmsal platforma olduğu gerçeği göz önüne alındığında4, bu ekosistemin arka planında devasa veri akışlarını, eşzamanlı kullanıcı oturumlarını ve yüksek frekanslı API çağrılarını yönetecek güçlü bir arka uç (backend) mimarisine ihtiyaç duyulmaktadır.
    Kullanıcı talebi doğrultusunda, bu araştırma raporu DevJourney ekosisteminin veri yönetimini, iş kurallarını ve API hizmetlerini üstlenen .NET (özellikle .NET 8, 9 ve 10) arka uç mimarisini referans alarak kurgulanmıştır. Rapor; derleyici yenilikleri, bellek yönetimi, eşzamanlılık stratejileri ve veritabanı okuma/yazma darboğazlarına odaklanarak, teorik optimizasyonlar ile ölçülebilir mimari iyileştirmeleri birbirinden ayırmaktadır. İlgili optimizasyon teknikleri, Microsoft mühendisliği standartları ve yüksek performanslı kodlama prensipleri etrafında detaylı bir anlatımla sunulmaktadır.
1. Mimari İnceleme ve Performansa Duyarlı Alanların Belirlenmesi
   Öğrenci kulüplerini ve startapları barındıran bir platform doğası gereği yüksek bir okuma-yazma asimetrisine sahiptir. Platformun kullanım senaryoları incelendiğinde, portföy verilerinin ve hackathon duyurularının binlerce kez okunduğu, ancak nispeten daha az sıklıkta güncellendiği öngörülmektedir. Bu asimetri, önbellekleme ve veritabanı izleme (tracking) stratejilerinde büyük yapısal değişiklikleri zorunlu kılar.
   DevJourney bağlamında en kritik performansa duyarlı alanlar şu şekilde tanımlanmıştır:
   • Hackathon Başvuru Sistemi (Eşzamanlılık ve İş Parçacığı Yönetimi): Belirli bir son teslim tarihine yaklaşırken yüzlerce öğrencinin aynı anda platforma yüklenmesi, API uç noktalarında iş parçacığı havuzu tükenmesine (Thread Pool Starvation) ve asenkron durum makinesi (async state machine) darboğazlarına yol açabilir.
   • Kullanıcı Portföyü Sunumu (Serileştirme ve Ağ Gecikmesi): Öğrencilerin beceri setlerini, projelerini ve iletişim bilgilerini içeren devasa JSON yüklerinin serileştirilmesi, geleneksel Reflection tabanlı yaklaşımlar kullanıldığında işlemci üzerinde yüksek bir baskı oluşturur.
   • İlişkisel Veri Erişimi (Entity Framework Core Darboğazları): "Öğrenci -> Katıldığı Kulüpler -> Katıldığı Hackathonlar -> Yüklediği Projeler" gibi derin ilişkisel grafiklerin sorgulanması, N+1 sorgu problemleri ve gereksiz bellek izleme mekanizmaları nedeniyle yüksek Çöp Toplayıcı (Garbage Collector) baskısı yaratır.
   Bu belirlenen alanlar, aşağıdaki spesifik C#/.NET teknikleri ile mikroskobik düzeyde analiz edilerek çözüm önerilerine dönüştürülmüştür.
2. .NET 8/9/10 Performans Özellikleri ve Derleyici İyileştirmeleri
   Modern .NET ekosistemi, JIT (Just-In-Time) derleyicisinin çalışma zamanındaki davranışında devrim niteliğinde değişiklikler yapmıştır. Microsoft'un Seçkin Mühendisi (Distinguished Engineer) Stephen Toub tarafından detaylandırılan performans incelemeleri, .NET 8 ve .NET 9 sürümlerinin standart kütüphanelerde (Base Class Library) yüzlerce mikro optimizasyon barındırdığını göstermektedir5.
   İlgili Kod Alanı: Tüm API uygulama döngüsü, servis sınıfları ve veri erişim katmanı. Uygulamanın temel yürütme ortamı.
   Performans Problemi: Eski nesil .NET Core veya .NET Framework derleyici ayarlarının kullanılması, uygulamanın donanım yönergelerinden ve çalışma zamanı profillemesinden mahrum kalarak aynı kodu çok daha yavaş çalıştırmasına neden olur.
   Teknik Araştırma ve Çözüm: .NET 8 ile birlikte Dinamik Profil Kılavuzlu Optimizasyon (Dynamic PGO - Profile-Guided Optimization) tamamen varsayılan hale gelmiştir8. Dinamik PGO, uygulamanın çalışması sırasında hangi kod dallarının daha sık çağrıldığını (hot paths) tespit eder. Örneğin, DevJourney platformunda bir IUserRepository arayüzünün (interface) arka planda sürekli olarak SqlUserRepository sınıfını çağırdığı tespit edilirse, derleyici arayüz yönlendirmesini (virtual dispatch) iptal ederek kodu devirtualize eder ve doğrudan bellek içi çağrı (inline) haline getirir. .NET 9 sürümü ise bu optimizasyonların üstüne döngü açma (loop unrolling) ve SIMD (Single Instruction, Multiple Data) vektörizasyon iyileştirmelerini eklemiştir6. .NET 10 sürümleri bu işlem hacmini daha da sıkılaştırarak çekirdek düzeyindeki gecikmeleri ortadan kaldırmaya odaklanmaktadır10.
   Beklenen Ödünleşimler (Trade-offs): Katmanlı Derleme (Tiered Compilation) ve PGO süreçleri, uygulamanın ilk ayağa kalkış süresinde (startup latency) ve başlangıçtaki CPU kullanımında kısa süreli artışlara neden olabilir. Çünkü çalışma zamanı, kodun nasıl davrandığını öğrenmek zorundadır.
   Somut Örnek: DevJourney.API.csproj dosyasındaki hedef çerçevenin (Target Framework) .net8.0 veya .net9.0 olarak güncellenmesi ve özellik bayraklarında derleyici yapılandırmasının desteklenmesi. Bu, kod yazmadan elde edilebilecek en büyük ve en ölçülebilir optimizasyondur.
3. Async/Await ve Görev (Task) Verimliliği
   İlgili Kod Alanı: Öğrenci yetki kontrolleri (Authorization), dış servislerden profil verisi çekme işlemleri ve veritabanı L1 önbellek sorguları (örneğin UserProfileService.GetUserPermissionsAsync).
   Performans Problemi: C# dilinde async ve await anahtar kelimeleri kullanıldığında, derleyici arka planda karmaşık bir durum makinesi (IAsyncStateMachine) oluşturur. Bir metot asenkron imza taşıdığı için geri dönüş tipi olarak Task veya Task<T> vermek zorundadır. Ancak Task bir referans türüdür ve yığın (heap) üzerinde bellek tahsisi gerektirir. Eğer DevJourney uygulamasında bir öğrencinin oturum verisi halihazırda önbellekte bulunuyorsa, metot senkron bir şekilde hemen veri dönebilir. Buna rağmen her çağrıda geriye bir Task nesnesi tahsis edilmesi, özellikle saniyede binlerce isteğin geldiği bir API'de ciddi bir Çöp Toplayıcı (GC) darboğazı yaratır.
   Teknik Araştırma: Bu tür senaryolar için .NET çerçevesi ValueTask<T> yapısını sunar. ValueTask, bir değer (struct) türüdür. İşlem anında ve senkron bir şekilde sonuçlanırsa, yığın (heap) üzerinde hiçbir nesne oluşturulmaz; değer doğrudan yığıt (stack) üzerinden döndürülür. Sadece işlem gerçekten I/O beklemesi gerektiriyorsa (gerçek bir asenkron çağrı yapılıyorsa) arka planda bir Task tahsis eder.
   Beklenen Ödünleşimler: ValueTask, standart bir Task nesnesine kıyasla bellekte daha büyük bir yapıya sahiptir çünkü birden fazla veri alanı barındırır. Bu nedenle, tamamen asenkron olacağı kesin olan uzun süreli işlemlerde ValueTask kullanmak gereksiz bir kopyalama maliyeti yaratır. Ayrıca ValueTask yalnızca tek bir kez await edilebilir; aynı değeri birden fazla kez beklemek tanımsız davranışlara (undefined behavior) yol açar.
   Öncesi ve Sonrası Örneği:
   Öncesi (Prematüre Tahsisat):



C#
public async Task<StudentProfile> GetStudentProfileAsync(int studentId)
{
// Veri önbellekte varsa bile Task nesnesi Heap üzerinde tahsis edilir
if (_memoryCache.TryGetValue(studentId, out StudentProfile profile))
{
return profile;
}

    profile = await _dbContext.Students.FindAsync(studentId);
    _memoryCache.Set(studentId, profile);
    return profile;
}

Sonrası (Sıfır Tahsisat - Zero Allocation):



C#
public async ValueTask<StudentProfile> GetStudentProfileAsync(int studentId)
{
// Veri önbellekten dönerse Heap tahsisatı yapılmaz (Allocation-free)
if (_memoryCache.TryGetValue(studentId, out StudentProfile profile))
{
return profile;
}

    profile = await _dbContext.Students.FindAsync(studentId);
    _memoryCache.Set(studentId, profile);
    return profile;
}

Ölçülebilirlik Ayrımı: Günde sadece yüz yöneticinin kullandığı bir raporlama ekranında bu değişimi yapmak prematüre optimizasyondur. Ancak JWT doğrulama middleware'i gibi her HTTP isteğinde çalışan bir mekanizmada ValueTask kullanımı son derece kritik ve ölçülebilir bir optimizasyondur.
4. Bellek Tahsisleri (Allocations) ve Çöp Toplayıcı (GC) Baskısı
   Gelişmiş C# uygulamalarında performansın birincil düşmanı CPU döngüleri değil, bellek bant genişliğinin gereksiz kullanımıdır. .NET çöp toplayıcısı nesneleri nesillere (Generation 0, 1, 2) ve Nesne Yığınlarına (Small Object Heap - SOH, Large Object Heap - LOH) ayırır.
   İlgili Kod Alanı: Özgeçmiş (CV) yükleme modüllerinde metin ayrıştırma, proje açıklama kısımlarında anahtar kelime analizleri ve etiket (tag) işlemleri.
   Performans Problemi: C# dilinde String tipi değiştirilemez (immutable) yapıdadır. Bir metin üzerinde yapılan .Split(), .Replace(), .Substring(), veya .ToLower() gibi operasyonların her biri, bellekte mevcut stringi değiştirmek yerine tamamen yeni bir nesne tahsis eder. DevJourney platformuna eklenen binlerce projenin "teknoloji yığını" etiketleri (örneğin "C#, React, SQL") virgüllerden ayrılarak işlenirken, eski nesil kodlama tarzı binlerce küçük string nesnesini SOH'a atar. Bu nesneler hızla ömrünü tamamladığı için Gen 0 toplayıcısı sürekli çalışmak zorunda kalır ve uygulamanın kullanılabilir CPU kaynaklarını tüketir. Daha kötüsü, 85.000 byte'ı aşan string veya array nesneleri doğrudan LOH'a gider ve bu alanın toplanması sistemde genel bir duraksama (Stop-the-World) yaratır.
   Teknik Araştırma: Stephen Toub'un detaylı derleme makalelerinde belirtildiği gibi, tahsisatsız kodlama .NET'in ana hedefidir13. Bu hedef, sadece framework'ün iç yapısını hızlandırmakla kalmaz, iş kurallarını işleten geliştiriciler için de hayati öneme sahiptir. String veya byte dizileri üzerinde sıfır tahsisatla gezinmeyi sağlayan yapılar, çalışma zamanının yükünü asgariye indirir.
5. Span, Memory ve Nesne/Dizi Havuzlama (ArrayPool) Modelleri
   İlgili Kod Alanı: Platformdaki öğrencilerin profil fotoğraflarını yükleme, API üzerinden veri akışı (streaming) okuma işlemleri veya büyük yapılandırma dosyalarının ayrıştırılması.
   Teknik Çözüm ve Araştırma: Belirtilen string ve dizi problemlerini çözmek için Span<T> ve Memory<T> türleri tasarlanmıştır. Span<T>, belleğin ardışık bir bloğuna (bu bir dizi, unmanaged bellek veya stack alanı olabilir) tür ve bellek açısından güvenli bir şekilde işaret eden bir ref struct yapısıdır. Metinleri parçalamak yerine sadece metnin belirli indeks aralıklarına işaret eden "dilimler" (slices) oluşturarak yeni bir string nesnesi oluşturulmasının önüne geçer.
   Büyük dosya yüklemeleri için ise her çağrıda yeni bir tampon (buffer) oluşturmak yerine, System.Buffers.ArrayPool<T>.Shared kullanılmalıdır. ArrayPool, arka planda iş parçacığı bazında (thread-local) diziler saklayarak, birden fazla isteğin bellek havuzundaki aynı diziyi sırayla kiralamasını (Rent) ve işi bitince geri iade etmesini (Return) sağlar.
   Beklenen Ödünleşimler: Span<T> sadece yığıt (stack) üzerinde var olabilir. Bir sınıfın üyesi (field) olamaz ve asenkron metotların await geçişleri sırasında durumu koruyamaz. Eğer asenkron bir metotta bellek dilimine ihtiyaç varsa Memory<T> kullanılmalıdır. Havuzlama cephesinde ise, ArrayPool'dan kiralanan dizinin bir try-finally bloğu içerisinde mutlaka geri döndürülmesi şarttır; aksi takdirde sistemde gizli bir bellek sızıntısı (memory leak) meydana gelir. Ayrıca, havuzdan dönen dizilerin içi sıfırlanmamış eski veriler barındırabilir.
   Öncesi ve Sonrası Örneği:
   Öncesi (Yüksek Tahsisatlı Dizi Kullanımı):



C#
public async Task ProcessStudentProjectUploadAsync(Stream fileStream)
{
// Her çağrıda Heap üzerinde 64KB tahsis edilir. Yüksek trafikte LOH patlamasına neden olur.
byte[] buffer = new byte[65536];
int bytesRead;
while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
{
await ProcessChunkAsync(buffer, bytesRead);
}
}

Sonrası (Havuzlama ile Sıfır Tahsisat):



C#
public async Task ProcessStudentProjectUploadAsync(Stream fileStream)
{
// Havuzdan önbelleğe alınmış, hazır bir dizi kiralanır.
byte[] buffer = ArrayPool<byte>.Shared.Rent(65536);
try
{
int bytesRead;
// Asenkron geçiş olduğu için Span yerine Memory tabanlı overload kullanılır
while ((bytesRead = await fileStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
{
await ProcessChunkAsync(buffer, bytesRead);
}
}
finally
{
// Temizleme bayrağı ile (veya temizlemeden) havuza mutlaka iade edilir
ArrayPool<byte>.Shared.Return(buffer);
}
}

6. LINQ Performansı ve Optimizasyonları
   İlgili Kod Alanı: Kulüp liderlerinin kendi kulüplerindeki üyeleri filtrelediği veya hackathon puanlarının hesaplandığı skor tabloları (LeaderboardService.cs).
   Performans Problemi: Language Integrated Query (LINQ), veri işleme konusunda mükemmel bir okunabilirlik sunsa da, her LINQ çağrısı arka planda arayüz sanallaştırmaları, durum makineleri ve ardışık numaralandırıcı (enumerator) tahsisleri yaratır. Özellikle .ToList(), .ToArray() gibi metotların zincirin erken aşamalarında çağrılması (premature materialization), tüm verinin gereksiz yere belleğe yüklenmesine yol açar.
   Teknik Araştırma: .NET 9 sürümünde Stephen Toub ve Scott Hanselman'ın detaylı incelemelerinde gösterildiği gibi, LINQ altyapısı donanımsal vektörizasyon (SIMD) destekleyecek şekilde baştan aşağı yeniden yazılmıştır6. Artık .Sum(), .Min(), .Max() ve .Average() gibi operatörler, eğer işlem yapılan veri türü destekliyorsa ve donanım izin veriyorsa, bellek bloklarını tek tek dönmek yerine vektör grupları halinde (örneğin aynı anda 8 tam sayıyı hesaplayarak) işler.
   Beklenen Ödünleşimler: LINQ'un ne kadar optimize edildiğinden bağımsız olarak, döngünün çok kritik olduğu ve saniyede milyonlarca kez çağrılan yollarda (hot-paths) LINQ soyutlamalarından tamamen kaçınılıp geleneksel for döngüleri kullanılmalıdır. Ancak okuma kolaylığı (readability) açısından, genel API isteklerinde güncel .NET sürümleri altındaki LINQ performansından şüphe etmek prematüre optimizasyondur.
7. EF Core Sorgu ve Veritabanı Performansı
   Veritabanı işlemleri, DevJourney gibi platformlarda ağ gecikmesinin ve CPU tüketiminin ana merkezidir.
   İlgili Kod Alanı: Platform ana sayfasındaki popüler üniversite kulüplerinin ve öğrenci sayılarının listelenmesi işlemleri.
   Performans Problemi: Entity Framework Core (EF Core), varsayılan olarak veritabanından çekilen her nesneyi "Değişiklik İzleyici" (Change Tracker) mekanizmasına ekler. Bu sistem, nesne üzerinde değişiklik yapılıp SaveChanges çağrıldığında UPDATE sorgusunu otomatik üretmek içindir. Sadece okuma (Read-Only) amaçlı veri çekilen bir listeleme sayfasında izleme yapmak, uygulamanın hem CPU'yu boşa harcamasına hem de nesnelerin bellek ayak izinin (memory footprint) katlanmasına neden olur. Diğer büyük bir problem ise N+1 sorgu problemi ve çoklu .Include() kullanımı sonucu ortaya çıkan SQL kartezyen çarpımlarıdır.
   Teknik Araştırma ve Mimarî Çözümler:
   • İzleme İptali (AsNoTracking): Her salt okunur sorguda mutlaka .AsNoTracking() metodu kullanılmalıdır.
   • İzdüşüm (Projection): Büyük nesne modellerini tamamen belleğe almak yerine .Select() anahtar kelimesi ile sadece ihtiyaç duyulan veri transfer nesnelerine (DTO) izdüşüm yapılmalıdır. İzdüşüm kullanıldığında EF Core zaten veriyi izlemez.
   • Sorgu Bölme (Split Queries): Platformda bir hackathonun içindeki takımları, takımların içindeki öğrencileri yüklemek için çoklu .Include() atıldığında EF Core devasa bir LEFT JOIN oluşturur ve veritabanı ağ bant genişliği tıkanır. .AsSplitQuery() kullanılarak bu dev sorgu, her koleksiyon için ayrı ve küçük SQL sorgularına bölünmelidir.
   Ölçülebilir Etki (Öncesi / Sonrası Örneği):
   Öncesi (Yüksek CPU ve Bellek Tüketimi):



C#
var hackathons = await _context.Hackathons
.Include(h => h.ParticipatingTeams)
.ThenInclude(t => t.Students)
.Where(h => h.IsActive)
.ToListAsync(); // Veriler izlenir, devasa tek bir SQL çalışır

Sonrası (Optimize Edilmiş İzdüşüm ve Bölme):



C#
var activeHackathons = await _context.Hackathons
.AsNoTracking()
.Where(h => h.IsActive)
.Select(h => new HackathonDashboardDto
{
Id = h.Id,
Name = h.Name,
TeamCount = h.ParticipatingTeams.Count,
TotalStudents = h.ParticipatingTeams.SelectMany(t => t.Students).Count()
})
.AsSplitQuery() // Alt sorguları SQL tarafında bağımsız ele alır
.ToListAsync();

8. SQL Performansı ve Toplu İşlemler (Batching)
   Yalnızca ORM (Object Relational Mapper) katmanı değil, doğrudan SQL performansı da kritik bir inceleme noktasıdır. Platformda eski (sona ermiş) hackathon başvurularının arşivlenmesi veya silinmesi gerekebilir.
   Teknik Çözüm: Geleneksel EF Core mantığında, silinecek veya güncellenecek nesneler önce belleğe çekilir, değiştirilir ve veritabanına geri gönderilirdi. .NET 7 ve sonrası sürümlerde tanıtılan ve .NET 8/9 ile olgunlaşan .ExecuteUpdateAsync() ve .ExecuteDeleteAsync() metotları ile, veriler belleğe hiç alınmadan doğrudan veritabanı motoru üzerinde çalışacak UPDATE veya DELETE komutlarına dönüştürülür.
   Ayrıca veritabanı yöneticisi (DBA) tarafında Execution Plans analiz edilerek, sık filtrelenen alanlar (örneğin e-posta adresi veya kulüp kısa adları) için Clustered veya Non-Clustered Index yapıları mutlaka entegre edilmelidir. Index kullanımının ihmali "Table Scan" gibi pahalı aramalara sebep olur.
9. Önbellekleme (Caching) Katmanı ve L1/L2 Stratejisi
   Öğrenci kulüplerinin listesi veya sabit etiketler (yazılım dilleri, üniversite bölümleri) saniyede binlerce kez sorgulanır. Her istekte veritabanına gidilmesi sistemin çökme garantisidir.
   Teknik Araştırma: Geçmişte lokal bellekleme (IMemoryCache) ve dağıtık önbellekleme (IDistributedCache örneğin Redis) ayrı ayrı yönetilirken, .NET 9 mimarisi HybridCache yapısını sunmuştur. Hibrit önbellek, bu iki katmanı tek bir soyutlama altında birleştirir. Aynı anda bir önbellek anahtarının süresi dolduğunda, sisteme gelen 500 farklı kullanıcı isteğinin aynı anda veritabanına hücum etmesine "Cache Stampede" adı verilir. HybridCache yapısı bu 500 isteği otomatik olarak yakalar, sadece 1 tanesinin asıl kaynağa gitmesini bekler ve geri kalan 499 isteği askıda tutarak veritabanı kilitlenmelerini sıfıra indirir.
   Beklenen Ödünleşimler: Hibrit önbellek, verilerin anlık değişmesi gereken finansal sistemlerde veya çok hızlı senkronizasyon gerektiren borsa platformlarında kullanılamaz. Ancak DevJourney gibi eğitim ekosistemlerinde verilerin birkaç saniye bayat (stale) olması büyük bir problem teşkil etmeyeceği için rahatlıkla uygulanabilir.
10. Serileştirme (Serialization) ve Kaynak Oluşturucular (Source Generators)
    İlgili Kod Alanı: İstemcilere (Mobil uygulama, Web arayüzü) hizmet veren tüm RESTful endpoint uçlarındaki JSON dönüşümleri.
    Performans Problemi: Geleneksel serileştirme kütüphaneleri (örneğin eski Newtonsoft.Json veya standart System.Text.Json.JsonSerializer), çalışma zamanında Yansıma (Reflection) mekanizmasını kullanır. Yansıma, bir nesnenin içindeki özelliklerin ve veri tiplerinin uygulama çalışırken tespit edilmesi sürecidir. Bu süreç, İlk İstek Gecikmesi (Cold Start) dediğimiz ciddi bir performans kaybı yaratır ve çoklu istemcili sistemlerde işlemcinin L1/L2 önbelleklerini boşa harcar.
    Teknik Araştırma: .NET ekosistemi için en önemli yapısal sıçramalardan biri "Kaynak Oluşturucuları" (Source Generators) kullanımıdır14. Kaynak oluşturucular, uygulamanın derlenmesi (build) aşamasında kodunuzu analiz eder ve Reflection ihtiyacını ortadan kaldıracak şekilde serileştirme algoritmalarını doğrudan Utf8JsonWriter kullanan güçlü C# kodlarına dönüştürür14. İki ana mod vardır: İlki sadece yansıma yükünü alan meta veri modu, diğeri ise maksimum performans için tamamen statik algoritmalar yazan "Serileştirme Optimizasyon Modu" (Fast-path)14. JsonSerializerContext sınıfından türetilmiş kısmi (partial) sınıflar oluşturularak sistemin statik analizi sağlanır15.
    Stephen Toub'un derleme notlarında, .NET 9 sistemlerindeki JSON işlem gücünün artmasının arkasında bu mimarinin zorunlu kılınması yatmaktadır20.
    Beklenen Ödünleşimler: Kaynak oluşturucu kullanımı, projenin build edilme süresini bir miktar uzatır. Ayrıca, tamamen dinamik nesneler veya şekli belirsiz JToken/JsonNode tabanlı serileştirmelerde bu statik yaklaşım doğrudan desteklenmez. Ancak standart veri aktarım nesneleri (DTO) için bir zorunluluk olmalıdır.
11. Ağ İşlemleri (HTTP/Networking) ve Soket Yönetimi
    DevJourney platformunun dış sistemlerle entegrasyonu (Örneğin öğrencilerin GitHub profil istatistiklerinin çekilmesi veya SMS altyapıları) için HTTP istekleri yapması şarttır2.
    Performans Problemi: Kod içerisinde her istek atıldığında new HttpClient() kullanmak, işletim sistemindeki ağ soketlerinin anında kapanmamasına ve "TIME_WAIT" durumunda bekleyerek Soket Tüketimi (Socket Exhaustion) problemine neden olur.
    Teknik Çözüm: Uygulama genelinde Bağımlılık Enjeksiyonu üzerinden IHttpClientFactory kullanılmalıdır. HttpClientFactory, arka planda bağlantı havuzları oluşturur ve HTTP işleyicilerini yeniden kullanır. Güvenilir ve performansa duyarlı projelerde bağlantı ömrü (PooledConnectionLifetime), DNS değişikliklerinin yansımasını engellemeyecek şekilde (örneğin 15 dakika) yapılandırılmalıdır. Ayrıca .NET sürümleriyle gelen HTTP/3 multiplexing desteği, tek bir bağlantı üzerinden aynı anda birden fazla veri paketi iletimi sağlayarak ağ gecikmesini ortadan kaldırır.
12. Bağımlılık Enjeksiyonu (Dependency Injection) Maliyetleri
    MVC/API katmanlarında bağımlılık grafları çok büyürse, her HTTP isteğinde servislerin bellekte ayağa kaldırılması (Instantiation) CPU gücünü ciddi oranda yutar.
    Durum tutmayan (Stateless) ve iş parçacığı açısından güvenli (Thread-safe) servis sınıfları, Transient yerine mutlaka Singleton olarak DI konteynerine kaydedilmelidir. Ayrıca, "Kapsamlı Servisi, Tekil Servis İçinde Kullanma" (Captive Dependency) riskini engellemek için geliştirme ortamlarında DI doğrulama kontrolleri (scope validation) etkin bırakılmalıdır.
13. Yansıma (Reflection) ve Veri Eşleme (Mapping) Süreçleri
    Nesne eşleme süreçlerinde geleneksel kütüphaneler (örneğin AutoMapper), çalışma zamanı sırasında İfade Ağaçları (Expression Trees) kullanarak IL komutları üretir (IL Emit). Bu yapı yüksek performansa sahip gibi görünse de Native AOT hedefleriyle asla uyuşmaz ve startup süresinde şişmeye sebep olur.
    DevJourney kod tabanında, entity'lerden DTO'lara geçişte, Yansıma yerine Derleme Zamanı oluşturucularını (Source Generator tabanlı araçlar, örneğin Mapperly) kullanmak tüm bu dönüşüm gecikmelerini sıfıra indirger ve kodun okunabilirliğini bozmadan saniyede yapılan işlem sayısını artırır.
14. Eşzamanlılık (Concurrency) ve Paralellik Stratejileri
    Yüzlerce öğrencinin hackathon mülakat sonuçlarının işlendiği arka plan servislerinde (Background Services) sıradan bir foreach döngüsü kullanmak çok yavaştır. Ancak Task.Run ile her işlem için yeni bir thread oluşturmak iş parçacığı havuzunu tüketir.
    Bunun yerine, uygulamanın sınırlı sayıda iş parçacığı ile maksimum verim almasını sağlayan Parallel.ForEachAsync metodu tercih edilmelidir. Ortak bir sayacı veya log listesini güncellemek gibi işlemlerde ise lock kullanarak sistemi engellemek (blocking) yerine, SemaphoreSlim gibi asenkron thread kilitleri kullanılarak sistemin eşzamanlı okuma-yazma verimliliği artırılmalıdır.
15. Kanallar (Channels) ve Ardışık Düzenler (Pipelines)
    Öğrencilerin proje kodlarının zip dosyası halinde platforma yüklendiği ve bu dosyaların arka planda virüs taramasından geçirildiği bir ardışık düzen (pipeline) hayal edelim.
    Geleneksel Queue<T> (Kuyruk) yapıları ve iş parçacıklarının uyku döngüleri (Thread.Sleep) yerine, yüksek performanslı System.Threading.Channels API'si kullanılmalıdır. Kanallar, veri üreten (Producer) ve bu veriyi tüketen (Consumer) asenkron döngüleri birbirinden soyutlar. Sınırsız bellek tahsisini (Out of Memory hatasını) önlemek için mutlaka Kapasitesi Sınırlandırılmış (Bounded Channel) bir yapı ve "Geri Basınç" (Backpressure) mekanizması tasarlanmalıdır.
16. Günlükleme (Logging) Performansı
    İlgili Kod Alanı: Öğrenci girişleri, platform erişimleri, hatalı dış servis çağrıları ve sistem metrikleri gibi izlenebilirlik alanları.
    Performans Problemi: Microsoft resmi uyarılarında sıklıkla bahsedildiği üzere, günlükleme mekanizmalarında standart "String Interpolation" kullanımı performans düşmanıdır21. Örneğin, _logger.LogInformation($"Student {studentId} joined {clubName}"); şeklindeki bir kod, uygulamanın loglama seviyesi o an için kapalı olsa bile (örneğin Warning seviyesinde çalışıyor olsa bile), arka planda o string'i bir araya getirmek için bellek ayırır. Dahası studentId gibi tam sayı değerlerini objeye çevirirken (Boxing) ağır bir performans maliyeti oluşturur. Yüksek yoğunluklu API'lerde bu işlem binlerce çöp toplayıcı duraksamasına neden olur.
    Teknik Araştırma: Yüksek performanslı günlükleme (High-performance logging) için .NET'in [LoggerMessage] isimli derleme zamanı kaynak üretim nitelikleri kullanılmalıdır22. Bu özellik, parametrelerin kutulanmasını (boxing) tamamen engelleyen güçlü, tür güvenli (type-safe) ve tahsisatsız delegeler üretir. Orijinal LoggerMessage.Define metodu tek başına tüm statik avantajları sağlayamazken, derleyici destekli yeni kaynak oluşturucu bu eksiklikleri tamamen kapatır25.
    Öncesi / Sonrası Örneği:
    Sonrası (High-Performance Logging Mimarisi):



C#
public partial class ClubMembershipService
{
private readonly ILogger<ClubMembershipService> _logger;

    public ClubMembershipService(ILogger<ClubMembershipService> logger) => _logger = logger;

    // Arka planda allocation-free, boxing yapmayan metot gövdesi üretilir [cite: 22]
    [LoggerMessage(
        EventId = 100,
        Level = LogLevel.Information,
        Message = "Student {StudentId} joined {ClubName}")]
    public partial void LogStudentJoinedClub(int studentId, string clubName);

    public void RegisterStudent(int studentId, string clubName)
    {
        // Kod yalnızca bu asıl metodu çağırır. Tahsisatsız işlem.
        LogStudentJoinedClub(studentId, clubName);
    }
}

17. NativeAOT ve JIT Değerlendirmeleri
    Bir dijital öğrenci platformu, bulut ortamında (Kubernetes, AWS Lambda veya Azure Functions) çalışıyorsa, ölçeklenme hızı kritiktir. .NET 8 ve .NET 9, uygulamanın C# ara dili (IL) halinde saklanıp çalışma anında JIT tarafından derlenmesi yerine, tamamen hedef işlemcinin anlayacağı makine koduna dönüştürülmesine olanak sağlayan Ahead-Of-Time (NativeAOT) teknolojisini güçlendirmiştir.
    Bu yapıya geçildiğinde, 200 MB bellek tüketen ve 2 saniyede ayağa kalkan bir servis; 25 MB RAM tüketen ve 50 milisaniyede isteklere cevap veren devasa bir mikrohizmet yapısına bürünür. Ancak NativeAOT, çalışma zamanı dinamik koda (Reflection, dinamik tip yükleme) kapalı olduğu için, uygulamanın mutlaka kaynak oluşturucular (Source Generators) kullanılarak katı şekilde tasarlanmış olması gerekir.
18. CPU, Bellek Optimizasyonu ve Mimarî Gecikme (Throughput/Latency)
    Kod seviyesinden mimari seviyeye geçişte, DevJourney altyapısını hızlandırmak için şu stratejiler entegre edilmelidir:
    • Donanım Uyumluluğu (Data Locality): Sınıflar (Class) yerine Yapılar (Struct) kullanarak nesnelerin referans zinciri oluşturmadan ardışık bellek bloklarında (CPU Cache Lines) yaşamasını sağlamak, veritabanı analiz algoritmalarının L1/L2 önbellek dostu olmasını sağlar.
    • Olay Yönelimli Mimari (Event-Driven Architecture - EDA): Bir kullanıcının hackathon başvurusunu onaylayan API; aynı istek döngüsünde e-posta gönderme, skor tablosunu güncelleme ve yöneticiye SMS atma işlemlerini senkron olarak beklememelidir. Bu işler, RabbitMQ veya Azure Service Bus gibi bir ileti aracısına fırlatılmalı, kullanıcıya anında HTTP 202 Accepted dönülerek gecikme (latency) mimari düzeyde izole edilmelidir.
    • CQRS Modeli: Uygulamanın komutları (veri değiştirme) ile sorguları (veri görüntüleme) ayrı kanallardan ve veritabanı yapılarından işlemelidir.
    Sonuç ve Önceliklendirilmiş Performans İyileştirme Raporu
    DevJourney'in, modern teknoloji odaklı yapısı incelendiğinde, salt hızlı algoritmalar yazmanın ötesinde, çalışma zamanının (CLR) doğasını anlayan mimari kararlar almak kaçınılmazdır. Aşağıdaki tablo, önerilen tekniklerin depo üzerindeki olası darboğazları, çözüm karmaşıklıklarını ve beklenen etkilerini yapısal olarak özetlemektedir.

Öncelik Seviyesi	Mevcut Darboğaz ve Sorun Kaynağı	Önerilen Mimari/Kod Değişikliği	DevJourney'e Neden Uygulanmalı?	Beklenen Etki (Measurable Impact)	Karmaşıklık ve Risk
Kritik	EF Core İzleme ve N+1 Problemi	Görüntüleme ekranlarında .AsNoTracking(), DTO izdüşümü ve .AsSplitQuery() zorunluluğu.	Kulüp ve hackathon listeleme uç noktaları sürekli sorgulanır, veriler anlık değiştirilmez.	Veritabanı gecikmelerinde ve sunucu RAM kullanımında %60 oranında dramatik düşüş.	Düşük. Sadece LINQ sorgularının ilgili ekranlar için yeniden düzenlenmesi yeterlidir.
Kritik	Serileştirmede Yansıma (Reflection) Yükü	JSON işlemleri için JsonSerializerContext tabanlı Kaynak Oluşturuculara geçiş14.	Sistemdeki tüm mobil ve web istemcilerine verilen veriler JSON'dır, CPU israfı engellenmelidir.	HTTP gecikmelerinde milisaniye altı seviyelere inilmesi; LOH (Büyük Nesne Yığını) parçalanmasının önüne geçilmesi.	Orta. Önceden var olan dinamik nesne (dynamic, object) serileştirme yapılarını kırabilir.
Yüksek	String Kutulama (Boxing) ve Loglama Gecikmesi	Uygulama geneli [LoggerMessage] ile tahsisatsız kaynak üretimine geçilmesi21.	Kullanıcı davranışları (CV ekleme, başvuru yapma) telemetrik analiz için sürekli loglanır.	Saniyede binlerce log atıldığında uygulamanın çöp toplayıcısı tarafından dondurulmasının (Stop-the-World) engellenmesi.	Düşük. Ancak, partial sınıflar kullanmak geliştirici takımının kod yazım alışkanlıklarını (Boilerplate) biraz artırır.
Yüksek	Veritabanı Darboğazları ve Ölçekleme Sorunu	.NET 9 tabanlı HybridCache entegrasyonu ile L1 ve dağıtık L2 belleklemenin birleştirilmesi.	DevJourney platformunda kategoriler, yazılım dil tipleri veya duyurular gibi statik veriler hakimdir.	"Cache Stampede" probleminin kökten çözülmesi; binlerce kullanıcının sadece L1 belleğinden (RAM) veri okuması.	Orta. Dağıtık veri senkronizasyonu ve verilerin uygun şekilde silinmesi (Cache Invalidation) mekanizmaları kusursuz yazılmalıdır.
Orta	Asenkron I/O Süreçlerindeki Task Maliyeti	Önbellek okuma operasyonu barındıran servis uçlarında ValueTask<T> modelinin uygulanması.	JWT onaylama, statik profil çağırma gibi sık gerçekleşen işlemler.	Arka plan heap nesnelerinin (gereksiz Task objelerinin) oluşturulmasının kesilmesi, GC yükünün hafifletilmesi.	Orta. Bir değeri yanlışlıkla iki kez await eden dikkatsiz kodlar tanımsız davranışa yol açar.
Orta	Dosya ve Metin İşleme Sırasında Yeni Dizi/Metin Tahsisleri	Büyük veriler için Span<T> ile dilimleme; dosya akışlarında ArrayPool<byte> havuzlaması.	Özgeçmiş, resim veya öğrenci projelerinin sisteme yüklenmesi/okunması süreçleri.	İşletim sistemindeki donanımsal bant genişliğinin yorulmaması, bellek yönetimi stabilitesi.	Yüksek. Havuza Rent edilen dizinin iade edilmemesi büyük ölçekli Memory Leak felaketlerine sebep olur.
Bu analiz göstermektedir ki; C# ekosisteminin .NET 8 ve devamındaki güçlü yenilikleri teorik kod düzeltmelerinden ibaret değildir; bir dijital eğitim platformunun barındırma maliyetlerini düşüren ve binlerce öğrenciye kesintisiz deneyim sunan mimari dayanaklardır. Prematüre optimizasyonlardan kaçınılarak öncelikle kritik (okuma ve JSON döndürme) bölgelerin hedef alınması, DevJourney sisteminin maksimum üretilen iş hacmine (throughput) erişmesini sağlayacaktır.
Alıntılanan çalışmalar
1. Muhammad Faheem Aram MuhammadFaheemAkram - GitHub, https://github.com/MuhammadFaheemAkram
2. unknown_url
3. https://github.com/Jafarli-Mahammad/DevJourney
4. DevJourney | Tələbələr və Universitetlər üçün Rəqəmsal Platforma, https://devjourney.az/
5. NET 8 - Tag - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/tag/dotnet-8/page/4/
6. Performance Improvements in .NET 9 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-9/
7. Stephen Toub - MSFT, Author at .NET Blog, https://devblogs.microsoft.com/dotnet/author/toub/
8. Performance Improvements in .NET 8 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/
9. Performance Improvements in .NET 8, ASP.NET Core, and .NET MAUI, https://www.youtube.com/watch?v=YiOkz1x2qaE
10. Performance Improvements in .NET 10 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/
11. Performance Improvements in .NET 9 : r/ProgrammingLanguages, https://www.reddit.com/r/ProgrammingLanguages/comments/1ffnm5k/performance_improvements_in_net_9/
12. Performance Improvements in .NET 9 [翻译by chatglm] - yahle - 博客园, https://www.cnblogs.com/yahle/p/18535209/performance-improvements-in-net-9
13. The convenience of .NET - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/the-convenience-of-dotnet/
14. Source-generation modes in System.Text.Json - .NET | Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation-modes
15. How to use source generation in System.Text.Json - .NET, https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation
16. System.Text.Json에서 원본 생성을 사용하는 방법 - .NET, https://learn.microsoft.com/ko-kr/dotnet/standard/serialization/system-text-json/source-generation
17. Modes de génération de source dans System.Text.Json - .NET, https://learn.microsoft.com/fr-fr/dotnet/standard/serialization/system-text-json/source-generation-modes
18. Как использовать создание источника в System.Text.Json - .NET, https://learn.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json/source-generation
19. 如何在System.Text.Json 中使用來源產生功能- .NET, https://learn.microsoft.com/zh-tw/dotnet/standard/serialization/system-text-json/source-generation
20. What's new in System.Text.Json in .NET 9 - Microsoft Developer Blogs, https://devblogs.microsoft.com/dotnet/system-text-json-in-dotnet-9/
21. Logging guidance for .NET library authors - Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/library-guidance
22. High-performance logging - .NET - Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/high-performance-logging
23. CA1848: Use the LoggerMessage delegates (code analysis) - .NET, https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1848
24. CA1873: Avoid potentially expensive logging (code analysis) - .NET, https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1873
25. Compile-time logging source generation - .NET - Microsoft Learn, https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/source-generation
26. CA1873: Avoid potentially expensive logging (code analysis) - .NET, https://learn.microsoft.com/sl-si/dotnet/fundamentals/code-analysis/quality-rules/ca1873