using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyKhoSach
{
    class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Quantity { get; set; }

        public Book(int id, string title, string author, string genre, int quantity)
        {
            ID = id;
            Title = title;
            Author = author;
            Genre = genre;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"ID: {ID}, Tên: {Title}, Tác giả: {Author}, Thể loại: {Genre}, Số lượng: {Quantity}";
        }
    }

    class Program
    {
        static List<Book> books = new List<Book>();
        static Stack<string> history = new Stack<string>();
        static Dictionary<string, List<int>> borrowedBooks = new Dictionary<string, List<int>>();
        static Dictionary<int, Queue<string>> waitingQueue = new Dictionary<int, Queue<string>>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Dữ liệu mẫu
            books.Add(new Book(1, "Lập trình C#", "Nguyễn Văn A", "Lập trình", 3));
            books.Add(new Book(2, "Tư duy nhanh và chậm", "Daniel Kahneman", "Tâm lý", 5));
            books.Add(new Book(3, "Đắc nhân tâm", "Dale Carnegie", "Kỹ năng sống", 4));

            while (true)
            {
                Console.WriteLine("\n===== MENU =====");
                Console.WriteLine("1. Thêm sách");
                Console.WriteLine("2. Tìm kiếm sách");
                Console.WriteLine("3. Phân loại sách theo thể loại");
                Console.WriteLine("4. Mượn sách");
                Console.WriteLine("5. Trả sách");
                Console.WriteLine("6. Xem danh sách người mượn");
                Console.WriteLine("7. Xem top 3 sách nhiều nhất");
                Console.WriteLine("8. Xem danh sách thể loại");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ThemSach();
                        break;
                    case "2":
                        TimKiemSach();
                        break;
                    case "3":
                        PhanLoaiSach();
                        break;
                    case "4":
                        XuLyMuonSach();
                        break;
                    case "5":
                        XuLyTraSach();
                        break;
                    case "6":
                        HienThiNguoiMuon();
                        break;
                    case "7":
                        HienThiTopSach();
                        break;
                    case "8":
                        ThongKeTheLoai();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }

        static void ThemSach()
        {
            Console.Write("Nhập ID: ");
            int id = int.Parse(Console.ReadLine());
            var existingBook = books.FirstOrDefault(b => b.ID == id);

            if (existingBook != null)
            {
                Console.Write("Nhập số lượng thêm: ");
                int sl = int.Parse(Console.ReadLine());
                existingBook.Quantity += sl;
                Console.WriteLine("Đã cập nhật số lượng.");
            }
            else
            {
                Console.Write("Nhập tên sách: ");
                string title = Console.ReadLine();
                Console.Write("Nhập tác giả: ");
                string author = Console.ReadLine();
                Console.Write("Nhập thể loại: ");
                string genre = Console.ReadLine();
                Console.Write("Nhập số lượng: ");
                int quantity = int.Parse(Console.ReadLine());
                books.Add(new Book(id, title, author, genre, quantity));
                Console.WriteLine("Đã thêm sách mới.");
            }
        }

        static void TimKiemSach()
        {
            Console.Write("Nhập thể loại hoặc tên tác giả: ");
            string input = Console.ReadLine();
            var result = books.Where(b => b.Genre.Contains(input, StringComparison.OrdinalIgnoreCase)
                                       || b.Author.Contains(input, StringComparison.OrdinalIgnoreCase));

            foreach (var book in result)
            {
                Console.WriteLine(book);
            }
        }

        static void PhanLoaiSach()
        {
            var group = books.GroupBy(b => b.Genre)
                             .Select(g => new { TheLoai = g.Key, TongSo = g.Sum(b => b.Quantity) });

            foreach (var item in group)
            {
                Console.WriteLine($"Thể loại: {item.TheLoai}, Tổng sách: {item.TongSo}");
            }
        }

        static void XuLyMuonSach()
        {
            Console.Write("Nhập tên người mượn: ");
            string name = Console.ReadLine();
            Console.Write("Nhập ID sách cần mượn: ");
            int id = int.Parse(Console.ReadLine());

            var book = books.FirstOrDefault(b => b.ID == id);
            if (book == null)
            {
                Console.WriteLine("Không tìm thấy sách.");
                return;
            }

            if (book.Quantity > 0)
            {
                book.Quantity--;
                history.Push($"Mượn: {id}");
                if (!borrowedBooks.ContainsKey(name))
                    borrowedBooks[name] = new List<int>();
                borrowedBooks[name].Add(id);
                Console.WriteLine("Đã mượn sách.");
            }
            else
            {
                Console.WriteLine("Sách đã hết. Thêm vào hàng chờ.");
                if (!waitingQueue.ContainsKey(id))
                    waitingQueue[id] = new Queue<string>();
                waitingQueue[id].Enqueue(name);
            }
        }

        static void XuLyTraSach()
        {
            Console.Write("Nhập tên người trả: ");
            string name = Console.ReadLine();
            Console.Write("Nhập ID sách trả: ");
            int id = int.Parse(Console.ReadLine());

            var book = books.FirstOrDefault(b => b.ID == id);
            if (book == null)
            {
                Console.WriteLine("Không tìm thấy sách.");
                return;
            }

            if (borrowedBooks.ContainsKey(name) && borrowedBooks[name].Contains(id))
            {
                borrowedBooks[name].Remove(id);
                book.Quantity++;
                history.Push($"Trả: {id}");
                Console.WriteLine("Đã trả sách.");

                if (waitingQueue.ContainsKey(id) && waitingQueue[id].Count > 0)
                {
                    string next = waitingQueue[id].Dequeue();
                    Console.WriteLine($"Người tiếp theo trong hàng chờ: {next}");
                    XuLyMuonSachTuHangCho(next, id);
                }
            }
            else
            {
                Console.WriteLine("Người này không mượn cuốn sách này.");
            }
        }

        static void XuLyMuonSachTuHangCho(string name, int id)
        {
            var book = books.FirstOrDefault(b => b.ID == id);
            if (book != null && book.Quantity > 0)
            {
                book.Quantity--;
                if (!borrowedBooks.ContainsKey(name))
                    borrowedBooks[name] = new List<int>();
                borrowedBooks[name].Add(id);
                history.Push($"Mượn: {id}");
                Console.WriteLine($"{name} đã được mượn sách từ hàng chờ.");
            }
        }

        static void HienThiNguoiMuon()
        {
            var joined = from b in books
                         from kv in borrowedBooks
                         from id in kv.Value
                         where b.ID == id
                         select new { TenNguoi = kv.Key, TenSach = b.Title };

            foreach (var item in joined)
            {
                Console.WriteLine($"{item.TenNguoi} đang mượn: {item.TenSach}");
            }
        }

        static void HienThiTopSach()
        {
            var top = books.OrderByDescending(b => b.Quantity).Take(3);
            Console.WriteLine("Top 3 sách số lượng nhiều nhất:");
            foreach (var book in top)
            {
                Console.WriteLine(book);
            }
        }

        static void ThongKeTheLoai()
        {
            HashSet<string> genres = new HashSet<string>(books.Select(b => b.Genre));
            Console.WriteLine("Danh sách thể loại:");
            foreach (var g in genres)
            {
                Console.WriteLine(g);
            }
        }
    }
}
