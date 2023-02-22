using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;
using TSP_SA;

public class Program
{


    static void Main(string[] args)
    {
        // Düğüm nesnesini Nodes adlı sınıftan liste olarak çağır
        List<Nodes> NodesList = new List<Nodes>();
        List<DistanceMatrix> DistancesList = new List<DistanceMatrix>();
        //literatür TSP problem txt dosyası 
        string AdressText = "C:\\Users\\hbasr\\Desktop\\Tavlama GSP\\TSP_DATA\\ALL_tsp\\ALL\\att48.tsp";
        string strFile = File.ReadAllText(AdressText);
        //txt'nin Tüm satırları oku
        string[] lines = File.ReadAllLines(AdressText);
        //düğüm koordinat bilgileri txt dosyalarında 6. satırdan başladığı için doğrudan 6 dan başla
        //problem ismi vs lazım olursa Lines[problem adının bulunduğu satırdan parçala ve al]
        for (int i = 6; i < lines.Length; i++)
        {
            //txt de fazla veya düzgün olmayan boşluk karakter yapılarını düzelt
            lines[i] = lines[i].Trim().Replace("\0"," ");
            lines[i] = lines[i].Trim().Replace("  ", " ");
            lines[i] = lines[i].Trim().Replace("   ", " ");
            lines[i] = lines[i].Trim().Replace("\t", " ");
            //her satırı boşluk karakterle parçalara ayır
            string[] parcalar;
            parcalar = lines[i].Split(' ');
            //Eğer i. satırın 0. parçası EOF ise txt okumayı sonlandır.
            if (parcalar[0] == "EOF")
            {
                break;
            }   
            string n = null;
            string x = null;
            string y = null;
            //halen bazı karakterler yapıyı bozabilir
            for (int j = 0; j < parcalar.Length; j++)
            {
                //eğer boş karaktere denk gelirse işlem yapmayacak. dolu bir parça ise 
                if (parcalar[j]!="")
                {
                    //ilk dolu olan parça düğüm numarası
                    if (n==null && x==null & y==null)
                    {
                        n = parcalar[j];
                        continue;
                    }
                    //ikinci dolu olan parça x koordinatı
                    if (n!=null && x==null && y==null)
                    {
                        x = parcalar[j];
                        continue;
                    }
                    //3. dolu olan parça y koordinatı
                    if (n!=null && x!=null && y==null)
                    {
                        y = parcalar[j];
                        break;
                    }
                }
            }
            //node adlı nesne oluştur ve bu nesnenin entitylerini Nodelist listesine ekle
            Nodes node = new Nodes();
            //düğüm numaralı 1 den başlıyor indexlemede kolaylık olsun diye 0'dan başlatmak için 1 eksik
            node.Node = Convert.ToInt32(n)-1;
            node.Coordx = Convert.ToInt32(x);
            node.Coordy = Convert.ToInt32(y);
            NodesList.Add(node);
        }
        //Düğüm listesideki nesne kadar düğüm var
        int NodeCount = NodesList.Count;
        //tüm düğümlerin birbirine olan uzaklıklarını hesapla ve 
        for (int i = 0; i < NodeCount; i++)
        {
            //Console.WriteLine(NodesList[i].Node+"\t"+NodesList[i].Coordx+"\t"+NodesList[i].Coordy);
            for (int j = 0; j < NodeCount; j++)
            {
                DistanceMatrix matrix = new DistanceMatrix();
                matrix.i = NodesList[i].Node;
                matrix.j = NodesList[j].Node;
                if (i==j)
                {
                    matrix.Distance = 0;
                }
                else
                {
                    double x1 = NodesList[i].Coordx;
                    double x2 = NodesList[j].Coordx;
                    double y1 = NodesList[i].Coordy;
                    double y2 = NodesList[j].Coordy;
                    //Karakök içinde x2-x1'in karesi artı y2-y1'in karesi dik uzaklık
                    double xfark = Math.Abs((x2 - x1));
                    double yfark = Math.Abs((y2 - y1));
                    double Karex = Math.Pow(xfark, 2);
                    double Karey = Math.Pow(yfark, 2);
                    double KareTopla = Karex + Karey;
                    double Karekökdistance = Math.Sqrt(KareTopla);
                    matrix.Distance = Karekökdistance;
                }
                DistancesList.Add(matrix);
                
            }
        }
        //Kontol amaçlı listeyi ekrana yazdırma
        //for (int i = 0; i < DistancesList.Count; i++)
        //{
        //    Console.WriteLine(DistancesList[i].i.ToString() + "\t" + DistancesList[i].j.ToString() + "\t" + DistancesList[i].Distance.ToString());
        //}
        double T;
        Console.Write("Sıcaklık Giriniz :  ");
        T = Convert.ToInt32(Console.ReadLine());
        double Delta;
        Random rnd = new Random();
        
        //M üst sınır
        int Mmax = NodeCount/2;
        //ilk rastgele çözüm
        ArrayList Atanabilir = new ArrayList();
        ArrayList Atananlar = new ArrayList();
        ArrayList Globaleniyicozum = new ArrayList();
        //Global en iyi amaç fonk değeri
        double GEAFD=0;
        //Kabul edilen çözüm
        ArrayList KEC = new ArrayList();
        //Kabul edilen çözümün amaç fonk değeri
        double KEAFD = 0;
        for (int i = 0; i < NodeCount; i++)
        {
            Atanabilir.Add(NodesList[i].Node);
        }
        for (int i = 0; i < NodeCount; i++)
        {
            //ilk atanan 0. düğümü ata. diğerlerini rastgele seç
            if (i==0)
            {
                Atananlar.Add(0);
                Atanabilir.Remove(0);
            }
            else
            {
                int atanannode = rnd.Next(1,NodeCount);
                Atananlar.Add(atanannode);
                Atanabilir.Remove(atanannode);
                //Uzaklık listesinin içerisinden select komutu ile i=düğüm i ve j=düğüm j olan nesnenin uzaklık bilgisini çekme
                double Uzaklık = DistancesList.Where(x => x.i == (int)Atananlar[i] && x.j == (int)Atananlar[i-1]).Select(x => x.Distance).FirstOrDefault();
                GEAFD += Uzaklık;
                KEAFD += Uzaklık;
                if (i==NodeCount-1)
                {
                    //Son düğümden sonra başlangıç düğümüne dönme
                    Atananlar.Add(0);  
                    double Uzaklık1 = DistancesList.Where(x => x.i == (int)Atananlar[i] && x.j == (int)Atananlar[0]).Select(x => x.Distance).FirstOrDefault();
                    GEAFD += Uzaklık1;
                    KEAFD += Uzaklık1;
                }
            }
            
            //ilk çözüm global en iyi çözüm olarak kabul et. ilk çözüm kabul edilen çözüm
            Globaleniyicozum.Add(Atananlar[i]);
            KEC.Add(Atananlar[i]);
            //Console.WriteLine(Atananlar[i].ToString());
        }
        Globaleniyicozum.Add(0);
        KEC.Add(0);
        //Console.WriteLine(Atananlar[^1].ToString());
        //Console.WriteLine(GEAFD.ToString());
        int sayac = 0;
        while (T>0.5)
        {
            sayac++;
            // Mmax kadar komşu çözüm üret M değeri burada sabit ama (sıcaklık düştükçe arttırılabilir bir değer de yapılabilir)
            for (int i = 0; i < Mmax; i++)
            {
                ArrayList komsucozum = new ArrayList();
                for (int j = 0; j < Atananlar.Count; j++)
                {
                    komsucozum.Add(KEC[j]);
                }
                //iki düğüm rastegele yer değiştirecek
                int swapnodex = rnd.Next(1, NodeCount - 1);
                Nodey:
                int swapnodey = rnd.Next(1, NodeCount - 1);
                //eğer rastgele seçilen düğümler birbirine eşitse ikinci düğümü tekrar rastgele seçilmesi için Nodey: etikete döner
                if (swapnodex==swapnodey)
                {
                    goto Nodey;
                }
                if (swapnodey>swapnodex)
                {
                    komsucozum.Insert(swapnodey, Atananlar[swapnodex]);
                    komsucozum.RemoveAt(swapnodex);
                }
                if (swapnodey<swapnodex)
                {
                    komsucozum.Insert(swapnodey, Atananlar[swapnodex]);
                    komsucozum.RemoveAt(swapnodex + 1);
                }
                //Komsu çözüm amaç fon değeri = uzaklık
                double KCAFD = 0;
                for (int k = 0; k < komsucozum.Count; k++)
                {
                    // k. ve k-1. düğüm arasındaki uzaklığı bulacağı için k değeri sıfıra eşit olamaz. yoksa index hatası alır k-1 = -1 çıkar
                    if (k!=0)
                    {
                        // Select Distance FROM DistancesList Where i=komsucozum[k] AND j = komsucozum[k-1]
                        double Uzaklık = DistancesList.Where(x => x.i == (int)komsucozum[k] && x.j == (int)komsucozum[k - 1]).Select(x => x.Distance).FirstOrDefault();
                        KCAFD += Uzaklık;
                    }
                   
                }
                //Eğer global çözümden daha iyi ise direkt kabul et. Global ve kabul edilen çözüm olarak ayarla
                if (KCAFD<=GEAFD)
                {
                    //Burda delta - çıkar o yüzden delta hesaplatmadım
                    GEAFD = KCAFD;
                    KEAFD = KCAFD;
                    Globaleniyicozum.Clear();
                    KEC.Clear();
                    for (int k = 0; k < komsucozum.Count; k++)
                    {
                        Globaleniyicozum.Add(komsucozum[k]);
                        KEC.Add(komsucozum[k]);
                    }
                    //Soğutma çizelgesi 0.8-0.99 arası
                    double alfa = rnd.Next(80, 100);
                    double a = alfa / 100;
                    T = T * a;
                    break;
                }
                else
                {
                    //Eğer Global en iyi değilse u değeri hesapla hesaplanan u değeri P=exp(-delta/T) değeri
                    double u = rnd.NextDouble();
                    //Exp(-delta/T)
                    Delta = KCAFD - KEAFD;
                    double exp = Math.Exp((-Delta)/T);
                    if (u<exp)
                    {
                        KEAFD = KCAFD;
                        KEC.Clear();
                        for (int k = 0; k < komsucozum.Count; k++)
                        {
                            Globaleniyicozum.Add(komsucozum[k]);                            KEC.Add(komsucozum[k]);
                        }
                        //Soğutma çizelgesi 0.8-0.99 arası
                        double alfa = rnd.Next(80, 100);
                        double a = alfa / 100;
                        T = T * a;
                        break;
                    }
                }
                
            }
        }
        for (int i = 0; i < Globaleniyicozum.Count; i++)
        {
            Console.Write(Globaleniyicozum[i].ToString() + "--");
        }
        Console.WriteLine("\n Toplam Uzaklık   " + GEAFD.ToString() + "\n Sayaç : "+ sayac.ToString());
    }
}
