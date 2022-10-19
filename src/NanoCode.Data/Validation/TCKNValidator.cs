using System;

namespace NanoCode.Data.Validation
{
    public static class TCKNValidator
    {
        public static string TCKNTamamla(string TCKN)
        {
            long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;
            C1 = C2 = C3 = C4 = C5 = C6 = C7 = C8 = C9 = Q1 = Q2 = 0;

            Int64 ATCNO;
            if (TCKN.Length == 9 && Int64.TryParse(TCKN, out ATCNO))
            {
                //Int64 ATCNO = Int64.Parse(TCKN);
                //C11 = ATCNO % 10; ATCNO = ATCNO / 10;
                //C10 = ATCNO % 10; ATCNO = ATCNO / 10;
                C9 = ATCNO % 10; ATCNO = ATCNO / 10;
                C8 = ATCNO % 10; ATCNO = ATCNO / 10;
                C7 = ATCNO % 10; ATCNO = ATCNO / 10;
                C6 = ATCNO % 10; ATCNO = ATCNO / 10;
                C5 = ATCNO % 10; ATCNO = ATCNO / 10;
                C4 = ATCNO % 10; ATCNO = ATCNO / 10;
                C3 = ATCNO % 10; ATCNO = ATCNO / 10;
                C2 = ATCNO % 10; ATCNO = ATCNO / 10;
                C1 = ATCNO % 10; ATCNO = ATCNO / 10;
                Q1 = (((C1 + C3 + C5 + C7 + C9) * 7) - (C2 + C4 + C6 + C8)) % 10;
                Q2 = (C1 + C2 + C3 + C4 + C5 + C6 + C7 + C8 + C9 + Q1) % 10;
            }

            return TCKN + Q1.ToString() + Q2.ToString();
        }

        public static bool IsValid(string TCKimlikNo)
        {
            /* TC Kimlik No Güvenlik Algoritması
             * ---------------------------------
             * TC Kimlik numaraları 11 basamaktan oluşmaktadır. İlk 9 basamak arasında kurulan bir algoritma bize 10. basmağı, ilk 10 basamak arasında kurulan algoritma ise bize 11. basamağı verir.
             * 11 hanelidir.
             * Her hanesi rakamsal değer içerir.
             * İlk hane 0 olamaz. 
             * 1. 3. 5. 7. ve 9. hanelerin toplamının 7 katından, 2. 4. 6. ve 8. hanelerin toplamı çıkartıldığında, elde edilen sonucun 10'a bölümünden kalan, yani Mod10'u bize 10. haneyi verir.
             * 1. 2. 3. 4. 5. 6. 7. 8. 9. ve 10. hanelerin toplamından elde edilen sonucun 10'a bölümünden kalan, yani Mod10'u bize 11. haneyi verir.
             */

            /* T.C. Kimlik No Akraba Algoritması
             * ---------------------------------
             * Rastgele bir tc no yazalım ( ilk 9 hane ):342165846
             * Bunu ilk 5 ve son 4 hane olacak şekilde parçalıyoruz.
             * Sizden yaşca aşağı inmek için (Kardeş , Kuzen vs.) 
             * 34216 => 5 basamaklı bu sayıyı 6 azaltıyoruz.
             * 5846 => 4 basamaklı bu sayıyı 2 artırıyoruz.
             */

            /*
             * T.C. kimlik no’sunun ilk 9 rakamından 29.999 çıkarıldığında sizden önceki kişinin T.C. kimlik no’suna ulaşıldığı
             */

            try
            {
                bool returnValue = false;
                if (TCKimlikNo.Length == 11)
                {
                    long C1, C2, C3, C4, C5, C6, C7, C8, C9, C10, C11, Q1, Q2;

                    Int64 ATCNO = Int64.Parse(TCKimlikNo);
                    C11 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C10 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C9 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C8 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C7 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C6 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C5 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C4 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C3 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C2 = ATCNO % 10; ATCNO = ATCNO / 10;
                    C1 = ATCNO % 10; ATCNO = ATCNO / 10;
                    Q1 = (((C1 + C3 + C5 + C7 + C9) * 7) - (C2 + C4 + C6 + C8)) % 10;
                    Q2 = (C1 + C2 + C3 + C4 + C5 + C6 + C7 + C8 + C9 + Q1) % 10;

                    if (C1 > 0 && C10 == Q1 && C11 == Q2)
                        returnValue = true;
                }
                return returnValue;
            }
            catch
            {
                return false;
            }
        }

        /*
        public static bool TCKimlikNoDogrula(long TCKimlikNo, string Ad, string Soyad, int DogumYili)
        {
            try
            {
                using (tckimlik.nvi.gov.tr.Service.KPSPublic.KPSPublicSoapClient client = new tckimlik.nvi.gov.tr.Service.KPSPublic.KPSPublicSoapClient())
                {
                    return client.TCKimlikNoDogrula(TCKimlikNo, Ad, Soyad, DogumYili);
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool KisiVeCuzdanDogrula(long TCKimlikNo, string Ad, string Soyad, bool SoyadYok, System.Nullable<int> DogumGun, bool DogumGunYok, System.Nullable<int> DogumAy, bool DogumAyYok, int DogumYil, string CuzdanSeri, System.Nullable<int> CuzdanNo, string TCKKSeriNo)
        {
            try
            {
                using (tckimlik.nvi.gov.tr.Service.KPSPublicV2.KPSPublicV2SoapClient client = new tckimlik.nvi.gov.tr.Service.KPSPublicV2.KPSPublicV2SoapClient())
                {
                    return client.KisiVeCuzdanDogrula(TCKimlikNo, Ad, Soyad, SoyadYok, DogumGun, DogumGunYok, DogumAy, DogumAyYok, DogumYil, CuzdanSeri, CuzdanNo, TCKKSeriNo);
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool YabanciKimlikNoDogrula(long KimlikNo, string Ad, string Soyad, System.Nullable<int> DogumGun, System.Nullable<int> DogumAy, int DogumYil)
        {
            try
            {
                using (tckimlik.nvi.gov.tr.Service.KPSPublicYabanciDogrula.KPSPublicYabanciDogrulaSoapClient client = new tckimlik.nvi.gov.tr.Service.KPSPublicYabanciDogrula.KPSPublicYabanciDogrulaSoapClient())
                {
                    return client.YabanciKimlikNoDogrula(KimlikNo, Ad, Soyad, DogumGun, DogumAy, DogumYil);
                }
            }
            catch
            {
                return false;
            }
        }
        */
    }
}
