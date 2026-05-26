using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;

namespace Naidis_Mobilapp;

public class KarussellPage : ContentPage
{
    public class CarouselItem
    {
        public string Title { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Description { get; set; } = "";
    }

    readonly ObservableCollection<CarouselItem> items;
    readonly CarouselView carouselView;
    readonly Label counterLabel;
    int position = 0;

    public KarussellPage()
    {
        Title = "Karussell";
        Background = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1),
            GradientStops =
            {
                new GradientStop(Color.FromArgb("#17223B"), 0),
                new GradientStop(Color.FromArgb("#4B6587"), 0.55f),
                new GradientStop(Color.FromArgb("#F0E5CF"), 1)
            }
        };

        items = new ObservableCollection<CarouselItem>
        {
            new CarouselItem
            {
                Title = "Päikesetõus",
                ImageUrl = "https://picsum.photos/id/1015/600/400",
                Description = "Mäed, valgus ja rahulik hommik."
            },
            new CarouselItem
            {
                Title = "Metsavaikus",
                ImageUrl = "https://picsum.photos/id/1016/600/400",
                Description = "Roheline mets ja vaikne jalutuskäik."
            },
            new CarouselItem
            {
                Title = "Järvepeegel",
                ImageUrl = "https://picsum.photos/id/1018/600/400",
                Description = "Vesi peegeldab pilvi ja maastikku."
            }
        };

        carouselView = new CarouselView
        {
            ItemsSource = items,
            HeightRequest = 360,
            PeekAreaInsets = new Thickness(36, 0),
            Loop = true,
            ItemTemplate = new DataTemplate(CreateCarouselCard)
        };
        carouselView.PositionChanged += OnPositionChanged;

        var indicatorView = new IndicatorView
        {
            IndicatorColor = Colors.LightGray,
            SelectedIndicatorColor = Color.FromArgb("#F9D923"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 4)
        };
        carouselView.IndicatorView = indicatorView;

        counterLabel = new Label
        {
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            HorizontalTextAlignment = TextAlignment.Center
        };
        UpdateCounter();

        var addButton = new Button
        {
            Text = "Lisa uus pilt",
            BackgroundColor = Color.FromArgb("#F9D923"),
            TextColor = Color.FromArgb("#17223B"),
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 10
        };
        addButton.Clicked += OnAddClicked;

        var fadeButton = new Button
        {
            Text = "Fade animatsioon",
            BackgroundColor = Color.FromArgb("#2F8F9D"),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 10
        };
        fadeButton.Clicked += OnFadeClicked;

        StartAutoScroll();

        var buttonGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            ColumnSpacing = 10,
            Children = { addButton, fadeButton }
        };
        Grid.SetColumn(fadeButton, 1);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 16,
                Children =
                {
                    new Label
                    {
                        Text = "CarouselView õppematerjal",
                        FontSize = 28,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    carouselView,
                    indicatorView,
                    counterLabel,
                    buttonGrid,
                    new Label
                    {
                        Text = "Puuduta kaarti info vaatamiseks. Swipe vasakule kustutab kaardi.",
                        TextColor = Colors.White,
                        FontSize = 13,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            }
        };
    }

    View CreateCarouselCard()
    {
        var card = new Border
        {
            Padding = 0,
            Margin = new Thickness(6, 8),
            BackgroundColor = Colors.Black,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 18 },
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Opacity = 0.35f,
                Radius = 12,
                Offset = new Point(0, 6)
            }
        };

        var grid = new Grid();

        var image = new Image { Aspect = Aspect.AspectFill };
        image.SetBinding(Image.SourceProperty, nameof(CarouselItem.ImageUrl));

        var gradient = new BoxView
        {
            Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 1),
                EndPoint = new Point(0, 0),
                GradientStops =
                {
                    new GradientStop(Colors.Black.WithAlpha(0.82f), 0),
                    new GradientStop(Colors.Black.WithAlpha(0.2f), 0.55f),
                    new GradientStop(Colors.Transparent, 1)
                }
            }
        };

        var title = new Label
        {
            TextColor = Colors.White,
            FontSize = 23,
            FontAttributes = FontAttributes.Bold
        };
        title.SetBinding(Label.TextProperty, nameof(CarouselItem.Title));

        var description = new Label
        {
            TextColor = Colors.White,
            FontSize = 14,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        description.SetBinding(Label.TextProperty, nameof(CarouselItem.Description));

        var textLayout = new VerticalStackLayout
        {
            Margin = new Thickness(16),
            Spacing = 4,
            VerticalOptions = LayoutOptions.End,
            Children = { title, description }
        };

        grid.Children.Add(image);
        grid.Children.Add(gradient);
        grid.Children.Add(textLayout);
        card.Content = grid;

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnCardTapped;
        card.GestureRecognizers.Add(tapGesture);

        var deleteItem = new SwipeItem
        {
            Text = "Kustuta",
            BackgroundColor = Colors.DarkRed
        };
        deleteItem.Invoked += OnSwipeDeleteInvoked;

        return new SwipeView
        {
            RightItems = new SwipeItems { deleteItem },
            Content = card
        };
    }

    async void OnCardTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not CarouselItem item)
        {
            return;
        }

        await DisplayAlertAsync(item.Title, item.Description, "OK");
    }

    void OnAddClicked(object? sender, EventArgs e)
    {
        items.Add(new CarouselItem
        {
            Title = "Rooma tänavad",
            ImageUrl = "https://picsum.photos/id/1029/600/400",
            Description = "Uus dünaamiliselt lisatud kaart ObservableCollectioni kaudu."
        });

        carouselView.Position = items.Count - 1;
        position = carouselView.Position;
        UpdateCounter();
    }

    async void OnFadeClicked(object? sender, EventArgs e)
    {
        await carouselView.FadeToAsync(0.25, 180);
        await carouselView.FadeToAsync(1, 260);
    }

    void OnSwipeDeleteInvoked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not CarouselItem item)
        {
            return;
        }

        items.Remove(item);

        if (items.Count == 0)
        {
            position = 0;
        }
        else
        {
            position = Math.Min(carouselView.Position, items.Count - 1);
            carouselView.Position = position;
        }

        UpdateCounter();
    }

    void OnPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        position = e.CurrentPosition;
        UpdateCounter();
    }

    void StartAutoScroll()
    {
        Dispatcher.StartTimer(TimeSpan.FromSeconds(4), () =>
        {
            if (items.Count == 0)
            {
                return true;
            }

            position = (position + 1) % items.Count;
            carouselView.Position = position;
            return true;
        });
    }

    void UpdateCounter()
    {
        counterLabel.Text = items.Count == 0
            ? "Kaarte pole"
            : $"{Math.Min(position + 1, items.Count)} / {items.Count}";
    }
}
