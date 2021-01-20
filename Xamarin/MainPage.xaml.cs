using ClassLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Forms;

namespace IronLib
{
    /// <summary>
    /// Класс главной страницы 
    /// </summary>
    [DesignTimeVisible(false)]    
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// переменная для запросов к БД
        /// </summary>
        static readonly HttpClient client = new HttpClient();
        /// <summary>
        /// для выбора случайных статей
        /// </summary>
        static readonly Random rnd = new Random();
        public MainPage()
        {
            InitializeComponent();
            Title = "Главная";
            CreatePage();
        }
        private async void CreatePage()
        {
            //узнаю число статей, для случайной генерации ссылок на статьи в главном меню
            int num_of_pages = Convert.ToInt32(await client.GetStringAsync("https://apifordb20200517173817.azurewebsites.net/api/DB/GetSize"));
            //это чтобы статьи не повторялись
            List<int> vs = new List<int>();
            //j временно нужна, пока в БД мало статей. в обновлениях уберём
            int j = 0;
            do
            {
                int number = rnd.Next(1, num_of_pages + 1);
                if (!vs.Contains(number))
                {
                    vs.Add(number);                     
                }
                j++;
            } while (vs.Count <= 9 && j<30);
            //отсюда начинается заполнение ячеек в Grid
            int row = 0;
            int colomn = 0;
            for (int i = 0; i < vs.Count; i++)
            {             
                //тут ссылка для получения контента для ячейки
                string json = await client.GetStringAsync($"https://apifordb20200517173817.azurewebsites.net/api/DB/GetMain/{vs[i]}");
                //дессериализация полученной строки
                JObject jObject = JObject.Parse(json);
                MyPageInMain page = jObject.ToObject<MyPageInMain>();
                //заполнение ячейки. снизу изображение, на ней черный снизу, прозрачный png файл, что также является кнопкой
                Add_Image(page.ContentsForMainImage.UrlToPict, colomn, row);
                Add_Dark(vs[i], colomn, row, page.ContentsForMainLabel.Text);
                Add_Label(page.ContentsForMainLabel.Text, colomn, row);
                if (colomn == 0)
                { 
                    colomn++;
                }
                else
                {
                    colomn--;
                    row++;
                }
            }
        }
        /// <summary>
        /// добавляет изображение в ячейку
        /// </summary>
        /// <param name="url">ссылка на изображение</param>
        /// <param name="colomn">столбец в Grid</param>
        /// <param name="row">строка в Grid</param>
        private void Add_Image(string url, int colomn, int row)
        {
            grid.Children.Add(new Image() { VerticalOptions = LayoutOptions.End, HorizontalOptions = LayoutOptions.FillAndExpand, Aspect = Aspect.Fill, Source = new UriImageSource { Uri = new Uri($"{url}") } },
                colomn, row);
        }

        /// <summary>
        /// добавляет "затемнитель" снизу ячейки, т.к. на фоне белых изображений не видно название статьи. также является кнопкой, для перехода к статье
        /// </summary>
        /// <param name="id">id строки статьи в БД. передаётся на следующую страницу где используется для получения информации с сервера</param>
        /// <param name="colomn">столбец в Grid</param>
        /// <param name="row">строка в Grid</param>
        /// <param name="title">также название статьи</param>
        private void Add_Dark(int id, int colomn, int row, string title)
        {
            //создание кнопки
            ImageButton imageButton = new ImageButton() 
            { Source = ImageSource.FromResource("IronLib.Picts.dark.png"), VerticalOptions = LayoutOptions.FillAndExpand, 
                HorizontalOptions = LayoutOptions.FillAndExpand, Opacity = 0.1};
            //подписываю на event откртие страницы просмотра контента
            imageButton.Clicked += (s, e) => { PageWithText page1 = new PageWithText(id, title); Navigation.PushAsync(page1); };
            grid.Children.Add(imageButton, colomn, row);
        }

        /// <summary>
        /// добавляет название статьи в ячейку
        /// </summary>
        /// <param name="text">текст в ячейке</param>
        /// <param name="colomn">столбец в Grid</param>
        /// <param name="row">строка в Grid</param>
        private void Add_Label(string text, int colomn, int row)
        {
            grid.Children.Add(new Label() {
                Margin = 10, Text = text, FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), VerticalOptions = LayoutOptions.End, HorizontalOptions = LayoutOptions.Start, TextColor = Color.White, MinimumHeightRequest = 150},
                colomn, row);
        }
    }
}