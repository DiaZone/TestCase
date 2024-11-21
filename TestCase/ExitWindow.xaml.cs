using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Point = System.Windows.Point;

namespace TestCase
{
    /// <summary>
    /// Логика взаимодействия для ExitWindow.xaml
    /// </summary>
    public partial class ExitWindow : Window
    {
        public const int MaxCountOfPoints = 10; //Максимальное количетсво точек
        public const int MinCountOfPoints = 2; //Минимальное количетсво точек

        Dictionary<string, int> TypeOfFigures = new Dictionary<string, int>() //Словарь типов фигур и максимального количества точек для их построения
        {
            {"Circle", 2},
            {"Ellipse", 2},
            {"Line", 2},
            {"Rectangle", 2},
            {"Square", 2},
            {"Polygon", MaxCountOfPoints}
        };

        Dictionary<string, string> Descriptions = new Dictionary<string, string>() //Словарь подсказок экрана
        {
            {"Line", "Укажите две точки, между которыми построится отрезок"},
            {"Ellipse", "Укажите две точки диагонали прямоугольника, в который будет вписана окружность"},
            {"Circle", "Укажите первую точку - центр окружности, затем вторую точку - длина радиуса"},
            {"Rectangle", "Укажите две точки диагонали прямоугольника"},
            {"Square", "Укажите две точки длины квадрата (Учитывается меньшее отклонение координат)"},
            {"Polygon", "Укажите точки, с помощью которых построится многоугольник (Максимум 10)"}
        };

        List<string> FigColors = new List<string>() { "Black", "Red", "Green", "Blue", "Cyan" }; //Список цветов

        public List<Point> coord = new List<Point>(); //Список координат

        public List<Item> myFigures = new List<Item>(); //Список фигур

        public ExitWindow()
        {
            InitializeComponent();
            foreach (var type in TypeOfFigures)
            {
                ItemComboBox.Items.Add(type.Key);
            }
            foreach (var type in FigColors)
            {
                ColorComboBox.Items.Add(type);
            }
            ItemComboBox.SelectedIndex = 0;
            ColorComboBox.SelectedIndex = 0;
        }

        /*
         * Функция добавления фигуры по кнопке
         */
        public void AddNewItemButton_Click(object sender, RoutedEventArgs e)
        {
            //При недостатке точек вызывается окно с уведомлением
            if (coord.Count < MinCountOfPoints)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;
                result = MessageBox.Show("Выбрано слишком мало точек", "Ошибка", button, icon, MessageBoxResult.Yes);

            }
            else
            {
                //Создаем объект класса фигуры и передаем название, цвет и точки построения
                Item figure = new Item(ItemComboBox.SelectedItem.ToString(), GetFigColor(ColorComboBox.SelectedItem.ToString()), coord);

                //Добавляем фигуру в список фигур
                myFigures.Add(figure);

                //Отрисовываем фигуру на канвасе
                DrawerSurface.Children.Add(figure.DrawFigure());

                //Очищаем список точек и удаляем их с экрана
                coord.Clear();
                PointsDeleter();
            }
        }

        /*
         * Функция очищения канваса по кнопке
         */
        public void DeleteAllFigures_Click(object sender, RoutedEventArgs e)
        {
            //Удаляем все дочерние элементы канваса и очищаем список фигур
            DrawerSurface.Children.Clear();
            myFigures.Clear();
        }

        /*
         * Функция удаления точек по кнопке
         */
        public void DeletePointsButton_Click(object sender, RoutedEventArgs e)
        {
            //Очищаем точки с экрана и удаляем координаты
            PointsDeleter();
            coord.Clear();
        }

        /*
         * Функция обработки нажатия по канвасу.
         */
        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Считываем количество точек в списке и ограничение на длину списка
            int pointscount = coord.Count;
            int maxpoints = TypeOfFigures[ItemComboBox.SelectedItem.ToString()];

            //При попытке добавить лишние точки вылезет уведомление
            if (pointscount >= maxpoints)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;
                result = MessageBox.Show("Указано достаточное количество точек", "Ошибка", button, icon, MessageBoxResult.Yes);

            }
            else
            {
                //Записываем координаты курсора
                Point currentPoint = new Point();
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    currentPoint = e.GetPosition(this);
                }

                //Создаем точку на экране и очищаем координаты из памяти
                Ellipse points = new Ellipse();
                points.Width = 4;
                points.Height = 4;
                points.Tag = "PointsToBuild";
                points.StrokeThickness = 4;
                points.Stroke = Brushes.Black;
                points.Margin = new Thickness(currentPoint.X - 2, currentPoint.Y - 2, 0, 0);
                DrawerSurface.Children.Add(points);
                coord.Add(currentPoint);

            }
        }

        /*
         * Функция удаления точек с канваса
         */
        private void PointsDeleter()
        {
            foreach (var el in DrawerSurface.Children.OfType<Ellipse>().ToList())
            {
                if (el.Tag.ToString() == "PointsToBuild")
                    DrawerSurface.Children.Remove(el);
            }
        }

        /*
         * Функция обработки события смены вида фигуры 
         */

        private void ItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PointsDeleter();
            coord.Clear();
            StatusBarText.Text = Descriptions[ItemComboBox.SelectedItem.ToString()];
        }

        /*
         * Функция получения цвета фигуры с экрана
         */
        public System.Drawing.Color GetFigColor(string color)
        {
            return System.Drawing.Color.FromName(color);
        }


    }

    /*
     * Класс фигуры
     * @param Name - Название фигуры
     * @param Color - Цвет фигуры
     * @param Points - Точки для построения фигуры
     */
    public class Item
    {
        public string Name { get; set; }
        public System.Drawing.Color Color { get; set; }

        public List<Point> Points { get; set; }

        /*
         * Конструктор класса
         */
        public Item(string name, System.Drawing.Color color, List<Point> points)
        {
            Name = name;
            Color = color;
            Points = points;
        }

        /*
         * Функция вызова метода для получения необходимой фигуры
         */
        public dynamic DrawFigure()
        {
            switch (Name)
            {
                case "Line":
                    return CreateLine();
                    break;
                case "Ellipse":
                    return CreateEllipse();
                    break;
                case "Circle":
                    return CreateCircle();
                    break;
                case "Rectangle":
                    return CreateRectangle();
                    break;
                case "Square":
                    return CreateSquare();
                    break;
                case "Polygon":
                    return CreatePolygon();
                    break;
                default:
                    return 0;
            }
        }

        /*
         * Функция получения координатных точек для построения линии
         */
        private Polygon CreateLine()
        {
            string name = "Line";
            PointCollection pointcoll = new PointCollection();
            foreach (Point p in Points)
            {
                pointcoll.Add(p);
            }
            return PolyFigDrawer(pointcoll, name, Color);
        }

        /*
         * Функция получения координатных точек для построения эллипса
         */
        private System.Windows.Shapes.Path CreateEllipse()
        {
            string name = "Ellipse";

            Point center = new Point((Points[0].X + Points[1].X) / 2, (Points[0].Y + Points[1].Y) / 2);
            double width = Math.Abs(Points[0].X - Points[1].X);
            double height = Math.Abs(Points[0].Y - Points[1].Y);

            Rect myRect = new Rect();
            myRect.X = center.X - width / 2;
            myRect.Y = center.Y - height / 2;
            myRect.Width = width;
            myRect.Height = height;
            EllipseGeometry ellipse = new EllipseGeometry(myRect);

            return EllipseFigDrawer(ellipse, name, Color);
        }

        /*
         * Функция получения координатных точек для построения круга
         */
        private System.Windows.Shapes.Path CreateCircle()
        {
            string name = "Circle";
            EllipseGeometry ellipse = new EllipseGeometry();
            ellipse.Center = Points[0];
            ellipse.RadiusX = ellipse.RadiusY = Math.Sqrt(Math.Pow((Points[1].X - Points[0].X), 2) + Math.Pow((Points[1].Y - Points[0].Y), 2));
            return EllipseFigDrawer(ellipse, name, Color);
        }

        /*
         * Функция получения координатных точек для построения прямоугольника
         */
        private Polygon CreateRectangle()
        {
            string name = "Rectangle";
            PointCollection pointcoll = new PointCollection
            {
                Points[0],
                new Point(Points[1].X, Points[0].Y),
                Points[1],
                new Point(Points[0].X, Points[1].Y),
            };
            return PolyFigDrawer(pointcoll, name, Color);
        }

        /*
         * Функция получения координатных точек для построения многоугольник
         */

        private Polygon CreatePolygon()
        {
            string name = "Polygon";
            PointCollection pointcoll = new PointCollection();
            foreach (Point p in Points)
            {
                pointcoll.Add(p);
            }
            return PolyFigDrawer(pointcoll, name, Color);

        }

        /*
         * Функция получения координатных точек для построения квадрата
         */

        private Polygon CreateSquare()
        {
            string name = "Square";
            double length = Math.Min(Math.Abs(Points[0].X - Points[1].X), Math.Abs(Points[0].Y - Points[1].Y));
            double xlength = Math.Sign(Points[0].X - Points[1].X) * length;
            double ylength = Math.Sign(Points[0].Y - Points[1].Y) * length;

            PointCollection pointcoll = new PointCollection
            {
                Points[0],
                new Point(Points[0].X - xlength, Points[0].Y),
                new Point(Points[0].X - xlength, Points[0].Y - ylength),
                new Point(Points[0].X, Points[0].Y - ylength),
            };
            return PolyFigDrawer(pointcoll, name, Color);
        }

        /*
         * Функция получения объекта полигональной фигуры по полученной информации из координатных точек
         */
        private Polygon PolyFigDrawer(PointCollection points, string name, System.Drawing.Color myColor)
        {
            Polygon polygon = new Polygon();
            polygon.Points = points;
            polygon.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B));
            polygon.StrokeThickness = 1;
            polygon.Tag = name;

            return polygon;
        }

        /*
         * Функция получения объекта эллипсоидной фигуры по полученной информации из координатных точек
         */
        private System.Windows.Shapes.Path EllipseFigDrawer(EllipseGeometry ellipse, string name, System.Drawing.Color myColor)
        {
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = ellipse;
            path.StrokeThickness = 1;
            path.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B));
            path.Tag = name;

            return path;
        }

    }

}
