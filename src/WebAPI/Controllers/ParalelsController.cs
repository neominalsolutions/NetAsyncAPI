using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
using WebAPI.Context;
using WebAPI.Entity;

namespace WebAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  // 2.5 dakika
  public class ParalelsController : ControllerBase
  {

    private readonly AppDbContext _db;

    public ParalelsController(AppDbContext db)
    {
      _db = db;
    }


    [HttpPost("syncForTest")]
    public IActionResult SyncForTest()
    {
      var stopWatch = Stopwatch.StartNew();
      stopWatch.Start();

      List<double> ints = new List<double>();

      // 1 Milyar Döngü
      Enumerable.Range(0, 10000).ToList().ForEach(i =>
      {
        double d = Math.Pow(2, i) * Math.Sqrt(i) * Math.PI;
        ints.Add(d);
      });

      stopWatch.Stop();
      Console.WriteLine($"elapsedTime" + stopWatch.ElapsedMilliseconds);

      return Ok();

    }


    // 48 saniye 1Milyar Fordaki performansı
    [HttpPost("ParalelForTest")]
    public IActionResult ParalelForTest()
    {
      var stopWatch = Stopwatch.StartNew();
      stopWatch.Start();

      // Not: ParalelFor içerisinde Normal Collcetion kullanmayalım. Concurent Collection
      ConcurrentBag<double> ints = new ConcurrentBag<double>();

      Parallel.For(0, 10000, (i) =>
      {
        double d = Math.Pow(2, i) * Math.Sqrt(i) * Math.PI;
        Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId);
        ints.Add(d);
      });

      //// 1 Milyar Döngü
      //Enumerable.Range(0, 1000000000).ToList().ForEach(i =>
      //{
      //  double d = Math.Pow(2, i) * Math.Sqrt(i) * Math.PI;
      //  ints.Add(d);
      //});

      stopWatch.Stop();
      Console.WriteLine($"elapsedTime" + stopWatch.ElapsedMilliseconds);

      return Ok();

    }



    [HttpPost("ParalelForAsync")]
    public IActionResult ParalelForAsyncTest(CancellationToken cancellationToken)
    {
      var stopWatch = Stopwatch.StartNew();
      stopWatch.Start();

      // Not: ParalelFor içerisinde Normal Collcetion kullanmayalım. Concurent Collection
      ConcurrentBag<double> ints = new ConcurrentBag<double>();

      Parallel.ForAsync(0, 10000, async (i, cancellationToken) =>
      {
        double d = Math.Pow(2, i) * Math.Sqrt(i) * Math.PI;
        Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId);
        ints.Add(d);
      });


      //// 1 Milyar Döngü
      //Enumerable.Range(0, 1000000000).ToList().ForEach(i =>
      //{
      //  double d = Math.Pow(2, i) * Math.Sqrt(i) * Math.PI;
      //  ints.Add(d);
      //});

      stopWatch.Stop();
      Console.WriteLine($"elapsedTime" + stopWatch.ElapsedMilliseconds);
      return Ok();

    }


    [HttpPost("ParalelForeachAsync")]
    public IActionResult ParalelForeachAsyncTest(CancellationToken cancellationToken)
    {
      var stopWatch = Stopwatch.StartNew();
      stopWatch.Start();

      // Not: ParalelFor içerisinde Normal Collcetion kullanmayalım. Concurent Collection
      ConcurrentBag<double> ints = new ConcurrentBag<double>();
      var list = Enumerable.Range(0, 10000);

      Parallel.ForEach(list, async (item, cancellationToken) =>
      {
        double d = Math.Pow(2, item) * Math.Sqrt(item) * Math.PI;
        Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId);

        // Foreach içerisinde veri hesaplarken aynı zamanda servisten bir okuma yazma işlemide yapılabilir. Fakat bu okuma yazma işlemi async non-blocking çalışırken
        // Parallel.ForEach algoritma ve hesaplama CPU based çalışabilir. Hibrit yöntem.
        await Console.Out.WriteAsync("item" + item);

        // Not: Paralel içerisinde async await çalıştırılabilir
        ints.Add(d);
      });

      //   Parallel.ForEach işini bitirince artık senkron kod üzerinde isteğimiz işlemi yapabiliriz.
      foreach (var item in ints)
      {
        // item = 5;
        Console.WriteLine("item" + item);
      }



      //// 1 Milyar Döngü
      //Enumerable.Range(0, 1000000000).ToList().ForEach(i =>
      //{
      //  double d = Math.Pow(2, i) * Math.Sqrt(i) * Math.PI;
      //  ints.Add(d);
      //});

      stopWatch.Stop();
      Console.WriteLine($"elapsedTime" + stopWatch.ElapsedMilliseconds);

      return Ok();

    }


    // Race Condition Simülasyon
    // Paralel Invoke

    [HttpPost("raceCondition")]
    public IActionResult RaceCondition()
    {
      int count = 0; // Shared Data 
      object lockObject = new object(); // başka bir yöntem
      // liste patladı
      // List<int> ints = new List<int>();
      ConcurrentBag<int> ints = new ConcurrentBag<int>();

      Parallel.For(0, 1000000, (i) =>
      {
        // count++;  // Farklı Threadler count değerini güncelliyor. Beklenen sonucunda dışında bir sonuç oluşuyor. Thread Safe çalışmadığımız için.
        // Interlocked.Increment(ref count);
        //Interlocked.Add(ref count, i);
        // ints.Add(i);

        ints.Add(i);

        lock (lockObject)
        {
          count++;
        }


      });


      Console.WriteLine($"Count {count} liste Count {ints.Count}");

      return Ok();
    }


    [HttpPost("concurentCollections")]
    public IActionResult ConcurentCollections()
    {
    
      ConcurrentBag<int> cb = new (); // paralelfor içerisinde sırasız işlemler için çok hızlı
      BlockingCollection<int> bc = new(500); // Bu sıralı bir şekilde thread lock ederek gider o sebebple biraz daha uzun sürer.
      // BlockingCollection direkte olarak kapasite sınırlaması yapılabilir. fakat ConcurrentBag böyle bir özellik yok

      var watch = Stopwatch.StartNew();
      watch.Start();

      Parallel.For(0, 1000000, (i) =>
      {
        // count++;  // Farklı Threadler count değerini güncelliyor. Beklenen sonucunda dışında bir sonuç oluşuyor. Thread Safe çalışmadığımız için.
        // Interlocked.Increment(ref count);
        //Interlocked.Add(ref count, i);
        // ints.Add(i);
        cb.Add(i);
   
      });

      watch.Stop();
      Console.WriteLine($"ConcurrentBag Time {watch.ElapsedMilliseconds}");

      watch.Start();

      Parallel.For(0, 1000000, (i) =>
      {
        if (bc.TryAdd(i))
        {

        }
      });

      watch.Stop();
      Console.WriteLine($"BlockingCollection Time {watch.ElapsedMilliseconds}");



      return Ok();
    }


    [HttpPost("paralelInvoke")]
    public IActionResult ParalelInvoke()
    {
      Action action1 = () =>
      {
        Thread.Sleep(300);
        Console.WriteLine("ThreadID" + Thread.CurrentThread.ManagedThreadId);
      };


      Action action2 = () =>
      {
        Thread.Sleep(300);
        Console.WriteLine("ThreadID" + Thread.CurrentThread.ManagedThreadId);
      };

      Action action3 = () =>
      {
        Thread.Sleep(300);
        Console.WriteLine("ThreadID" + Thread.CurrentThread.ManagedThreadId);
      };

      Parallel.Invoke(action1, action2, action3);

      // ParalelInvoke ise birden fazla uzun süren CPU-bounded işlemin paralel olarak işlenmesini sağlamak için tercih edilir.


      return Ok();
    }


    [HttpPost("Plinq")]
    public IActionResult ParalelLinq()
    {
      // AsParallel sayesinde veritabanında çekilin milyonlarca kayıt programda paralel olarak işlenebiliyor.
      // bu yazım şekli büyük veriilerde ForAll kullanımına göre performans düşürücü bir yöntemdir. 
      List<AppUser> users  = _db.Users.OrderBy(x => x.UserName).AsParallel().AsOrdered().ToList();


      // Not ForAll sıralı yapıyı bozar, yazma işlemlerinde paralel çalışır.
      // WithDegreeOfParallelism(2) 2 farklı thread üzerinde çalıştırmaya zorla
      // WithDegreeOfParallelism önermiyoruz.
      _db.Users.AsParallel().WithDegreeOfParallelism(2).ForAll((item) =>
      {
        // Veriler çekildikten sonra hesaplanacak algoritma
        Console.WriteLine("item" + item.UserName);
        Console.WriteLine("ThreadID" + Thread.CurrentThread.ManagedThreadId);
      });


      return Ok();
    }








  }
}
