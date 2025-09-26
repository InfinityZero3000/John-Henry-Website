using System.Text.RegularExpressions;

namespace JohnHenryFashionWeb.Helpers
{
    public static class GoogleMapsHelper
    {
        public static Dictionary<string, string> GetStoreGoogleMapsIframes()
        {
            // Dictionary mapping store address/location to Google Maps iframe
            var storeIframes = new Dictionary<string, string>
            {
                // Key format: "Province - City/District - Street Address" -> iframe src
                ["Bình Phước - Phước Long - Đường DT 759, P. Phước Bình"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4952.775702124136!2d106.95147277600306!3d11.816443188402726!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317369001639c7f7%3A0x46d73a9fe0b28afa!2zxJDhuqBJIEzDnSBRVeG6pk4gw4FPIE5BTSBKT0hOIEhFTlJZIFBIxq_hu5pDIELDjE5I!5e1!3m2!1svi!2s!4v1758344358125!5m2!1svi!2s",
                
                ["Bình Dương - Thuận An - 53D Nguyễn Văn Tiết, P. Lái Thiêu"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4968.388651293824!2d106.71008557599255!3d10.919523589238082!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174d702ec3d8ab9%3A0xae3898419261767a!2sJohn%20Henry%20Thu%E1%BA%ADn%20An!5e1!3m2!1svi!2s!4v1758345142281!5m2!1svi!2s",
                
                ["Bình Dương - Dĩ An - 223 Nguyễn An Ninh"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4968.643324403909!2d106.76547187599243!3d10.90428988925237!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174d90042eed063%3A0x9669b811dcaf5444!2sJOHN%20HENRY%20TP%20D%C4%A8%20AN!5e1!3m2!1svi!2s!4v1758345198509!5m2!1svi!2s",
                
                ["Đà Nẵng - Liên Chiểu - 26 Ngô Văn Sở, P. Hòa Khánh Bắc"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4862.305498119074!2d108.14781967606423!3d16.068972584610478!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x314219a0dbbcaf89%3A0x805b5c0db09622d2!2zSm9obiBIZW5yeSBMacOqbiBDaGnhu4N1IMSQw6AgTuG6tW5nIDI2IE5nw7QgVsSDbiBT4buf!5e1!3m2!1svi!2s!4v1758345221187!5m2!1svi!2s",
                
                ["Đà Nẵng - Cẩm Lệ - 267 Ông Ích Đường, P. Khuê Trung"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4863.668555674103!2d108.20314167606335!3d16.01311728465836!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31421b8e6e4759d5%3A0x713554e8ed37f0bd!2zSk9ITiBIRU5SWSBDYXE%E1%BA%BzVSUXM4IFHHU6WyATUEhEASKE5HUUIOE!5e1!3m2!1svi!2s!4v1758346970989!5m2!1svi!2s",
                
                ["Đà Nẵng - Sơn Trà - 17 Nguyễn Văn Thoại"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4862.677092188843!2d108.23455547606403!3d16.05376418462352!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3142193f4c5e115f%3A0xabe5e20c5eb71c1a!2zSk9ITiBIRU5SWSBTxqBOIFRSw4A!5e1!3m2!1svi!2s!4v1758346991324!5m2!1svi!2s",
                
                ["Đăk Lăk - Ban Mê Thuột - 255 Lê Duẩn, P. Ea Tam"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4936.922582142575!2d108.03250037601386!3d12.663176587624562!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31721d00544b0ed1%3A0x51d300af9f55feec!2sJOHN%20HENRY%20255%20L%C3%8A%20DU%E1%BA%A8N%20-%20BMT!5e1!3m2!1svi!2s!4v1758347006050!5m2!1svi!2s",
                
                ["Đăk Lăk - Krông Pắc - 21A Lê Duẩn, Thị Trấn Phước An"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4935.980153880688!2d108.29944507601444!3d12.711763887580256!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3171e568cb75f29d%3A0x9ef93cb0f07faf0!2zSm9obiBIZW5yeSBQaMaw4bubYyBBbg!5e1!3m2!1svi!2s!4v1758347022083!5m2!1svi!2s",
                
                ["Đăk Nông - Gia Nghĩa - 20 Tôn Đức Thắng, P. Nghĩa Thành"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1270692.783061612!2d106.4889025599632!3d11.199608838724146!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3173c79a2301e141%3A0xbc3cd51fb5c00f60!2zxJDhuqFpIEzDvSBKb2huIEhlbnJ5!5e1!3m2!1svi!2s!4v1758347887877!5m2!1svi!2s",
                
                ["Đồng Nai - Long Khánh - 14 Nguyễn Thị Minh Khai, P. Xuân An"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4968.1475694005085!2d107.24785467599273!3d10.933924989224584!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174f965009b915f%3A0x48d5395c92c7d543!2zSk9ITiBIRU5SWSBMT05HIEtIw4FOSCAtIFNob3AgcXXhuqduIMOhbyB0aOG7nWkgdHJhbmcgbmFt!5e1!3m2!1svi!2s!4v1758348021106!5m2!1svi!2s",
                
                ["Hà Tĩnh - Hà Tĩnh - 97 Phan Đình Phùng, P. Nam Hà"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4802.978096158339!2d105.90191237610439!3d18.340295782718687!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31384f03f9141033%3A0xc6924a0e454619c!2zSk9ITiBIRU5SWSBIw6AgVMSpbmggLSDwn5GW8J-RlSBTaG9wIHF14bqnbiDDoW8gdGjhu51pIHRyYW5nIG5hbQ!5e1!3m2!1svi!2s!4v1758348155878!5m2!1svi!2s",
                
                ["Hồ Chí Minh - Quận 12 - 372A Nguyễn Ảnh Thủ, P. Trung Mỹ Tây"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4969.377817989768!2d106.60803747599189!3d10.860236689293695!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752b003d5b74ed%3A0x17d19497ea88c85c!2zSk9ITiBIRU5SWSBOR1VZ4buETiDhuqJOSCBUSOG7pg!5e1!3m2!1svi!2s!4v1758348180687!5m2!1svi!2s",
                
                ["Kiên Giang - Phú Quốc - 289 Nguyễn Trung Trực, KP. 5, P. Đông Dương"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4979.664542587641!2d103.96815047598491!3d10.223576489893565!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31a78dd9629798cd%3A0xc50962c6fc40fe28!2sJOHN%20HENRY%20PHU%20QUOC!5e1!3m2!1svi!2s!4v1758348201036!5m2!1svi!2s",
                
                ["Kon Tum - Kon Tum - 152 -154 Lê Hồng Phong"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4902.1273393194!2d107.99993587603728!3d14.350162886107059!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x316bff3efd559983%3A0xf6aa9e86e404dfad!2sJOHN%20HENRY%20Kon%20Tum%20-%20Shop%20%C3%A1o%20qu%E1%BA%A7n%20th%E1%BB%9Di%20trang%20nam!5e1!3m2!1svi!2s!4v1758348217424!5m2!1svi!2s",
                
                ["Long An - Tân An - 69 Nguyễn Trung Trực, P. 2"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4974.680304249129!2d106.41091107598828!3d10.536808889597808!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x310ab7002b6c5229%3A0xc96d56f184906741!2zSm9obiBIZW5yeSBUw6JuIEFuIHwgVGjhu51pIFRyYW5nIE5hbSBDYW8gQ-G6pXAgVMOibiBBbg!5e1!3m2!1svi!2s!4v1758348233479!5m2!1svi!2s",
                
                ["Ninh Bình - Ninh Bình - 209 Lương Văn Thăng, P. Đông Thành"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4746.8308724805565!2d105.97270797614223!3d20.263768781201062!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3136770022cb7a59%3A0x13abeec206af5a82!2sJOHN%20HENRY%20NINH%20B%C3%8CNH!5e1!3m2!1svi!2s!4v1758348249720!5m2!1svi!2s",
                
                ["Phú Yên - Tuy Hòa - 185 Phan Đình Phùng, P. 2"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4928.546230870654!2d109.29714057601946!3d13.088805687237482!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x316fec13e10267e9%3A0x784f8302257b13e4!2sJohn%20Henry%20Tuy%20H%C3%B2a!5e1!3m2!1svi!2s!4v1758348269649!5m2!1svi!2s",
                
                ["Quảng Bình - Đồng Hới - 39 Trần Hưng Đạo"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4826.59412545288!2d106.61666547608834!3d17.47050758343093!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x314757662d44f987%3A0xf27c057373281cb3!2zSk9ITiBIRU5SWSDEkOG7k25nIEjhu5tpIC0g8J-RlvCfkZUgQ-G7rWEgaMOgbmcgcXXhuqduIMOhbyB0aOG7nWkgdHJhbmcgbmFt!5e1!3m2!1svi!2s!4v1758348286182!5m2!1svi!2s",
                
                ["Quảng Nam - Hội An - 169 Lý Thường Kiệt"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4866.822498900965!2d108.32582947606122!3d15.883141884769701!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31420fe44389bfbf%3A0x14d549fb8cb4cbcf!2zSk9ITiBIRU5SWSBI4buYSSBBTiAtIFNob3AgcXXhuqduIMOhbyB0aOG7nWkgdHJhbmcgbmFt!5e1!3m2!1svi!2s!4v1758348305701!5m2!1svi!2s",
                
                ["Quảng Nam - Điện Bàn - 216 Trần Nhân Tông, P Vĩnh Điện"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d77866.61933414411!2d108.20740822605293!3d15.889710468981987!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31420f39e9c6c693%3A0x65030fdb14b51159!2zSk9ITiBIRU5SWSDEkGnhu4duIELDoG4gLSBTaG9wIHF14bqnbiDDoW8gdGjhu51pIHRyYW5nIG5hbQ!5e1!3m2!1svi!2s!4v1758348343765!5m2!1svi!2s",
                
                ["Vĩnh Long - Vĩnh Long - 58 Phạm Thái Bường, P.4"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4979.332347211019!2d105.97518077598517!3d10.244747789873495!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x310a9d6794c2facd%3A0xce9c73e83799e3fa!2sJOHN%20HENRY%20v%C4%A9nh%20long!5e1!3m2!1svi!2s!4v1758347790239!5m2!1svi!2s",
                
                ["Vũng Tàu - Vũng Tàu - 99 Đường 30 Tháng 4, P. Rạch Dừa"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4976.9986612533985!2d107.09879547598673!3d10.39227428973409!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31756f620d5c790f%3A0xc442027c3f702e74!2zSk9ITiBIRU5SWSBWxahORyBUw4BV!5e1!3m2!1svi!2s!4v1758347756523!5m2!1svi!2s",
                
                ["Vũng Tàu - Bà Rịa - 346 Cách Mạng Tháng Tám, P.Phước Trung"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4975.365461144384!2d107.17222537598784!3d10.49429868963786!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3175730050498f73%3A0x61da234dca5067ff!2zSk9ITiBIRU5SWSBCw4AgUuG7ikE!5e1!3m2!1svi!2s!4v1758347740977!5m2!1svi!2s",
                
                ["Hà Nội - Sóc Sơn - 117 Tổ 1 Miếu Thờ Tiền Dược"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4723.441669965313!2d105.77509757615805!3d21.015129280630763!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x313454aa6e1ff1f5%3A0xb1499aaadc72327!2sJohn%20Henry%20The%20Garden%20H%C3%A0%20N%E1%BB%99i!5e1!3m2!1svi!2s!4v1758347612959!5m2!1svi!2s",
                
                ["Hồ Chí Minh - Thủ Đức - 1016 Phạm Văn Đồng, P.Hiệp Bình Chánh"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d4969.5499810402425!2d106.76283777599171!3d10.849885189303363!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317527afbb78d465%3A0xbbf22d36847f2d05!2zSk9ITiBIRU5SWSAtIFZJTkNPTSBUSFXMiSDEkMyvzIFD!5e1!3m2!1svi!2s!4v1758347503826!5m2!1svi!2s",
                
                // Additional stores from the link-google-map.html file
                ["Đăk Lăk - Buôn Ma Thuột - 86 Y Jut, Thành Công"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39492.65582607051!2d108.00480437431642!3d12.680757700000003!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31721d0a7c7e9e2d%3A0x1461fc0830164567!2sJOHN%20HENRY%2086%20Y%20JUT%20CH024!5e1!3m2!1svi!2s!4v1758347151556!5m2!1svi!2s",
                
                ["Đăk Lăk - Buôn Ma Thuột - 38 Nơ Trang Long, Tân tiến"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39492.65582607051!2d108.00480437431642!3d12.680757700000003!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31721d55aaa51171%3A0x9a882819077baa00!2sJohn%20Henry%20N%C6%A1%20Trang%20Long%20CH025!5e1!3m2!1svi!2s!4v1758347170664!5m2!1svi!2s",
                
                ["Đăk Lăk - Buôn Ma Thuột - 51 Ngô Quyền, Tân Lợi"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39491.44738728013!2d108.0146796743164!3d12.688547100000005!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3171f71e2ee2504b%3A0x4b38f5f72806d29b!2sJohn%20Henry%20Ng%C3%B4%20Quy%E1%BB%81n%20CH023!5e1!3m2!1svi!2s!4v1758347241476!5m2!1svi!2s",
                
                ["Đăk Lăk - Buôn Ma Thuột - 58 Phan Chu Trinh, Thành Công"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39491.44738728013!2d108.0146796743164!3d12.688547100000005!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31721d008f221247%3A0xe696c9c5c1fdd207!2sJOHN%20HENRY%2058%20Phan%20Chu%20Trinh%20-%20BMT%20-%20CH123!5e1!3m2!1svi!2s!4v1758347264241!5m2!1svi!2s",
                
                ["Hồ Chí Minh - Tân Phú - 2 Đ. Trường Chinh, Tây Thạnh"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39760.625089126086!2d106.5928637743164!3d10.818068200000004!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317529c48ea60935%3A0x84045baa8b807683!2sJOHN%20HENRY%20%26%20FREELANCER%20Co.op%20Th%E1%BA%AFng%20L%E1%BB%A3i!5e1!3m2!1svi!2s!4v1758347317428!5m2!1svi!2s",
                
                ["Hồ Chí Minh - Quận 5 - 148 Nguyễn Trãi, Phường 2"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39768.63032013212!2d106.64033237431639!3d10.7575321!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752f02cd1295eb%3A0x32f09512edba68b6!2sJohn%20Henry%20Shop!5e1!3m2!1svi!2s!4v1758347345412!5m2!1svi!2s",
                
                ["Hồ Chí Minh - Gò Vấp - 636 Đ. Quang Trung, Phường 11"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39758.239998690406!2d106.62239537431638!3d10.836039799999995!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317529a70e6959c9%3A0xaebc62c8e17c4093!2sJohn%20Henry!5e1!3m2!1svi!2s!4v1758347369614!5m2!1svi!2s",
                
                ["Đăk Lăk - Buôn Ma Thuột - 41 Nguyễn Thị Minh Khai, Thành Công"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d39491.81363865582!2d108.0064587743164!3d12.686186800000003!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31721ddee6da7231%3A0xc99422a91f83c157!2sJOHN%20HENRY%20%26%20FREELANCER%2041%20Nguy%E1%BB%85n%20Th%E1%BB%8B%20Minh%20Khai%20-%20BMT!5e1!3m2!1svi!2s!4v1758347401109!5m2!1svi!2s",
                
                ["Hồ Chí Minh - Tân Phú - 657 Đ. Lũy Bán Bích, Hòa Thạnh"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d318124.7715579685!2d106.44930017561275!3d10.780514000000027!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752f81c149b92d%3A0xecd63245da3c44cc!2zSk9ITiBIRU5SWSBMxahZIELDgU4gQsONQ0g!5e1!3m2!1svi!2s!4v1758347712502!5m2!1svi!2s",
                
                ["Hồ Chí Minh - Gò Vấp - 468 Đ. Quang Trung, Phường 10"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d5019033.3152100835!2d103.72733396237714!3d14.383367549443527!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752996c0e2c7cb%3A0x7a8c639d76a9e560!2sJohnHenry%20Quang%20Trung%20G%C3%B2%20V%E1%BA%A5p%20-%20JohnHenry%26Freelancer!5e1!3m2!1svi!2s!4v1758347834932!5m2!1svi!2s",
                
                ["Đồng Nai - Biên Hòa - Quốc Lộ 51, Phường Long Bình Tân"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d318235.3625751207!2d106.77589222950766!3d10.675400650732623!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174df1b9b43dc33%3A0xcaa01782c04feeb5!2zSk9ITiBIRU5SWSBCaWdDIMSQ4buTbmcgTmFp!5e1!3m2!1svi!2s!4v1758348097695!5m2!1svi!2s",
                
                ["Đồng Nai - Biên Hòa - 491 Phạm Văn Thuận"] = 
                    "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d318235.3625751207!2d106.77589222950766!3d10.675400650732623!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174dfbeab7add9b%3A0xedc0ca081400aae5!2zSk9ITiBIRU5SWSAmIEZSRUVMQU5DRVIgVEFNIEhJ4buGUCxCScOKTiBIw5JB!5e1!3m2!1svi!2s!4v1758348129498!5m2!1svi!2s"
            };

            return storeIframes;
        }

        // Helper method to get iframe by matching store address
        public static string? GetGoogleMapIframe(string storeAddress, string province, string city)
        {
            var iframes = GetStoreGoogleMapsIframes();
            
            // Try different matching strategies
            var normalizedAddress = NormalizeAddress(storeAddress);
            var normalizedProvince = NormalizeProvince(province);
            var normalizedCity = NormalizeCity(city);

            // Strategy 1: Exact key match
            foreach (var kvp in iframes)
            {
                if (kvp.Key.Contains(normalizedProvince) && 
                    kvp.Key.Contains(normalizedCity) && 
                    ContainsAddressKeywords(kvp.Key, normalizedAddress))
                {
                    return kvp.Value;
                }
            }

            // Strategy 2: Partial match by province and street name
            foreach (var kvp in iframes)
            {
                if (kvp.Key.Contains(normalizedProvince) && 
                    ContainsStreetName(kvp.Key, normalizedAddress))
                {
                    return kvp.Value;
                }
            }

            // Strategy 3: Match by province only (fallback)
            foreach (var kvp in iframes)
            {
                if (kvp.Key.Contains(normalizedProvince))
                {
                    return kvp.Value;
                }
            }

            return null;
        }

        private static string NormalizeAddress(string address)
        {
            return address.Replace("TP.", "").Replace("P.", "").Replace("Q.", "")
                         .Replace("H.", "").Replace(",", "").Trim();
        }

        private static string NormalizeProvince(string province)
        {
            return province.Replace("TP.", "").Replace("Tỉnh", "").Trim();
        }

        private static string NormalizeCity(string city)
        {
            return city.Replace("TP.", "").Replace("P.", "").Replace("Q.", "")
                      .Replace("H.", "").Trim();
        }

        private static bool ContainsAddressKeywords(string key, string address)
        {
            var addressParts = address.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var keyLower = key.ToLower();
            
            // Check if key contains at least 2 significant words from address
            int matchCount = 0;
            foreach (var part in addressParts)
            {
                if (part.Length > 2 && keyLower.Contains(part.ToLower()))
                {
                    matchCount++;
                }
            }
            
            return matchCount >= 2;
        }

        private static bool ContainsStreetName(string key, string address)
        {
            // Extract street name (usually the first part before comma)
            var streetPart = address.Split(',')[0].Trim();
            var keyLower = key.ToLower();
            var streetLower = streetPart.ToLower();
            
            // Remove numbers and extract significant words
            var streetWords = Regex.Replace(streetLower, @"\d+[a-z]*", "")
                                  .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                  .Where(w => w.Length > 2)
                                  .ToArray();
            
            // Check if key contains significant street words
            return streetWords.Any(word => keyLower.Contains(word));
        }
    }
}