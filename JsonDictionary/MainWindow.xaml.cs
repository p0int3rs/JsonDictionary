using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JsonDictionary.Model;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace JsonDictionary
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, Word> listeveriler = new();
        private string secilenDosyaYolu = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void jsonyukle()
        {
            if (!File.Exists(secilenDosyaYolu))
                return;

            string json = File.ReadAllText(secilenDosyaYolu);

            listeveriler = JsonConvert.DeserializeObject<Dictionary<string, Word>>(json)
                           ?? new Dictionary<string, Word>();

            ListeyiGuncelle();
        }

        private void BtnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dosyaSecim = new OpenFileDialog
            {
                Filter = "JSON Dosyaları (*.json)|*.json",
                Title = "JSON Dosyasını Seç"
            };

            if (dosyaSecim.ShowDialog() == true)
            {
                secilenDosyaYolu = dosyaSecim.FileName;
                DosyaYolu.Text = secilenDosyaYolu;
                jsonyukle();
            }
        }

        private void ListeKaydet()
        {
            if (string.IsNullOrEmpty(secilenDosyaYolu))
            {
                MessageBox.Show("Önce bir JSON dosyası seçmelisiniz.");
                return;
            }

            string json = JsonConvert.SerializeObject(listeveriler, Formatting.Indented);
            File.WriteAllText(secilenDosyaYolu, json);
        }

        private void ListeyiGuncelle()
        {
            WordItemsControl.ItemsSource = null;
            WordItemsControl.ItemsSource = listeveriler.ToList();

            count_label.Text = $"{listeveriler.Count:D2} Kelime";
        }

        private void Temizle()
        {
            eng_textbox.Clear();
            type_textbox.Clear();
            meaning_textbox.Clear();
            root_textbox.Clear();
            root_type_textbox.Clear();
            root_meaning_textbox.Clear();
            eng_textbox.Focus();
        }

        private void ekle_buton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(secilenDosyaYolu))
            {
                MessageBox.Show("Önce bir JSON dosyası seçmelisiniz.");
                return;
            }

            string kelime = eng_textbox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(kelime))
            {
                MessageBox.Show("Kelime boş olamaz.");
                return;
            }

            if (listeveriler.ContainsKey(kelime))
            {
                MessageBox.Show("Bu kelime zaten mevcut.");
                return;
            }

            Word yeniKelime = new Word
            {
                Type = type_textbox.Text.Trim(),
                Meaning = meaning_textbox.Text.Trim(),
                Root = root_textbox.Text.Trim(),
                RootType = root_type_textbox.Text.Trim(),
                RootMeaning = root_meaning_textbox.Text.Trim()
            };

            listeveriler.Add(kelime, yeniKelime);

            ListeKaydet();
            ListeyiGuncelle();
            Temizle();
        }

        private void guncelle_buton_Click(object sender, RoutedEventArgs e)
        {
            string kelime = eng_textbox.Text.Trim().ToLower();

            if (!listeveriler.ContainsKey(kelime))
            {
                MessageBox.Show("Güncellenecek kelime bulunamadı.");
                return;
            }

            listeveriler[kelime] = new Word
            {
                Type = type_textbox.Text.Trim(),
                Meaning = meaning_textbox.Text.Trim(),
                Root = root_textbox.Text.Trim(),
                RootType = root_type_textbox.Text.Trim(),
                RootMeaning = root_meaning_textbox.Text.Trim()
            };

            ListeKaydet();
            ListeyiGuncelle();
            MessageBox.Show("Kelime güncellendi.");
        }

        private void KartSil_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string kelime = btn?.Tag?.ToString();

            if (string.IsNullOrEmpty(kelime))
                return;

            if (MessageBox.Show($"{kelime} silinsin mi?",
                "Onay",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                listeveriler.Remove(kelime);
                ListeKaydet();
                ListeyiGuncelle();
            }
        }

        private void KartGuncelle_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string kelime = btn?.Tag?.ToString();

            if (string.IsNullOrEmpty(kelime) || !listeveriler.ContainsKey(kelime))
                return;

            Word w = listeveriler[kelime];

            eng_textbox.Text = kelime;
            type_textbox.Text = w.Type;
            meaning_textbox.Text = w.Meaning;
            root_textbox.Text = w.Root;
            root_type_textbox.Text = w.RootType;
            root_meaning_textbox.Text = w.RootMeaning;

            eng_textbox.Focus();
        }

        private void OyunaBasla_Click(object sender, RoutedEventArgs e)
        {
            if (listeveriler == null || listeveriler.Count < 4)
            {
                MessageBox.Show("Oyunu başlatmak için en az 4 kelime olmalı!");
                return;
            }
            GameWindow gameWindow = new GameWindow(listeveriler);
            gameWindow.Owner = this;
            gameWindow.Show();

        }
    }
}