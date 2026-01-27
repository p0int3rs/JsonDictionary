using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace JsonDictionary
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> listeveriler = new Dictionary<string, string>();
        private string secilenDosyaYolu = "";

        public MainWindow()
        {
            InitializeComponent();
        }
        private void jsonyukle()
        {
                if (File.Exists(secilenDosyaYolu))
                {
                    string json = File.ReadAllText(secilenDosyaYolu);
                    listeveriler = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                               ?? new Dictionary<string, string>();
                    ListeyiGuncelle();
                }
        }
        private void BtnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dosyasecim = new OpenFileDialog
            {
                Filter = "JSON Dosyaları (*.json)|*.json",
                Title = "JSON Dosyasını Seç"
            };

            if (Dosyasecim.ShowDialog() == true)
            {
                secilenDosyaYolu = Dosyasecim.FileName;
                DosyaYolu.Text = secilenDosyaYolu;
                jsonyukle();
            }
        }
        private void ListeKaydet()
        {
                string json = JsonConvert.SerializeObject(listeveriler, Formatting.Indented);
                File.WriteAllText(secilenDosyaYolu, json);
            
        }


        private void ListeyiGuncelle()
        {
            Listebox.Items.Clear();

            foreach (var item in listeveriler)
            {
                Listebox.Items.Add($"{item.Key}  »  {item.Value}");
            }
            if (Listebox.Items.Count > 0)
            {
                Listebox.ScrollIntoView(Listebox.Items[Listebox.Items.Count - 1]);
            }
        }
        private void Temizle()
        {
            eng_textbox.Clear();
            tr_textbox.Clear();
            eng_textbox.Focus();
        }

        private void ekle_buton_Click(object sender, RoutedEventArgs e)
        {
            string ing = eng_textbox.Text.Trim().ToLower();
            string tr = tr_textbox.Text.Trim();

            if (string.IsNullOrEmpty(ing) || string.IsNullOrEmpty(tr))
            {
                MessageBox.Show("Lütfen her iki alanı da doldurun!");
                return;
            }

            if (listeveriler.ContainsKey(ing))
            {
                MessageBox.Show("Bu kelime zaten mevcut. Güncelle butonunu kullanın.");
                return;
            }

            listeveriler.Add(ing, tr);
            ListeKaydet();
            ListeyiGuncelle();
            Temizle();
        }
        private void guncelle_buton_Click(object sender, RoutedEventArgs e)
        {
            string ing = eng_textbox.Text.Trim().ToLower();
            string tr = tr_textbox.Text.Trim();

            if (listeveriler.ContainsKey(ing))
            {
                listeveriler[ing] = tr;
                ListeKaydet();
                ListeyiGuncelle();
                MessageBox.Show("Kelime güncellendi.");
            }
            else
            {
                MessageBox.Show("Güncellenecek kelime bulunamadı.");
            }
        }
        private void sil_buton_Click(object sender, RoutedEventArgs e)
        {
            if (Listebox.SelectedItem == null) return;

            string seciliSatir = Listebox.SelectedItem.ToString();
            string anahtar = seciliSatir.Split('»')[0].Trim();

            if (listeveriler.Remove(anahtar))
            {
                ListeKaydet();
                ListeyiGuncelle();
                Temizle();
            }
        }
    }
}